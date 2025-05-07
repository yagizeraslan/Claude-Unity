namespace YagizEraslan.Claude.Unity
{
    [System.Serializable]
    public class ChatCompletionResponse
    {
        public ClaudeMessage[] content;
    }

    [System.Serializable]
    public class ClaudeMessage
    {
        public string role;
        public string text;
    }
}