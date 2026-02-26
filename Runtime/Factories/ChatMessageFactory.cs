namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Factory class for creating ChatMessage instances.
    /// Provides consistent message creation and eliminates magic strings.
    /// </summary>
    public static class ChatMessageFactory
    {
        /// <summary>
        /// Creates a user message with the specified content.
        /// </summary>
        public static ChatMessage CreateUserMessage(string content)
            => new ChatMessage(ChatRole.User, content);

        /// <summary>
        /// Creates an assistant message with the specified content.
        /// </summary>
        public static ChatMessage CreateAssistantMessage(string content)
            => new ChatMessage(ChatRole.Assistant, content);

        /// <summary>
        /// Creates a system message with the specified content.
        /// </summary>
        public static ChatMessage CreateSystemMessage(string content)
            => new ChatMessage(ChatRole.System, content);

        /// <summary>
        /// Creates an empty assistant message placeholder for streaming responses.
        /// </summary>
        public static ChatMessage CreateStreamingPlaceholder()
            => new ChatMessage(ChatRole.Assistant, string.Empty);

        /// <summary>
        /// Creates a message with the specified role and content.
        /// </summary>
        public static ChatMessage Create(ChatRole role, string content)
            => new ChatMessage(role, content);
    }
}
