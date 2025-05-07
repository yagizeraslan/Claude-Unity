using System;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.Claude.Unity
{
    public class ClaudeChatController
    {
        private readonly ClaudeStreamingApi streamingApi;
        private readonly ClaudeApi claudeApi;
        private readonly List<ChatMessage> history = new();
        private readonly Action<ChatMessage, bool> onMessageUpdate;
        private readonly Action<string> onStreamingUpdate;
        private readonly string selectedModelName;
        private readonly bool useStreaming;

        private string currentStreamContent = "";

        public ClaudeChatController(IClaudeApi api, string modelName, Action<ChatMessage, bool> messageCallback, Action<string> streamingCallback, bool useStreaming)
        {
            var concreteApi = api as ClaudeApi;
            if (concreteApi == null)
            {
                Debug.LogError("ClaudeChatController requires ClaudeApi instance, not just IClaudeApi interface!");
            }

            this.claudeApi = concreteApi;
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
                currentStreamContent = "";

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
                        currentStreamContent += partialToken;
                        onStreamingUpdate?.Invoke(currentStreamContent);
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
    }
}
