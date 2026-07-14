using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Code.Gameplay.Player;
using Code.Gameplay.Player.Body;
using Code.Gameplay.UI.HUD;
using Code.Infrastructure;
using Code.Infrastructure.Audio;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Code.Infrastructure.RhytmSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay
{
    public class GameplayPoop : Singleton<GameplayPoop>
    {
        [SerializeField] private Transform deadBodyRoot;
        [SerializeField] private TimerUi timerUI;
        
        [SerializeField] private float switchTime = 10f;
        [SerializeField] private float ghostTime = 5f;

        [SerializeField] private IconSwapAnimation iconSwapAnimation;

        [SerializeField] private SoundData switchSound;
        
        [SerializeField] private BeatOutlineAnimation outlineAnimation;
        
        private PlayerController _player1;
        private PlayerController _player2;
        
        private EventBusService _eventBus;
        private CancellationTokenSource _cts;
        private bool _ghostState;
        private float _currentGhostTimeLeft;
        private bool _playerOneAttack;
        private List<DeadBody> _deadBodies;
        
        private bool _musicIsPlaying;

        public void Initialize(PlayerController player1, PlayerController player2)
        {
            _player1 = player1;
            _player2 = player2;
            _eventBus = EventBusService.Instance;
            _cts = new CancellationTokenSource();
            _deadBodies = deadBodyRoot.GetComponentsInChildren<DeadBody>().ToList();
            BeatTracker.Instance.OnBeat += outlineAnimation.OnBeat;
        }

        private void Update()
        {
            GhostCycle();
        }

        public void StartGameplayLoop()
        {
            _player1.StateMachine.ChangeState(_player1.GhostState);
            _player2.StateMachine.ChangeState(_player2.GhostState);
            _playerOneAttack = true;
            
            StartGhostTimer();
        }

        public void StartGhostTimer()
        {
            _currentGhostTimeLeft = ghostTime;
            _ghostState = true;
        }

        public void RemoveDeadBodies(DeadBody deadBody)
        {
            _deadBodies.Remove(deadBody);
        }

        public void StopCycle()
        {
            _cts.Cancel();
            _cts.Dispose();

            _cts = new CancellationTokenSource();
        }

        public void SwitchPlayerRoles()
        {
            timerUI.StopTimer();
            StopCycle();

            outlineAnimation.PrepareRoleSwap();

            StartTimerSwitch().Forget();
        }

        // private async UniTask SwitchDelay()
        // {
        //     _player1.Input.DisableInput();
        //     _player2.Input.DisableInput();
        //     AudioManager.Instance.PausePlayerMusic();
        //     //Time.timeScale = 0f;
        //     iconSwapAnimation.Play();
        //     
        //     AudioManager.Instance.PlaySound(switchSound, switchDelay);
        //     
        //     await UniTask.WaitForSeconds(switchDelay, true);
        //     
        //     //Time.timeScale = 1f;
        //     _player1.Input.EnableInput();
        //     _player2.Input.EnableInput();
        //     
        //     StartTimerSwitch().Forget();
        // }

        private void GhostCycle()
        {
            if (_ghostState)
            {
                if (_currentGhostTimeLeft > 0f)
                {
                    if (_player1.HaveBody && _player2.HaveBody)
                    {
                        StartTimerSwitch().Forget();
                        _ghostState = false;
                        return;
                    }
                    
                    _currentGhostTimeLeft -= Time.deltaTime;
                    
                    return;
                }
                
                FindDeadBody();
                _ghostState = false;
            }
        }

        private async UniTask StartTimerSwitch()
        {
            CancellationToken token = _cts.Token;

            try
            {
                if (_playerOneAttack)
                {
                    _player1.StateMachine.ChangeState(_player1.AttackState);
                    _player2.StateMachine.ChangeState(_player2.ProtectionState);
                }
                else
                {
                    _player1.StateMachine.ChangeState(_player1.ProtectionState);
                    _player2.StateMachine.ChangeState(_player2.AttackState);
                }

                _playerOneAttack = !_playerOneAttack;

                _player1.Input.DisableInput();
                _player2.Input.DisableInput();

                iconSwapAnimation.Play();

                if (!_musicIsPlaying)
                {
                    AudioManager.Instance.PlayPlayerMusic();
                    _musicIsPlaying = true;
                }

                // BeatOutlineAnimation сама ждёт следующий OnBeat,
                // запускает анимацию и завершает этот UniTask после неё.
                await outlineAnimation
                    .WaitForCompletion()
                    .AttachExternalCancellation(token);

                _player1.Input.EnableInput();
                _player2.Input.EnableInput();

                timerUI.StartTimer(switchTime);

                await UniTask.Delay(
                    TimeSpan.FromSeconds(switchTime),
                    cancellationToken: token);

                timerUI.StopTimer();

                SwitchPlayerRoles();

                Debug.Log("Switch");
            }
            catch (OperationCanceledException)
            {
                // Старый цикл был отменён.
            }
        }
        
        private void FindDeadBody()
        {
            if (!_player1.HaveBody)
            {
                GetRandomDeadBody(_player1);
            }

            if (!_player2.HaveBody)
            {
                GetRandomDeadBody(_player2);
            }
            
            StartTimerSwitch().Forget();
        }

        private void GetRandomDeadBody(PlayerController player)
        {
            DeadBody deadBody = _deadBodies[Random.Range(0, _deadBodies.Count)];
            _deadBodies.Remove(deadBody);
            player.SetBody(deadBody);
        }

        private void OnDestroy()
        {
            BeatTracker.Instance.OnBeat -= outlineAnimation.OnBeat;
        }
    }
}