using Code.Infrastructure.RhytmSystem;
using DG.Tweening;
using UnityEngine;

namespace Code.Gameplay.UI
{
    public class BeatPulsationVisualizer : MonoBehaviour
    {
        [SerializeField] private float pulseDuration = 0.2f;

        private Vector3 _defaultScale;

        void Awake()
        {
            _defaultScale = transform.localScale;
            BeatTracker.Instance.OnBeat += Pulse;
        }

        private void Pulse()
        {
            transform.DOKill();

            Sequence sequence = DOTween.Sequence();
            // сильный удар
            sequence.Append(
                transform.DOScale(_defaultScale * 1.20f, pulseDuration * 0.15f).SetEase(Ease.OutQuad));
            // сжатие
            sequence.Append(
                transform.DOScale(_defaultScale * 0.95f, pulseDuration * 0.10f).SetEase(Ease.InQuad));
            // второй удар
            sequence.Append(
                transform.DOScale(_defaultScale * 1.08f, pulseDuration * 0.15f).SetEase(Ease.OutQuad));
            // возвращение в исходное состояние
            sequence.Append(
                transform.DOScale(_defaultScale, pulseDuration * 0.6f).SetEase(Ease.OutQuad));
        }

        private void OnDestroy()
        {
            BeatTracker.Instance.OnBeat -= Pulse;
        }
    }
}