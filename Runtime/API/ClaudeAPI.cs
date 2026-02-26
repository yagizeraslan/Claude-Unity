using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Non-streaming implementation of the Claude API.
    /// </summary>
    public class ClaudeApi : IClaudeApi
    {
        private readonly ClaudeSettings settings;

        public string ApiKey => settings.apiKey;
        public ClaudeSettings Settings => settings;

        public ClaudeApi(ClaudeSettings config)
        {
            this.settings = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Creates a chat completion request (legacy method, returns null on failure).
        /// </summary>
        public async Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request)
        {
            var result = await CreateChatCompletionAsync(request);
            return result.GetValueOrDefault();
        }

        /// <summary>
        /// Creates a chat completion request with explicit error handling via Result wrapper.
        /// </summary>
        public async Task<Result<ChatCompletionResponse>> CreateChatCompletionAsync(ChatCompletionRequest request)
        {
            // Input validation
            if (request == null)
            {
                return Result<ChatCompletionResponse>.Failure(
                    $"{ApiConstants.LOG_PREFIX_API} Request cannot be null.");
            }

            if (request.messages == null || request.messages.Length == 0)
            {
                return Result<ChatCompletionResponse>.Failure(
                    $"{ApiConstants.LOG_PREFIX_API} No messages found in request.");
            }

            if (string.IsNullOrEmpty(settings.apiKey))
            {
                return Result<ChatCompletionResponse>.Failure(
                    $"{ApiConstants.LOG_PREFIX_API} API key is not configured.");
            }

            // Ensure max_tokens has a valid value
            int maxTokens = request.max_tokens > 0 ? request.max_tokens : ApiConstants.DEFAULT_MAX_TOKENS_FALLBACK;

            // Build request body
            string prompt = request.messages[request.messages.Length - 1].content;
            string escapedPrompt = prompt.Replace("\\", "\\\\").Replace("\"", "\\\"");
            string body = $@"
    {{
        ""model"": ""{request.model}"",
        ""messages"": [{{""role"": ""user"", ""content"": ""{escapedPrompt}""}}],
        ""max_tokens"": {maxTokens},
        ""stream"": false
    }}";

            byte[] jsonToSend = Encoding.UTF8.GetBytes(body);

            using var www = new UnityWebRequest(ApiConstants.API_URL, "POST");
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader(ApiConstants.HEADER_CONTENT_TYPE, ApiConstants.CONTENT_TYPE_JSON);
            www.SetRequestHeader(ApiConstants.HEADER_API_KEY, settings.apiKey);
            www.SetRequestHeader(ApiConstants.HEADER_ANTHROPIC_VERSION, ApiConstants.API_VERSION);

            try
            {
                await www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    return Result<ChatCompletionResponse>.Failure(
                        $"{ApiConstants.LOG_PREFIX_API} Request failed: {www.error}",
                        httpStatusCode: (int)www.responseCode);
                }

                var json = www.downloadHandler.text;
                var response = JsonUtility.FromJson<ChatCompletionResponse>(json);

                if (response == null)
                {
                    return Result<ChatCompletionResponse>.Failure(
                        $"{ApiConstants.LOG_PREFIX_API} Failed to parse response.");
                }

                return Result<ChatCompletionResponse>.Success(response);
            }
            catch (Exception e)
            {
                return Result<ChatCompletionResponse>.Failure(
                    $"{ApiConstants.LOG_PREFIX_API} Exception: {e.Message}",
                    exception: e);
            }
        }
    }
}
