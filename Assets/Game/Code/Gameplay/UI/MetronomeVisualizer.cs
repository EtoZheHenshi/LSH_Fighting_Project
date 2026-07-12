using Code.Infrastructure.RhytmSystem;
using DG.Tweening;
using UnityEngine;

namespace Code.Gameplay.UI
{
    public class MetronomeVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject leftBracketPrefab;
        [SerializeField] private GameObject rightBracketPrefab;

        [SerializeField] private Transform center;

        [SerializeField] private float startOffset = 5f;
        [SerializeField] private int spawnOffsetMs = 1000;
        private float _destroyTimeSec;
        private float _beatDurationMs;
        private int _currentBeat;

        private float CenterX => center.localPosition.x;

        private void Start()
        {
            _destroyTimeSec = BeatTracker.Instance.HitRadiusMs / 1000f;
            _beatDurationMs = 60f / BeatTracker.Instance.BPM * 1000f;
            _currentBeat = (int)(spawnOffsetMs / _beatDurationMs);
        }

        private void Update()
        {
            float musicTime = Store.Instance.MusicPositionMs;
            int currentBeat = (int)((musicTime + spawnOffsetMs) / _beatDurationMs);
            if (_currentBeat != currentBeat)
            {
                _currentBeat = currentBeat;
                SpawnPair();
            }
        }

        private void SpawnPair()
        {
            GameObject left = Instantiate(leftBracketPrefab, transform);
            GameObject right = Instantiate(rightBracketPrefab, transform);

            left.transform.localPosition =
                new Vector3(CenterX - startOffset, 0f, 0f);

            right.transform.localPosition =
                new Vector3(CenterX + startOffset, 0f, 0f);

            left.transform
                .DOLocalMoveX(CenterX, spawnOffsetMs / 1000f)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(left, _destroyTimeSec));

            right.transform
                .DOLocalMoveX(CenterX, spawnOffsetMs / 1000f)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(right, _destroyTimeSec));
        }
    }
}