using System;
using System.Threading.Tasks;
using Code.Infrastructure.RhytmSystem;
using UnityEngine;

namespace Code.Infrastructure
{
    public class MetronomeVisualizer : MonoBehaviour
    {
        private Color _normalColor;
        private Color _flashColor;
        private float _flashInterval;
        private Renderer _renderer;
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _normalColor = _renderer.material.color;
            _flashColor = Color.darkRed;
            _renderer.material.color = _flashColor;

            _flashInterval = 60f / BeatTracker.Instance.BPM * 2;
            
        }

        private void OnEnable()
        {
            BeatTracker.Instance.OnBeat += Flash;
        }

        [ContextMenu("FlashMetronome")]
        private async void Flash()
        {
            _renderer.material.color = _flashColor;
            await Task.Delay((int)(_flashInterval * 1000));
            _renderer.material.color = _normalColor;
        }
        
        private void OnDisable()
        {
            BeatTracker.Instance.OnBeat -= Flash;
        }
    }
}