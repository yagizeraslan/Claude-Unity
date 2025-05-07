#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace YagizEraslan.Claude.Unity
{
    public class ClaudeSettingsEditor : EditorWindow
    {
        private string apiKey;

        [MenuItem("Claude/Settings")]
        public static void ShowWindow()
        {
            GetWindow<ClaudeSettingsEditor>("Claude Settings");
        }

        private void OnGUI()
        {
            apiKey = EditorPrefs.GetString("ClaudeAPIKey", "");

            GUILayout.Label("Editor-only Dev API Key", EditorStyles.boldLabel);
            apiKey = EditorGUILayout.TextField("API Key", apiKey);

            if (GUILayout.Button("Save"))
            {
                EditorPrefs.SetString("ClaudeAPIKey", apiKey);
                Debug.Log("Claude API key saved for editor testing.");
            }
        }
    }
}
#endif