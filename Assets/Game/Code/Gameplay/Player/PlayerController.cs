using System;
using Code.Gameplay.Player.Attacks;
using Code.Gameplay.Player.PlayerStateSystem;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.InputSystem;
using UnityEngine;

namespace Code.Gameplay.Player
{
    /// <summary>
    /// "Контекст" паттерна State — владелец всех данных и компонентов,
    /// которые нужны состояниям для работы.
    /// 
    /// Разделение ответственности:
    ///   PlayerController — ВЛАДЕЕТ данными (Rigidbody, ввод, настройки)
    ///                      и прокидывает Unity-события (Update, коллизии).
    ///   Состояния        — РЕШАЮТ, что с этими данными делать.
    ///   StateMachine     — СЛЕДИТ, какое состояние сейчас активно.
    /// 
    /// Это единственный MonoBehaviour во всей системе.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        private const float Skin = 0.02f;
        
        [SerializeField] private bool isItPlayerTwo;
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask bodyObstacleLayers;
        [SerializeField] private LayerMask ghostObstacleLayers; 
        [SerializeField] private AttackConfig attackConfig;
    
        private Rigidbody2D _rb;
        private StateMachine _stateMachine;
        
        public GhostState GhostState { get; private set; }
        public AttackState AttackState { get; private set; }
        public ProtectionState ProtectionState { get; private set; }
        
        public StateMachine StateMachine => _stateMachine;
        public Rigidbody2D Rigidbody => _rb;
        public IPlayerInput Input;
        public PlayerController Enemy { get; private set; }
        public float MoveSpeed => moveSpeed;


        private void Awake()
        {
            _stateMachine = new StateMachine();
            _rb = GetComponent<Rigidbody2D>();
            EventBusService eventBus = EventBusService.Instance;
            
            GhostState = new GhostState(this, _stateMachine, eventBus);
            AttackState = new AttackState(this, _stateMachine, eventBus, attackConfig, enemyLayer);
            ProtectionState = new ProtectionState(this, _stateMachine, eventBus);
        }

        private void Start()
        {
            if (isItPlayerTwo)
            {
                Input = InputService.Instance.Player2;
                Enemy = GameObject.FindWithTag("Player1").GetComponent<PlayerController>();
            }
            else
            {
                Input = InputService.Instance.Player1;
                Enemy = GameObject.FindWithTag("Player2").GetComponent<PlayerController>();
            }

            // У машины всегда должно быть начальное состояние.
            //_stateMachine.ChangeState(GhostState);
        }

        private void Update()
        {
            _stateMachine.Tick();
        }

        private void FixedUpdate()
        {
            // if (Input.Move.sqrMagnitude > 0.01f)
            // {
            //     Move();
            // }
            
            Move();
            
            Rotate();
        }

        public void TakeDamage(float damage)
        {
            Debug.Log($"Take {damage} damage");
        }

        private void Move()
        {
            ContactFilter2D filter = new ContactFilter2D();
            //filter.SetLayerMask(bodyObstacleLayers);
            filter.useLayerMask = false;

            RaycastHit2D[] hits = new RaycastHit2D[8];
            float distance = moveSpeed * Time.fixedDeltaTime;

            int count = _rb.Cast(
                Input.Move,
                filter,
                hits,
                distance);

            if (count == 0)
            {
                _rb.MovePosition(_rb.position + Input.Move * distance);
            }
            else
            {
                _rb.MovePosition(_rb.position + Input.Move * (hits[0].distance - Skin));
            }
        }

        private void Rotate()
        {
            Vector2 direction = Enemy.Rigidbody.position - _rb.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
            _rb.MoveRotation(angle);
        }
    }
}