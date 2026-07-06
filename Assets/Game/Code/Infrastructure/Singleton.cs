using UnityEngine;

// Базовый класс для любого синглтона
namespace Code.Infrastructure
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
// Ищем объект на сцене, если он ещё не найден
                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
// Если объекта нет на сцене — создаём его программно
                        GameObject go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
        }
    }
}