namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Represents the role of a participant in a chat conversation.
    /// </summary>
    public enum ChatRole
    {
        User,
        Assistant,
        System
    }

    /// <summary>
    /// Extension methods for ChatRole enum to convert between enum and API string representations.
    /// </summary>
    public static class ChatRoleExtensions
    {
        /// <summary>
        /// Converts a ChatRole enum value to its API string representation.
        /// </summary>
        public static string ToApiString(this ChatRole role)
        {
            return role switch
            {
                ChatRole.User => "user",
                ChatRole.Assistant => "assistant",
                ChatRole.System => "system",
                _ => "user"
            };
        }

        /// <summary>
        /// Converts an API string representation to a ChatRole enum value.
        /// </summary>
        public static ChatRole FromApiString(string roleString)
        {
            return roleString?.ToLowerInvariant() switch
            {
                "user" => ChatRole.User,
                "assistant" => ChatRole.Assistant,
                "system" => ChatRole.System,
                _ => ChatRole.User
            };
        }
    }
}
