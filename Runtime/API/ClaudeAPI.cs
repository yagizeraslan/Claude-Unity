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
            request.stream = false;
            string body = JsonUtility.ToJson(request);
            byte[] jsonToSend = Encoding.UTF8.GetBytes(body);

            using UnityWebRequest www = new UnityWebRequest("https://api.anthropic.com/v1/messages", "POST");
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

            return JsonUtility.FromJson<ChatCompletionResponse>(www.downloadHandler.text);
        }
    }
}