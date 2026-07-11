using DG.Tweening;
using UnityEngine;

namespace Code.Gameplay.UI.HUD
{
    public class IconSwapAnimation : MonoBehaviour
    {
        [Header("Icons")]
        [SerializeField] private RectTransform leftIcon;
        [SerializeField] private RectTransform rightIcon;

        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup leftCanvasGroup;
        [SerializeField] private CanvasGroup rightCanvasGroup;

        [Header("Animation")]
        [SerializeField] private float totalDuration = 1.5f;
        [SerializeField] private float appearDuration = 0.3f;
        [SerializeField] private float disappearDuration = 0.3f;
        [SerializeField] private float arcHeight = 50f;
        [SerializeField] private float startScale = 0.8f;
        [SerializeField] private float endScale = 0.9f;

        private Sequence _sequence;

        public void Play()
        {
            _sequence?.Kill();

            float swapDuration = totalDuration - appearDuration - disappearDuration;
            float halfSwapDuration = swapDuration * 0.5f;

            Vector2 leftStart = leftIcon.anchoredPosition;
            Vector2 rightStart = rightIcon.anchoredPosition;

            Vector2 center = (leftStart + rightStart) * 0.5f;

            Vector2 leftMid = center + Vector2.up * arcHeight;
            Vector2 rightMid = center + Vector2.down * arcHeight;

            leftIcon.localScale = Vector3.one * startScale;
            rightIcon.localScale = Vector3.one * startScale;

            leftCanvasGroup.alpha = 0f;
            rightCanvasGroup.alpha = 0f;

            _sequence = DOTween.Sequence()
                .SetUpdate(UpdateType.Normal, true);

            // Появление
            _sequence.Append(leftCanvasGroup.DOFade(1f, appearDuration));
            _sequence.Join(rightCanvasGroup.DOFade(1f, appearDuration));

            _sequence.Join(
                leftIcon.DOScale(1f, appearDuration)
                    .SetEase(Ease.OutBack));

            _sequence.Join(
                rightIcon.DOScale(1f, appearDuration)
                    .SetEase(Ease.OutBack));

            // Первая половина обмена
            _sequence.Append(
                leftIcon.DOAnchorPos(leftMid, halfSwapDuration)
                    .SetEase(Ease.InOutSine));

            _sequence.Join(
                rightIcon.DOAnchorPos(rightMid, halfSwapDuration)
                    .SetEase(Ease.InOutSine));

            // Вторая половина обмена
            _sequence.Append(
                leftIcon.DOAnchorPos(rightStart, halfSwapDuration)
                    .SetEase(Ease.InOutSine));

            _sequence.Join(
                rightIcon.DOAnchorPos(leftStart, halfSwapDuration)
                    .SetEase(Ease.InOutSine));

            // Исчезновение
            _sequence.Append(leftCanvasGroup.DOFade(0f, disappearDuration));
            _sequence.Join(rightCanvasGroup.DOFade(0f, disappearDuration));

            _sequence.Join(leftIcon.DOScale(endScale, disappearDuration));
            _sequence.Join(rightIcon.DOScale(endScale, disappearDuration));
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}