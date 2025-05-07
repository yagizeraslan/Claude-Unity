namespace YagizEraslan.Claude.Unity
{
    public enum ClaudeModel
    {
        Claude_3_Opus,
        Claude_3_Sonnet,
        Claude_3_Haiku
    }

    public static class ClaudeModelExtensions
    {
        public static string ToModelString(this ClaudeModel model)
        {
            return model switch
            {
                ClaudeModel.Claude_3_Opus => "claude-3-opus-20240229",
                ClaudeModel.Claude_3_Sonnet => "claude-3-sonnet-20240229",
                ClaudeModel.Claude_3_Haiku => "claude-3-haiku-20240307",
                _ => "claude-3-sonnet-20240229"
            };
        }
    }
}