using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YagizEraslan.Claude.Unity
{
    public class ClaudeChatController : IDisposable
    {
        private readonly ClaudeStreamingApi streamingApi;
        private readonly ClaudeApi claudeApi;
        private readonly ClaudeSettings settings;
        private readonly List<ChatMessage> history = new();
        private readonly Action<ChatMessage, bool> onMessageUpdate;
        private readonly Action<string> onStreamingUpdate;
        private readonly string selectedModelName;
        private readonly bool useStreaming;

        private readonly StringBuilder currentStreamContent = new StringBuilder();
        private bool disposed = false;

        public ClaudeChatController(IClaudeApi api, string modelName, Action<ChatMessage, bool> messageCallback, Action<string> streamingCallback, bool useStreaming)
        {
            var concreteApi = api as ClaudeApi;
            if (concreteApi == null)
            {
                Debug.LogError("ClaudeChatController requires ClaudeApi instance, not just IClaudeApi interface!");
            }

            this.claudeApi = concreteApi;
            this.settings = concreteApi.Settings; // Access settings through the API
            this.streamingApi = new ClaudeStreamingApi();
            this.selectedModelName = modelName;
            this.onMessageUpdate = messageCallback;
            this.onStreamingUpdate = streamingCallback;
            this.useStreaming = useStreaming;
        }

        public void SendUserMessage(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                Debug.LogWarning("User message is empty.");
                return;
            }

            var userChat = new ChatMessage
            {
                role = "user",
                content = userMessage
            };
            history.Add(userChat);
            TrimHistoryIfNeeded();
            onMessageUpdate?.Invoke(userChat, true);

            var request = new ChatCompletionRequest
            {
                model = selectedModelName,
                messages = history.ToArray(),
                stream = useStreaming,
                max_tokens = 500
            };

            if (useStreaming)
            {
                currentStreamContent.Clear();

                // Create placeholder AI message in UI
                var aiMessage = new ChatMessage
                {
                    role = "assistant",
                    content = ""
                };
                onMessageUpdate?.Invoke(aiMessage, false);

                streamingApi.CreateChatCompletionStream(
                    request,
                    claudeApi.ApiKey,
                    partialToken =>
                    {
                        currentStreamContent.Append(partialToken);
                        onStreamingUpdate?.Invoke(currentStreamContent.ToString());
                    });
            }
            else
            {
                HandleFullResponse(request);
            }
        }

        private async void HandleFullResponse(ChatCompletionRequest request)
        {
            try
            {
                var awaitedResponse = await claudeApi.CreateChatCompletion(request);

                if (awaitedResponse != null && awaitedResponse.content != null && awaitedResponse.content.Length > 0)
                {
                    var aiMessage = new ChatMessage
                    {
                        role = awaitedResponse.content[0].role,
                        content = awaitedResponse.content[0].text
                    };
                    history.Add(aiMessage);
                    TrimHistoryIfNeeded();
                    onMessageUpdate?.Invoke(aiMessage, false);
                }
                else
                {
                    Debug.LogWarning("No response content received from Claude API.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while sending message to Claude API: {ex.Message}");
            }
        }

        private void TrimHistoryIfNeeded()
        {
            // Only trim if maxHistoryMessages > 0 (0 means unlimited)
            if (settings.maxHistoryMessages > 0 && history.Count > settings.maxHistoryMessages)
            {
                // Remove oldest messages to prevent unbounded memory growth
                int messagesToRemove = Math.Min(settings.historyTrimCount, history.Count - settings.maxHistoryMessages + settings.historyTrimCount);
                for (int i = 0; i < messagesToRemove; i++)
                {
                    history.RemoveAt(0);
                }
                
                Debug.Log($"[ClaudeChatController] Trimmed {messagesToRemove} old messages from history. Current count: {history.Count}");
            }
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
