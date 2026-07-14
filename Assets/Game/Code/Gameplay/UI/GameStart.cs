using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.UI
{
    public class GameStart : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        public event Action OnStart;
        
        private void Awake()
        {
            startButton.onClick.AddListener(StartAction);
        }

        private void StartAction()
        {
            gameObject.SetActive(false);
            OnStart?.Invoke();
        }

        private void OnDestroy()
        {
            startButton.onClick.RemoveAllListeners();
        }
    }
}