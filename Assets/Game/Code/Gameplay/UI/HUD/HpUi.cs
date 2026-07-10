using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.UI.HUD
{
    public class HpUi : MonoBehaviour
    {
        [SerializeField] public Image healthBar;

        private float _currentHealth;
        private float _maxHealth;
        
        public void SetHealth(float health)
        {
            _maxHealth = health;
            _currentHealth = _maxHealth;
            healthBar.fillAmount = 1f;
            gameObject.SetActive(true);
        }

        public void UpdateHealth(float health)
        {
            _currentHealth = health;
            if (_currentHealth < 0)
            {
                _currentHealth = 0;
            }
            healthBar.fillAmount = _currentHealth / _maxHealth;
        }
        
        public void HideHealth()
        {
            gameObject.SetActive(false);
        }
    }
}