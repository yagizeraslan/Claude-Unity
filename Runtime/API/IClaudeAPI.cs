using System.Threading.Tasks;

namespace YagizEraslan.Claude.Unity
{
    public interface IClaudeApi
    {
        Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request);
    }
}