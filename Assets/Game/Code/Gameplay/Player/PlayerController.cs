using Code.Gameplay.Player.Body;
using Code.Gameplay.Player.PlayerStateSystem;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.UI.HUD;
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
        [SerializeField] private LayerMask deadBodyLayer;
        [SerializeField] private LayerMask bodyObstacleLayers;
        [SerializeField] private LayerMask ghostObstacleLayers;
        [SerializeField] private SpriteRenderer ghostSpriteRenderer;
        
        [Header("UI")] 
        [SerializeField] private HpUi hpUi;

        private float _hp;
        private float _moveSpeed;
        private Rigidbody2D _rb;
        private LayerMask _moveObstacleLayers;
        private DeadBody _currentBody;
        
        private StateMachine _stateMachine;
        private EventBusService _eventBus;
        
        public GhostState GhostState { get; private set; }
        public AttackState AttackState { get; private set; }
        public ProtectionState ProtectionState { get; private set; }
        public WaitState WaitState { get; private set; }
        
        public StateMachine StateMachine => _stateMachine;
        public Rigidbody2D Rigidbody => _rb;
        public IPlayerInput Input;
        public PlayerController Enemy { get; private set; }
        public bool HaveBody { get; private set; }


        private void Awake()
        {
            _stateMachine = new StateMachine();
            _rb = GetComponent<Rigidbody2D>();
            _eventBus = EventBusService.Instance;
            _moveSpeed = ghostMoveSpeed;
            _moveObstacleLayers = ghostObstacleLayers;
            
            GhostState = new GhostState(this, _stateMachine, _eventBus, deadBodyLayer);
            WaitState = new WaitState(this, _stateMachine, _eventBus);
            
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

        public void SetBody(DeadBody deadBody)
        {
            ghostSpriteRenderer.enabled = false;
            transform.position = deadBody.transform.position;
            transform.rotation = deadBody.transform.rotation;
            deadBody.transform.parent = transform;
            _currentBody = deadBody;

            AttackState = new AttackState(this, _stateMachine, _eventBus, deadBody.AttackConfig, enemyLayer);
            ProtectionState = new ProtectionState(this, _stateMachine, _eventBus, deadBody.BlockConfig);
            _hp = deadBody.Hp;
            _moveSpeed = deadBody.MoveSpeed;
            HaveBody = true;
            
            hpUi.SetHealth(_hp);
        }

        public void RemoveBody()
        {
            if (_currentBody != null)
            {
                _currentBody.transform.parent = null;
                _currentBody.gameObject.layer = 0;
                _currentBody.SpriteRenderer.sortingOrder = -2;
                _currentBody = null;
            }
            
            _moveSpeed = ghostMoveSpeed;
            ghostSpriteRenderer.enabled = true;
            AttackState = null;
            ProtectionState = null;
            HaveBody = false;
            //_stateMachine.ChangeState(GhostState);
            GameplayPoop.Instance.StartGhostTimer();
        }

        public void TakeDamage(float damage)
        {
            if (ProtectionState.CanBlock)
            {
                Debug.Log("Blocked");
            }
            else
            {
                _hp -= damage;
                hpUi.UpdateHealth(_hp);
                Debug.Log($"HP = {_hp}");

                if (_hp <= 0)
                {
                    GameplayPoop.Instance.StopCycle();
                    Enemy.StateMachine.ChangeState(Enemy.WaitState);
                    _stateMachine.ChangeState(GhostState);
                }
            }
        }

        private void Move()
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(_moveObstacleLayers);

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
        
        private void OnDrawGizmos()
        {
            AttackState?.DrawGizmos();
        }
    }
}