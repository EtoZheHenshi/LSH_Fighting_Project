using System;
using Code.Gameplay;
using Code.Gameplay.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.EntryPoints
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] GameplayPoop gameplayPoop;
        
        PlayerController _player1;
        PlayerController _player2;
        
        private void Awake()
        {
            _player1 = GameObject.FindWithTag("Player1").GetComponent<PlayerController>();
            _player2 = GameObject.FindWithTag("Player2").GetComponent<PlayerController>();
            
            gameplayPoop.Initialize(_player1, _player2);
        }

        private void Start()
        {
            gameplayPoop.StartGameplayLoop();
        }
    }
}