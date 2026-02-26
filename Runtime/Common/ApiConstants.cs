namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Centralized constants for Claude API configuration and default values.
    /// </summary>
    public static class ApiConstants
    {
        // API Configuration
        public const string API_URL = "https://api.anthropic.com/v1/messages";
        public const string API_VERSION = "2023-06-01";

        // HTTP Headers
        public const string CONTENT_TYPE_JSON = "application/json";
        public const string ACCEPT_SSE = "text/event-stream";
        public const string HEADER_CONTENT_TYPE = "Content-Type";
        public const string HEADER_ACCEPT = "Accept";
        public const string HEADER_API_KEY = "x-api-key";
        public const string HEADER_ANTHROPIC_VERSION = "anthropic-version";

        // SSE Protocol
        public const string SSE_DATA_PREFIX = "data: ";
        public const string SSE_DONE_MARKER = "[DONE]";

        // Default Request Parameters
        public const float DEFAULT_TEMPERATURE = 0.7f;
        public const int DEFAULT_MAX_TOKENS = 1000;
        public const int DEFAULT_MAX_TOKENS_FALLBACK = 500;
        public const float DEFAULT_TOP_P = 1f;

        // Memory Management
        public const int DEFAULT_MAX_HISTORY_MESSAGES = 50;
        public const int DEFAULT_HISTORY_TRIM_COUNT = 30;
        public const int DEFAULT_MAX_UI_MESSAGES = 100;
        public const int DEFAULT_UI_TRIM_COUNT = 70;

        // Logging Prefixes
        public const string LOG_PREFIX_API = "[ClaudeApi]";
        public const string LOG_PREFIX_STREAMING = "[ClaudeStreamingApi]";
        public const string LOG_PREFIX_CHAT = "[ClaudeChat]";
        public const string LOG_PREFIX_CONTROLLER = "[ClaudeChatController]";
    }
}
