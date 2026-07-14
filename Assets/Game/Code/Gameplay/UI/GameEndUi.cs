using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Gameplay.UI
{
    public class GameEndUi : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            restartButton.onClick.AddListener(Restart);
            quitButton.onClick.AddListener(Quit);
            EventBusService.Instance.Subscribe<GameEndEvent>(Show);
            gameObject.SetActive(false);
        }

        private void Show(GameEndEvent gameEndEvent)
        {
            gameObject.SetActive(true);
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
            EventBusService.Instance.Unsubscribe<GameEndEvent>(Show);
        }
    }
}