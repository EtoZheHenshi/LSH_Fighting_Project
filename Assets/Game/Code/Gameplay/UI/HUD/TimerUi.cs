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
                timeText.text = Mathf.Ceil(_switchTime).ToString();
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