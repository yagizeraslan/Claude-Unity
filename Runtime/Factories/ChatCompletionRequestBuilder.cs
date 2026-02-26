using System.Collections.Generic;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Builder class for creating ChatCompletionRequest instances using fluent API.
    /// </summary>
    public class ChatCompletionRequestBuilder
    {
        private string model;
        private readonly List<ChatMessage> messages = new List<ChatMessage>();
        private float temperature = ApiConstants.DEFAULT_TEMPERATURE;
        private float topP = ApiConstants.DEFAULT_TOP_P;
        private int maxTokens = ApiConstants.DEFAULT_MAX_TOKENS;
        private bool stream = false;
        private string stop = null;

        /// <summary>
        /// Sets the model to use for the request.
        /// </summary>
        public ChatCompletionRequestBuilder WithModel(string modelName)
        {
            this.model = modelName;
            return this;
        }

        /// <summary>
        /// Sets the model to use for the request using ClaudeModel enum.
        /// </summary>
        public ChatCompletionRequestBuilder WithModel(ClaudeModel modelType)
        {
            this.model = modelType.ToModelString();
            return this;
        }

        /// <summary>
        /// Adds a single message to the request.
        /// </summary>
        public ChatCompletionRequestBuilder AddMessage(ChatMessage message)
        {
            if (message != null)
                messages.Add(message);
            return this;
        }

        /// <summary>
        /// Adds a user message to the request.
        /// </summary>
        public ChatCompletionRequestBuilder AddUserMessage(string content)
        {
            messages.Add(ChatMessageFactory.CreateUserMessage(content));
            return this;
        }

        /// <summary>
        /// Adds an assistant message to the request.
        /// </summary>
        public ChatCompletionRequestBuilder AddAssistantMessage(string content)
        {
            messages.Add(ChatMessageFactory.CreateAssistantMessage(content));
            return this;
        }

        /// <summary>
        /// Adds a system message to the request.
        /// </summary>
        public ChatCompletionRequestBuilder AddSystemMessage(string content)
        {
            messages.Add(ChatMessageFactory.CreateSystemMessage(content));
            return this;
        }

        /// <summary>
        /// Sets the messages array from an existing collection.
        /// </summary>
        public ChatCompletionRequestBuilder WithMessages(IEnumerable<ChatMessage> existingMessages)
        {
            messages.Clear();
            if (existingMessages != null)
                messages.AddRange(existingMessages);
            return this;
        }

        /// <summary>
        /// Sets the temperature for response randomness (0.0 to 1.0).
        /// </summary>
        public ChatCompletionRequestBuilder WithTemperature(float temp)
        {
            this.temperature = temp;
            return this;
        }

        /// <summary>
        /// Sets the top_p (nucleus sampling) value (0.0 to 1.0).
        /// </summary>
        public ChatCompletionRequestBuilder WithTopP(float topP)
        {
            this.topP = topP;
            return this;
        }

        /// <summary>
        /// Sets the maximum tokens for the response.
        /// </summary>
        public ChatCompletionRequestBuilder WithMaxTokens(int tokens)
        {
            this.maxTokens = tokens > 0 ? tokens : ApiConstants.DEFAULT_MAX_TOKENS;
            return this;
        }

        /// <summary>
        /// Enables or disables streaming mode.
        /// </summary>
        public ChatCompletionRequestBuilder WithStreaming(bool enabled)
        {
            this.stream = enabled;
            return this;
        }

        /// <summary>
        /// Sets the stop sequence.
        /// </summary>
        public ChatCompletionRequestBuilder WithStop(string stopSequence)
        {
            this.stop = stopSequence;
            return this;
        }

        /// <summary>
        /// Builds and returns the ChatCompletionRequest.
        /// </summary>
        public ChatCompletionRequest Build()
        {
            return new ChatCompletionRequest
            {
                model = this.model,
                messages = this.messages.ToArray(),
                temperature = this.temperature,
                top_p = this.topP,
                max_tokens = this.maxTokens,
                stream = this.stream,
                stop = this.stop
            };
        }

        /// <summary>
        /// Creates a new builder instance.
        /// </summary>
        public static ChatCompletionRequestBuilder Create() => new ChatCompletionRequestBuilder();
    }
}
