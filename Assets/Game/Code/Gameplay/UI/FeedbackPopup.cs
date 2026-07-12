using Code.Infrastructure.RhytmSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FeedbackPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Animation")]
        [SerializeField] private float duration = 0.8f;
        [SerializeField] private float spawnOffset = 0.7f;
        [SerializeField] private float travelDistance = 0.8f;
        [SerializeField] private float randomAngle = 20f;

        [SerializeField] private float startScale = 0.6f;
        [SerializeField] private float overshootScale = 1.15f;
        [SerializeField] private float endScale = 0.9f;

        private Sequence _sequence;

        public void Play(HitQuality result, Transform player)
        {
            _sequence?.Kill();

            SetupVisual(result);

            // За спиной игрока
            Vector3 direction = -player.right;

            // Небольшой разброс
            direction = Quaternion.Euler(
                0f,
                0f,
                Random.Range(-randomAngle, randomAngle))
                * direction;

            transform.position = player.position + direction * spawnOffset;
            transform.localScale = Vector3.one * startScale;
            canvasGroup.alpha = 1f;

            Vector3 moveOffset = direction * travelDistance;

            _sequence = DOTween.Sequence()
                .SetUpdate(UpdateType.Normal, true);

            // Легкое увеличение
            _sequence.Append(
                transform.DOScale(overshootScale, duration * 0.2f)
                    .SetEase(Ease.OutBack));

            // Возврат масштаба
            _sequence.Join(
                transform.DOScale(endScale, duration * 0.8f)
                    .SetDelay(duration * 0.2f));

            // Полет
            _sequence.Join(
                transform.DOBlendableMoveBy(moveOffset, duration)
                    .SetEase(Ease.OutQuad));

            // Плавное исчезновение
            _sequence.Insert(
                duration * 0.35f,
                canvasGroup.DOFade(0f, duration * 0.65f));

            // _sequence.OnComplete(() =>
            // {
            //     gameObject.SetActive(false);
            // });
        }

        private void SetupVisual(HitQuality result)
        {
            switch (result)
            {
                case HitQuality.Miss:
                    text.text = "MISS";
                    text.color = new Color(0.75f, 0.75f, 0.75f);
                    break;

                case HitQuality.Bad:
                    text.text = "BAD";
                    text.color = new Color(1f, 0.4f, 0.4f);
                    break;

                case HitQuality.Good:
                    text.text = "GOOD";
                    text.color = new Color(1f, 0.9f, 0.25f);
                    break;

                case HitQuality.Perfect:
                    text.text = "PERFECT";
                    text.color = new Color(0.4f, 1f, 0.55f);
                    break;
            }
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }
    }
}