namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Represents a message in a chat conversation.
    /// </summary>
    [System.Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public ChatMessage() { }

        /// <summary>
        /// Creates a new ChatMessage with the specified role and content.
        /// </summary>
        /// <param name="chatRole">The role of the message sender.</param>
        /// <param name="messageContent">The content of the message.</param>
        public ChatMessage(ChatRole chatRole, string messageContent)
        {
            role = chatRole.ToApiString();
            content = messageContent ?? string.Empty;
        }

        /// <summary>
        /// Gets the ChatRole enum value for this message.
        /// </summary>
        public ChatRole GetRole() => ChatRoleExtensions.FromApiString(role);

        /// <summary>
        /// Returns true if this is a user message.
        /// </summary>
        public bool IsUserMessage => GetRole() == ChatRole.User;

        /// <summary>
        /// Returns true if this is an assistant message.
        /// </summary>
        public bool IsAssistantMessage => GetRole() == ChatRole.Assistant;

        /// <summary>
        /// Returns true if this is a system message.
        /// </summary>
        public bool IsSystemMessage => GetRole() == ChatRole.System;
    }
}
