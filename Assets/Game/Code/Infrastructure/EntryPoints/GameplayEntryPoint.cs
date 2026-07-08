using System;
using Code.Gameplay;
using Code.Gameplay.Player;
using UnityEngine;

namespace Code.Infrastructure.EntryPoints
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        GameplayLoop _gameplayLoop;
        PlayerController _player1;
        PlayerController _player2;
        
        private void Awake()
        {
            _player1 = GameObject.FindWithTag("Player1").GetComponent<PlayerController>();
            _player2 = GameObject.FindWithTag("Player2").GetComponent<PlayerController>();
            
            _gameplayLoop = new GameplayLoop(_player1, _player2);
        }

        private void Start()
        {
            _gameplayLoop.StartGameplayLoop();
        }
    }
}