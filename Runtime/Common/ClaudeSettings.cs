using UnityEngine;

namespace YagizEraslan.Claude.Unity
{
    [CreateAssetMenu(fileName = "ClaudeSettings", menuName = "Claude/Settings", order = 1)]
    public class ClaudeSettings : ScriptableObject
    {
        [Tooltip("Your Claude API Key (used at runtime)")]
        public string apiKey;
        
        [Header("Memory Management")]
        [Tooltip("Maximum number of messages to keep in conversation history (0 = unlimited)")]
        [Range(0, 200)]
        public int maxHistoryMessages = 50;
        
        [Tooltip("Number of oldest messages to remove when history limit is reached")]
        [Range(5, 50)]
        public int historyTrimCount = 10;
    }
}