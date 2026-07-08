using System;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.EntryPoints
{
    public class BootstrapEntryPoint : MonoBehaviour
    {
        [SerializeField] private InputService inputSystemPrefab;
        [SerializeField] private EventBusService eventBusPrefab;

        private void Awake()
        {
            Instantiate(inputSystemPrefab);
            Instantiate(eventBusPrefab);

            SceneManager.LoadScene(1);
        }
    }
}