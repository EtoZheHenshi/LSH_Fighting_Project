using System;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.UI.HUD
{
    public class TimerUi : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeText;
        
        private float _switchTime;
        private bool _isActive;

        private void Update()
        {
            if (_isActive)
            {
                _switchTime -= Time.deltaTime;
                float time = Mathf.Ceil(_switchTime);
                time = time > 0 ? time : 0;
                timeText.text = time.ToString();
            }
        }

        public void StartTimer(float time)
        {
            _switchTime = time;
            _isActive = true;
        }

        public void StopTimer()
        {
            _isActive = false;
        }
    }
}