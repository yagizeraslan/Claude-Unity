namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Request structure for Claude chat completion API.
    /// </summary>
    [System.Serializable]
    public class ChatCompletionRequest
    {
        public string model;
        public ChatMessage[] messages;
        public float temperature = ApiConstants.DEFAULT_TEMPERATURE;
        public float top_p = ApiConstants.DEFAULT_TOP_P;
        public bool stream;
        public int max_tokens = ApiConstants.DEFAULT_MAX_TOKENS;
        public string stop = null;

        /// <summary>
        /// Creates a new ChatCompletionRequestBuilder for fluent request construction.
        /// </summary>
        public static ChatCompletionRequestBuilder Builder() => new ChatCompletionRequestBuilder();
    }
}
