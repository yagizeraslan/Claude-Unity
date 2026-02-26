// Sample script to test Claude API Integration for Unity
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Sample MonoBehaviour demonstrating Claude API integration with Unity UI.
    /// </summary>
    public class ClaudeChat : MonoBehaviour
    {
        [Header("Claude Configuration")]
        [SerializeField] private ClaudeSettings config;
        [SerializeField] private ClaudeModel modelType = ClaudeModel.Claude_4_Sonnet;
        [SerializeField] private bool useStreaming = false;

        [Header("UI Elements")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private RectTransform sentMessagePrefab;
        [SerializeField] private RectTransform receivedMessagePrefab;
        [SerializeField] private Transform messageContainer;

        [Header("Memory Management")]
        [SerializeField] private int maxUIMessages = ApiConstants.DEFAULT_MAX_UI_MESSAGES;
        [SerializeField] private int uiTrimCount = ApiConstants.DEFAULT_UI_TRIM_COUNT;

        private ClaudeChatController controller;
        private TMP_Text activeStreamingText;
        private readonly List<GameObject> uiMessages = new List<GameObject>();

        private void Start()
        {
            sendButton.onClick.AddListener(SendMessage);
            inputField.onSubmit.AddListener(OnInputFieldSubmit);

            // Initialize controller once
            InitializeController();
        }

        private void OnDestroy()
        {
            // Clean up event listeners to prevent memory leaks
            if (sendButton != null)
            {
                sendButton.onClick.RemoveListener(SendMessage);
            }

            if (inputField != null)
            {
                inputField.onSubmit.RemoveListener(OnInputFieldSubmit);
            }

            // Dispose controller to free up resources
            controller?.Dispose();

            // Clear UI messages list (GameObjects will be destroyed with the scene)
            uiMessages.Clear();
        }

        private void OnInputFieldSubmit(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                SendMessage();
            }
        }

        private void InitializeController()
        {
            var api = new ClaudeApi(config);
            var streamingApi = new ClaudeStreamingApi();

            controller = new ClaudeChatController(
                api,
                streamingApi,
                config,
                GetSelectedModelName(),
                AddFullMessageToUI,
                AppendStreamingCharacter,
                OnError,
                useStreaming
            );
        }

        private string GetSelectedModelName()
        {
            return modelType.ToModelString();
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(inputField.text)) return;

            // Recreate controller only if model type or streaming mode changed
            if (controller == null || ShouldRecreateController())
            {
                InitializeController();
            }

            controller.SendUserMessage(inputField.text);
            inputField.text = "";
            inputField.ActivateInputField();
        }

        private bool ShouldRecreateController()
        {
            // Check if current controller settings match current UI settings
            // This is a simplified check - in a real implementation you might store the previous settings
            return false; // For now, reuse the same controller
        }

        private void AddFullMessageToUI(ChatMessage message, bool isUser)
        {
            var prefab = isUser ? sentMessagePrefab : receivedMessagePrefab;
            var instance = Instantiate(prefab, messageContainer);
            var textComponent = instance.GetComponentInChildren<TMP_Text>();

            // Track UI message for memory management
            uiMessages.Add(instance.gameObject);
            TrimUIMessagesIfNeeded();

            if (textComponent != null)
            {
                if (!isUser && useStreaming)
                {
                    textComponent.text = "";
                    activeStreamingText = textComponent;
                }
                else
                {
                    textComponent.text = message.content;
                    activeStreamingText = null;
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)messageContainer);
        }

        private void AppendStreamingCharacter(string partialContent)
        {
            if (activeStreamingText != null)
            {
                activeStreamingText.text = partialContent;
            }
            else
            {
                Debug.LogWarning($"{ApiConstants.LOG_PREFIX_CHAT} activeStreamingText is null — cannot update streaming content.");
            }
        }

        private void OnError(string errorMessage)
        {
            Debug.LogError($"{ApiConstants.LOG_PREFIX_CHAT} {errorMessage}");
        }

        private void TrimUIMessagesIfNeeded()
        {
            if (HistoryTrimmer.TrimGameObjectsIfNeeded(uiMessages, maxUIMessages, uiTrimCount))
            {
                Debug.Log($"{ApiConstants.LOG_PREFIX_CHAT} Trimmed UI messages. Current count: {uiMessages.Count}");
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)messageContainer);
            }
        }
    }
}
