using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Claude.Unity
{
    public class ClaudeStreamingApi
    {
        public void CreateChatCompletionStream(ChatCompletionRequest request, string apiKey, Action<string> onStreamUpdate)
        {
            request.stream = true;
            string body = JsonUtility.ToJson(request);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);

            UnityWebRequest req = new UnityWebRequest("https://api.anthropic.com/v1/messages", "POST");
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new ClaudeStreamingDownloadHandler(onStreamUpdate);
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("x-api-key", apiKey);
            req.SetRequestHeader("anthropic-version", "2023-06-01");

            req.SendWebRequest().completed += _ =>
            {
                if (req.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Claude Streaming Error: {req.error}");
            };
        }

        private class ClaudeStreamingDownloadHandler : DownloadHandlerScript
        {
            private readonly StringBuilder buffer = new();
            private readonly Action<string> onStreamUpdate;

            public ClaudeStreamingDownloadHandler(Action<string> onStreamUpdate, int bufferSize = 1024)
                : base(new byte[bufferSize])
            {
                this.onStreamUpdate = onStreamUpdate;
            }

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (data == null || dataLength == 0) return false;

                string chunk = Encoding.UTF8.GetString(data, 0, dataLength);
                buffer.Append(chunk);

                string[] lines = buffer.ToString().Split('\n');
                buffer.Clear();

                foreach (string line in lines)
                {
                    if (line.StartsWith("data: "))
                    {
                        string jsonChunk = line.Substring(6).Trim();
                        if (jsonChunk == "[DONE]") return true;

                        try
                        {
                            var parsed = Newtonsoft.Json.Linq.JObject.Parse(jsonChunk);
                            var delta = parsed["delta"]?["text"]?.ToString();
                            if (!string.IsNullOrEmpty(delta))
                            {
                                onStreamUpdate?.Invoke(delta);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("Claude chunk parse failed: " + e.Message);
                        }
                    }
                }

                return true;
            }
        }
    }
}
