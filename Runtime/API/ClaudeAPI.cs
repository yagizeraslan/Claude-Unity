using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Claude.Unity
{
    public class ClaudeApi : IClaudeApi
    {
        private readonly ClaudeSettings settings;

        public string ApiKey => settings.apiKey;

        public ClaudeApi(ClaudeSettings config)
        {
            this.settings = config;
        }

        public async Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request)
        {
            if (request.messages == null || request.messages.Length == 0)
            {
                Debug.LogError("ClaudeApi: No messages found in request.");
                return null;
            }

            request.max_tokens = request.max_tokens > 0 ? request.max_tokens : 500;

            string prompt = request.messages[request.messages.Length - 1].content;
            string escapedPrompt = prompt.Replace("\\", "\\\\").Replace("\"", "\\\"");
            string body = $@"
    {{
        ""model"": ""{request.model}"",
        ""messages"": [{{""role"": ""user"", ""content"": ""{escapedPrompt}""}}],
        ""max_tokens"": {request.max_tokens},
        ""stream"": false
    }}";

            byte[] jsonToSend = Encoding.UTF8.GetBytes(body);

            using var www = new UnityWebRequest("https://api.anthropic.com/v1/messages", "POST");
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("x-api-key", settings.apiKey);
            www.SetRequestHeader("anthropic-version", "2023-06-01");

            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Claude Request Failed: {www.error}");
                return null;
            }

            var json = www.downloadHandler.text;
            try
            {
                return JsonUtility.FromJson<ChatCompletionResponse>(json);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse Claude response: " + e.Message);
                return null;
            }
        }
    }
}