using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Controller for managing chat interactions with the Claude API.
    /// Supports both streaming and non-streaming modes with dependency injection.
    /// </summary>
    public class ClaudeChatController : IDisposable
    {
        private readonly IClaudeStreamingApi streamingApi;
        private readonly IClaudeApi claudeApi;
        private readonly ClaudeSettings settings;
        private readonly List<ChatMessage> history = new List<ChatMessage>();
        private readonly Action<ChatMessage, bool> onMessageUpdate;
        private readonly Action<string> onStreamingUpdate;
        private readonly Action<string> onError;
        private readonly string selectedModelName;
        private readonly bool useStreaming;

        private readonly StringBuilder currentStreamContent = new StringBuilder();
        private bool disposed = false;

        /// <summary>
        /// Creates a new ClaudeChatController with dependency injection support.
        /// </summary>
        /// <param name="api">The Claude API implementation.</param>
        /// <param name="streamingApi">The streaming API implementation (optional, will create default if null).</param>
        /// <param name="config">The Claude settings configuration.</param>
        /// <param name="modelName">The model name to use for requests.</param>
        /// <param name="messageCallback">Callback invoked when a message is added.</param>
        /// <param name="streamingCallback">Callback invoked during streaming updates.</param>
        /// <param name="errorCallback">Optional callback for error handling.</param>
        /// <param name="useStreaming">Whether to use streaming mode.</param>
        public ClaudeChatController(
            IClaudeApi api,
            IClaudeStreamingApi streamingApi,
            ClaudeSettings config,
            string modelName,
            Action<ChatMessage, bool> messageCallback,
            Action<string> streamingCallback,
            Action<string> errorCallback = null,
            bool useStreaming = false)
        {
            this.claudeApi = api ?? throw new ArgumentNullException(nameof(api));
            this.streamingApi = streamingApi ?? new ClaudeStreamingApi();
            this.settings = config ?? throw new ArgumentNullException(nameof(config));
            this.selectedModelName = modelName;
            this.onMessageUpdate = messageCallback;
            this.onStreamingUpdate = streamingCallback;
            this.onError = errorCallback;
            this.useStreaming = useStreaming;
        }

        /// <summary>
        /// Legacy constructor for backward compatibility.
        /// </summary>
        public ClaudeChatController(
            IClaudeApi api,
            string modelName,
            Action<ChatMessage, bool> messageCallback,
            Action<string> streamingCallback,
            bool useStreaming)
        {
            var concreteApi = api as ClaudeApi;
            if (concreteApi == null)
            {
                Debug.LogError($"{ApiConstants.LOG_PREFIX_CONTROLLER} Requires ClaudeApi instance for legacy constructor.");
                throw new ArgumentException("Legacy constructor requires ClaudeApi instance", nameof(api));
            }

            this.claudeApi = concreteApi;
            this.settings = concreteApi.Settings;
            this.streamingApi = new ClaudeStreamingApi();
            this.selectedModelName = modelName;
            this.onMessageUpdate = messageCallback;
            this.onStreamingUpdate = streamingCallback;
            this.useStreaming = useStreaming;
        }

        /// <summary>
        /// Sends a user message and triggers the appropriate response handling.
        /// </summary>
        public void SendUserMessage(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                Debug.LogWarning($"{ApiConstants.LOG_PREFIX_CONTROLLER} User message is empty.");
                return;
            }

            // Create user message using factory
            var userChat = ChatMessageFactory.CreateUserMessage(userMessage);
            history.Add(userChat);
            TrimHistoryIfNeeded();
            onMessageUpdate?.Invoke(userChat, true);

            // Build request using builder pattern
            var request = ChatCompletionRequest.Builder()
                .WithModel(selectedModelName)
                .WithMessages(history)
                .WithStreaming(useStreaming)
                .WithMaxTokens(ApiConstants.DEFAULT_MAX_TOKENS_FALLBACK)
                .Build();

            if (useStreaming)
            {
                HandleStreamingResponse(request);
            }
            else
            {
                HandleFullResponse(request);
            }
        }

        private void HandleStreamingResponse(ChatCompletionRequest request)
        {
            currentStreamContent.Clear();

            // Create placeholder AI message using factory
            var aiMessage = ChatMessageFactory.CreateStreamingPlaceholder();
            onMessageUpdate?.Invoke(aiMessage, false);

            streamingApi.CreateChatCompletionStream(
                request,
                claudeApi.ApiKey,
                onStreamUpdate: partialToken =>
                {
                    currentStreamContent.Append(partialToken);
                    onStreamingUpdate?.Invoke(currentStreamContent.ToString());
                },
                onError: errorMsg =>
                {
                    Debug.LogError(errorMsg);
                    onError?.Invoke(errorMsg);
                },
                onComplete: () =>
                {
                    // Add completed message to history
                    if (currentStreamContent.Length > 0)
                    {
                        var completedMessage = ChatMessageFactory.CreateAssistantMessage(currentStreamContent.ToString());
                        history.Add(completedMessage);
                        TrimHistoryIfNeeded();
                    }
                });
        }

        private async void HandleFullResponse(ChatCompletionRequest request)
        {
            var result = await claudeApi.CreateChatCompletionAsync(request);

            result.Match(
                onSuccess: response =>
                {
                    if (response?.content != null && response.content.Length > 0)
                    {
                        var aiMessage = ChatMessageFactory.CreateAssistantMessage(response.content[0].text);
                        history.Add(aiMessage);
                        TrimHistoryIfNeeded();
                        onMessageUpdate?.Invoke(aiMessage, false);
                    }
                    else
                    {
                        Debug.LogWarning($"{ApiConstants.LOG_PREFIX_CONTROLLER} No response content received from Claude API.");
                    }
                },
                onFailure: error =>
                {
                    Debug.LogError($"{ApiConstants.LOG_PREFIX_CONTROLLER} {error}");
                    onError?.Invoke(error);
                });
        }

        private void TrimHistoryIfNeeded()
        {
            int trimToCount = settings.maxHistoryMessages - settings.historyTrimCount;
            if (trimToCount < 0) trimToCount = settings.maxHistoryMessages / 2;

            if (HistoryTrimmer.TrimIfNeeded(history, settings.maxHistoryMessages, trimToCount))
            {
                Debug.Log($"{ApiConstants.LOG_PREFIX_CONTROLLER} Trimmed history. Current count: {history.Count}");
            }
        }

        /// <summary>
        /// Cancels any ongoing streaming request.
        /// </summary>
        public void CancelStreaming()
        {
            streamingApi?.CancelStream();
        }

        /// <summary>
        /// Clears the conversation history.
        /// </summary>
        public void ClearHistory()
        {
            history.Clear();
            currentStreamContent.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Cancel any ongoing streaming
                    CancelStreaming();

                    // Clear message history to help with memory cleanup
                    history.Clear();

                    // Clear current stream content
                    currentStreamContent.Clear();
                }

                disposed = true;
            }
        }

        ~ClaudeChatController()
        {
            Dispose(false);
        }
    }
}
