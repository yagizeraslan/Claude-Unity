using System.Collections.Generic;

namespace YagizEraslan.Claude.Unity
{
    [System.Serializable]
    public class ChatCompletionRequest
    {
        public string model;
        public ChatMessage[] messages;
        public float temperature = 0.7f;
        public bool stream;
        public int max_tokens = 1000;
        public string stop = null;
    }
}