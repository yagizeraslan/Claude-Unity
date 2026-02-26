using System.Threading.Tasks;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Interface for Claude API implementations.
    /// Enables dependency injection and testability.
    /// </summary>
    public interface IClaudeApi
    {
        /// <summary>
        /// Gets the API key used for authentication.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Creates a chat completion request and returns the response.
        /// Returns null on failure (legacy behavior).
        /// </summary>
        Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request);

        /// <summary>
        /// Creates a chat completion request with explicit error handling.
        /// Returns a Result wrapper that indicates success or failure.
        /// </summary>
        Task<Result<ChatCompletionResponse>> CreateChatCompletionAsync(ChatCompletionRequest request);
    }
}
