using System;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Interface for streaming Claude API implementations.
    /// Enables dependency injection and testability.
    /// </summary>
    public interface IClaudeStreamingApi
    {
        /// <summary>
        /// Creates a streaming chat completion request.
        /// </summary>
        /// <param name="request">The chat completion request.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="onStreamUpdate">Callback invoked for each streaming token.</param>
        /// <param name="onError">Optional callback invoked on errors.</param>
        /// <param name="onComplete">Optional callback invoked when streaming completes.</param>
        void CreateChatCompletionStream(
            ChatCompletionRequest request,
            string apiKey,
            Action<string> onStreamUpdate,
            Action<string> onError = null,
            Action onComplete = null);

        /// <summary>
        /// Cancels the current streaming request.
        /// </summary>
        void CancelStream();
    }
}
