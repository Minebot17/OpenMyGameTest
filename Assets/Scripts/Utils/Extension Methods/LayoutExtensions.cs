using System;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Utils.Extension_Methods
{
    public static class LayoutExtensions
    {
        public static IDisposable CreatePlaceholder(this HorizontalOrVerticalLayoutGroup layoutGroup, RectTransform original)
        {
            var placeHolder = new GameObject(
                original.gameObject.name + " (Placeholder)", typeof(RectTransform));
            var placeHolderTransform = placeHolder.GetComponent<RectTransform>();
            
            placeHolderTransform.SetParent(original.parent);
            placeHolderTransform.SetSiblingIndex(original.GetSiblingIndex());
            placeHolderTransform.anchorMin = original.anchorMin;
            placeHolderTransform.anchorMax = original.anchorMax;
            placeHolderTransform.pivot = original.pivot;
            placeHolderTransform.offsetMin = original.offsetMin;
            placeHolderTransform.offsetMax = original.offsetMax;
            placeHolderTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, original.sizeDelta.x);
            placeHolderTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, original.sizeDelta.y);
            placeHolderTransform.anchoredPosition = original.anchoredPosition;

            if (!original.TryGetComponent(out LayoutElement layoutElement))
            {
                layoutElement = original.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.ignoreLayout = true;
            return Disposable.Create(() =>
            {
                Object.Destroy(placeHolder);
                
                if (original.TryGetComponent(out LayoutElement layoutElement))
                {
                    Object.Destroy(layoutElement);
                }
            });
        }
    }
}