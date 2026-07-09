using System;
using Code.Gameplay.Player.Attacks;
using Code.Gameplay.Player.Blocks;
using Code.Gameplay.Player.Body;
using Code.Gameplay.Player.PlayerStateSystem;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.InputSystem;
using UnityEngine;

namespace Code.Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        private const float Skin = 0.02f;
        
        [SerializeField] private bool isItPlayerTwo;
        [SerializeField] private float ghostMoveSpeed = 8f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask bodyObstacleLayers;
        [SerializeField] private LayerMask ghostObstacleLayers;
        [SerializeField] private SpriteRenderer ghostSpriteRenderer;

        private float _moveSpeed;
        private AttackConfig _attackConfig;
        private BlockConfig _blockConfig;
        private Rigidbody2D _rb;
        private LayerMask _moveObstacleLayers;
        private Transform _currentBody;
        
        private StateMachine _stateMachine;
        
        public GhostState GhostState { get; private set; }
        public AttackState AttackState { get; private set; }
        public ProtectionState ProtectionState { get; private set; }
        
        public StateMachine StateMachine => _stateMachine;
        public Rigidbody2D Rigidbody => _rb;
        public IPlayerInput Input;
        public PlayerController Enemy { get; private set; }
        public float MoveSpeed => _moveSpeed;


        private void Awake()
        {
            _stateMachine = new StateMachine();
            _rb = GetComponent<Rigidbody2D>();
            EventBusService eventBus = EventBusService.Instance;
            _moveSpeed = ghostMoveSpeed;
            _moveObstacleLayers = ghostObstacleLayers;
            
            GhostState = new GhostState(this, _stateMachine, eventBus);
            AttackState = new AttackState(this, _stateMachine, eventBus, _attackConfig, enemyLayer);
            ProtectionState = new ProtectionState(this, _stateMachine, eventBus, _blockConfig);
            
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
        }

        private void Update()
        {
            _stateMachine.Tick();
        }

        private void FixedUpdate()
        {
            Move();
            
            Rotate();
        }

        public void SetBody(BodyConfig bodyConfig, Transform bodyTransform)
        {
            ghostSpriteRenderer.enabled = false;
            _rb.position = bodyTransform.position;
            bodyTransform.parent = transform;
            _currentBody = bodyTransform;
            
            _attackConfig = bodyConfig.AttackConfig;
            _blockConfig = bodyConfig.BlockConfig;
            _moveSpeed = bodyConfig.MoveSpeed;
        }

        public void RemoveBody()
        {
            if (_currentBody != null)
            {
                _currentBody.parent = null;
                _currentBody = null;
            }
            
            _moveSpeed = ghostMoveSpeed;
            ghostSpriteRenderer.enabled = true;
            _attackConfig = null;
            _blockConfig = null;
        }

        public void TakeDamage(float damage)
        {
            if (ProtectionState.CanBlock)
            {
                Debug.Log("Blocked");
            }
            else
            {
                Debug.Log($"Take {damage} damage");
            }
        }

        private void Move()
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(_moveObstacleLayers);
            //filter.useLayerMask = false;

            RaycastHit2D[] hits = new RaycastHit2D[8];
            float distance = _moveSpeed * Time.fixedDeltaTime;

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