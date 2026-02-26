using System;
using System.Text;
using UnityEngine;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Parses Server-Sent Events (SSE) data from Claude's streaming API.
    /// Follows Single Responsibility Principle by only handling SSE parsing logic.
    /// </summary>
    public class SseParser
    {
        private readonly StringBuilder buffer = new StringBuilder();
        private readonly Action<string> onData;
        private readonly Action onComplete;
        private readonly Action<string> onError;

        /// <summary>
        /// Creates a new SSE parser with the specified callbacks.
        /// </summary>
        /// <param name="onData">Callback invoked when text content is received.</param>
        /// <param name="onComplete">Callback invoked when streaming is complete.</param>
        /// <param name="onError">Optional callback invoked on parsing errors.</param>
        public SseParser(Action<string> onData, Action onComplete, Action<string> onError = null)
        {
            this.onData = onData ?? throw new ArgumentNullException(nameof(onData));
            this.onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
            this.onError = onError;
        }

        /// <summary>
        /// Processes a chunk of SSE data received from the stream.
        /// </summary>
        /// <param name="chunk">The raw chunk of data to process.</param>
        public void ProcessChunk(string chunk)
        {
            if (string.IsNullOrEmpty(chunk)) return;

            buffer.Append(chunk);
            string bufferContent = buffer.ToString();
            string[] lines = bufferContent.Split('\n');

            // If the buffer doesn't end with a newline, the last line is incomplete
            if (!bufferContent.EndsWith("\n"))
            {
                buffer.Clear();
                buffer.Append(lines[lines.Length - 1]);
                for (int i = 0; i < lines.Length - 1; i++)
                    ProcessLine(lines[i]);
            }
            else
            {
                buffer.Clear();
                foreach (string line in lines)
                    ProcessLine(line);
            }
        }

        private void ProcessLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            if (line.StartsWith(ApiConstants.SSE_DATA_PREFIX))
            {
                string jsonData = line.Substring(ApiConstants.SSE_DATA_PREFIX.Length).Trim();

                if (jsonData == ApiConstants.SSE_DONE_MARKER)
                {
                    onComplete?.Invoke();
                    return;
                }

                try
                {
                    // Parse Claude's streaming response format
                    var response = JsonUtility.FromJson<ClaudeStreamChunk>(jsonData);
                    string deltaText = response?.delta?.text;
                    if (!string.IsNullOrEmpty(deltaText))
                    {
                        onData?.Invoke(deltaText);
                    }
                }
                catch (Exception)
                {
                    // Partial JSON is expected during streaming - this is not an error
                }
            }
        }

        /// <summary>
        /// Resets the parser's internal buffer.
        /// </summary>
        public void Reset() => buffer.Clear();

        // Claude's streaming response DTOs
        [Serializable]
        private class ClaudeStreamChunk
        {
            public string type;
            public Delta delta;
        }

        [Serializable]
        private class Delta
        {
            public string type;
            public string text;
        }
    }
}
