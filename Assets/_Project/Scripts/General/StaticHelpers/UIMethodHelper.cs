using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.General.StaticHelpers
{
    public static class UIMethodHelper
    {
        public static (float firstPostion, float perComponentSpacing, RectTransform prefabRect) CalculateHorizontalGridPosition(int number, float spacing, GameObject uiPrefab)
        {
            float halfInputs = number / 2f;
            int halfInputsCeil = Mathf.CeilToInt(halfInputs);
            float firstSpacing = spacing * halfInputsCeil;
            firstSpacing = (Mathf.Approximately(halfInputs, halfInputsCeil))
                ? firstSpacing - (spacing / 2)
                : firstSpacing;

            RectTransform prefabRect = uiPrefab.GetComponent<RectTransform>();

            float firstPosition = halfInputs * prefabRect.rect.width + firstSpacing;

            firstPosition -= prefabRect.rect.width / 2; //center

            float perInputPosition = prefabRect.rect.width + spacing;

            return (-firstPosition, perInputPosition, prefabRect);
        }
        
        public static void HandleEnableDisableElements(
            List<GameObject> enableElements,
            Action<GameObject> onEnableAction = null,
            List<GameObject> disableElements = null,
            Action<GameObject> onDisableAction = null
        )
        {
            foreach (var enableInput in enableElements)
            {
                onEnableAction?.Invoke(enableInput);
            }

            if (disableElements == null)
                return;

            foreach (var disableInput in disableElements)
            {
                onDisableAction?.Invoke(disableInput);
            }
        }
    }
}