namespace YagizEraslan.Claude.Unity
{
    public enum ClaudeModel
    {
        Claude_3_Haiku,
        Claude_3_Sonnet,
        Claude_3_Opus,
        Claude_3_5_Haiku,
        Claude_3_5_Sonnet,
        Claude_3_7_Sonnet
    }

    public static class ClaudeModelExtensions
    {
        public static string ToModelString(this ClaudeModel model)
        {
            return model switch
            {
                ClaudeModel.Claude_3_Haiku      => "claude-3-haiku-20240307",
                ClaudeModel.Claude_3_Sonnet     => "claude-3-sonnet-20240229",
                ClaudeModel.Claude_3_Opus       => "claude-3-opus-20240229",
                ClaudeModel.Claude_3_5_Haiku    => "claude-3-5-haiku-20241022",
                ClaudeModel.Claude_3_5_Sonnet   => "claude-3-5-sonnet-20241022",
                ClaudeModel.Claude_3_7_Sonnet   => "claude-3-7-sonnet-20250219",
                _ => "claude-3-sonnet-20240229"
            };
        }
    }
}