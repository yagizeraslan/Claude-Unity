using UnityEngine;

namespace YagizEraslan.Claude.Unity
{
    [CreateAssetMenu(fileName = "ClaudeSettings", menuName = "Claude/Settings", order = 1)]
    public class ClaudeSettings : ScriptableObject
    {
        [Tooltip("Your Claude API Key (used at runtime)")]
        public string apiKey;
    }
}