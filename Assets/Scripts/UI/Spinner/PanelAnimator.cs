using System;
using DG.Tweening;
using UnityEngine;

namespace Scripts.UI.Spinner
{
    public static class PanelAnimator
    {
        private const float Duration = 0.3f;

        public static void Show(GameObject panel, Transform animRoot)
        {
            panel.SetActive(true);

            var cg = GetOrAddCanvasGroup(panel);
            cg.alpha = 0f;
            animRoot.localScale = Vector3.one * 0.8f;

            DOTween.Kill(animRoot);
            animRoot.DOScale(Vector3.one, Duration).SetEase(Ease.OutBack);
            cg.DOFade(1f, Duration).SetEase(Ease.OutQuad);
        }

        public static void Hide(GameObject panel, Transform animRoot, Action onComplete = null)
        {
            var cg = GetOrAddCanvasGroup(panel);

            DOTween.Kill(animRoot);
            animRoot.DOScale(Vector3.one * 0.8f, Duration * 0.6f).SetEase(Ease.InBack);
            cg.DOFade(0f, Duration * 0.6f).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    panel.SetActive(false);
                    onComplete?.Invoke();
                });
        }

        private static CanvasGroup GetOrAddCanvasGroup(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = go.AddComponent<CanvasGroup>();
            return cg;
        }
    }
}
