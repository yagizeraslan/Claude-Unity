using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// Utility class for trimming lists to manage memory usage.
    /// Provides reusable trimming logic for chat history and UI elements.
    /// </summary>
    public static class HistoryTrimmer
    {
        /// <summary>
        /// Trims a list if it exceeds the maximum count by removing oldest items.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to trim.</param>
        /// <param name="maxCount">The maximum allowed count (0 = unlimited).</param>
        /// <param name="trimToCount">The target count after trimming.</param>
        /// <returns>True if the list was trimmed, false otherwise.</returns>
        public static bool TrimIfNeeded<T>(List<T> list, int maxCount, int trimToCount)
        {
            if (list == null || maxCount <= 0 || list.Count <= maxCount)
                return false;

            if (trimToCount < 0) trimToCount = 0;
            if (trimToCount > maxCount) trimToCount = maxCount;

            int itemsToRemove = list.Count - trimToCount;
            if (itemsToRemove > 0)
            {
                list.RemoveRange(0, itemsToRemove);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Trims a list of GameObjects if it exceeds the maximum count,
        /// destroying the removed GameObjects to prevent memory leaks.
        /// </summary>
        /// <param name="gameObjects">The list of GameObjects to trim.</param>
        /// <param name="maxCount">The maximum allowed count (0 = unlimited).</param>
        /// <param name="trimToCount">The target count after trimming.</param>
        /// <returns>True if the list was trimmed, false otherwise.</returns>
        public static bool TrimGameObjectsIfNeeded(List<GameObject> gameObjects, int maxCount, int trimToCount)
        {
            if (gameObjects == null || maxCount <= 0 || gameObjects.Count <= maxCount)
                return false;

            if (trimToCount < 0) trimToCount = 0;
            if (trimToCount > maxCount) trimToCount = maxCount;

            int itemsToRemove = gameObjects.Count - trimToCount;
            if (itemsToRemove > 0)
            {
                for (int i = 0; i < itemsToRemove; i++)
                {
                    if (gameObjects[i] != null)
                        Object.Destroy(gameObjects[i]);
                }
                gameObjects.RemoveRange(0, itemsToRemove);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates how many items should be removed to trim a list.
        /// </summary>
        /// <param name="currentCount">The current item count.</param>
        /// <param name="maxCount">The maximum allowed count.</param>
        /// <param name="trimToCount">The target count after trimming.</param>
        /// <returns>The number of items to remove, or 0 if no trimming is needed.</returns>
        public static int CalculateItemsToRemove(int currentCount, int maxCount, int trimToCount)
        {
            if (maxCount <= 0 || currentCount <= maxCount)
                return 0;

            if (trimToCount < 0) trimToCount = 0;
            if (trimToCount > maxCount) trimToCount = maxCount;

            return currentCount - trimToCount;
        }
    }
}
