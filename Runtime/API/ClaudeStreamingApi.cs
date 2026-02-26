using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Streaming implementation of the Claude API using Server-Sent Events (SSE).
    /// </summary>
    public class ClaudeStreamingApi : IClaudeStreamingApi
    {
        private UnityWebRequest currentRequest;
        private bool isCancelled;

        /// <summary>
        /// Creates a streaming chat completion request.
        /// </summary>
        public void CreateChatCompletionStream(
            ChatCompletionRequest request,
            string apiKey,
            Action<string> onStreamUpdate,
            Action<string> onError = null,
            Action onComplete = null)
        {
            // Input validation
            if (request == null)
            {
                var errorMsg = $"{ApiConstants.LOG_PREFIX_STREAMING} Request cannot be null.";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
                return;
            }

            if (request.messages == null || request.messages.Length == 0)
            {
                var errorMsg = $"{ApiConstants.LOG_PREFIX_STREAMING} No messages found in request.";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
                return;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                var errorMsg = $"{ApiConstants.LOG_PREFIX_STREAMING} API key is not configured.";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
                return;
            }

            // Reset cancellation state
            isCancelled = false;

            // Ensure streaming is enabled and max_tokens has a valid value
            request.stream = true;
            int maxTokens = request.max_tokens > 0 ? request.max_tokens : ApiConstants.DEFAULT_MAX_TOKENS_FALLBACK;

            // Build request body
            string prompt = request.messages[request.messages.Length - 1].content;
            string escapedPrompt = prompt.Replace("\\", "\\\\").Replace("\"", "\\\"");
            string body = $@"
            {{
                ""model"": ""{request.model}"",
                ""messages"": [{{""role"": ""user"", ""content"": ""{escapedPrompt}""}}],
                ""max_tokens"": {maxTokens},
                ""stream"": true
            }}";

            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);

            // Create and configure the request
            currentRequest = new UnityWebRequest(ApiConstants.API_URL, "POST");
            currentRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);

            // Create SSE parser and download handler
            var sseParser = new SseParser(onStreamUpdate, () => onComplete?.Invoke(), onError);
            currentRequest.downloadHandler = new ClaudeStreamingDownloadHandler(sseParser);

            currentRequest.SetRequestHeader(ApiConstants.HEADER_CONTENT_TYPE, ApiConstants.CONTENT_TYPE_JSON);
            currentRequest.SetRequestHeader(ApiConstants.HEADER_API_KEY, apiKey);
            currentRequest.SetRequestHeader(ApiConstants.HEADER_ANTHROPIC_VERSION, ApiConstants.API_VERSION);

            currentRequest.SendWebRequest().completed += _ =>
            {
                if (!isCancelled && currentRequest.result != UnityWebRequest.Result.Success)
                {
                    var errorMsg = $"{ApiConstants.LOG_PREFIX_STREAMING} Error: {currentRequest.error}";
                    Debug.LogError(errorMsg);
                    onError?.Invoke(errorMsg);
                }

                // Dispose of the request to prevent memory leaks
                DisposeCurrentRequest();
            };
        }

        /// <summary>
        /// Cancels the current streaming request.
        /// </summary>
        public void CancelStream()
        {
            isCancelled = true;
            if (currentRequest != null)
            {
                currentRequest.Abort();
                DisposeCurrentRequest();
                Debug.Log($"{ApiConstants.LOG_PREFIX_STREAMING} Stream cancelled.");
            }
        }

        private void DisposeCurrentRequest()
        {
            if (currentRequest != null)
            {
                currentRequest.Dispose();
                currentRequest = null;
            }
        }

        /// <summary>
        /// Custom download handler that processes streaming data using SseParser.
        /// </summary>
        private class ClaudeStreamingDownloadHandler : DownloadHandlerScript
        {
            private readonly SseParser sseParser;

            public ClaudeStreamingDownloadHandler(SseParser parser, int bufferSize = 1024)
                : base(new byte[bufferSize])
            {
                this.sseParser = parser ?? throw new ArgumentNullException(nameof(parser));
            }

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (data == null || dataLength == 0) return false;

                string chunk = Encoding.UTF8.GetString(data, 0, dataLength);
                sseParser.ProcessChunk(chunk);
                return true;
            }
        }
    }
}
