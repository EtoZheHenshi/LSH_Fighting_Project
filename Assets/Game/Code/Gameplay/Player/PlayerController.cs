using Code.Gameplay.Player.Body;
using Code.Gameplay.Player.PlayerStateSystem;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.UI;
using Code.Gameplay.UI.HUD;
using Code.Infrastructure.Audio;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Code.Infrastructure.InputSystem;
using Code.Infrastructure.RhytmSystem;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private PlayerIcons playerIcons;
        [SerializeField] private Transform playerAttack;
        [SerializeField] private SpriteRenderer shieldSpriteRenderer;
        
        [Header("UI")] 
        [SerializeField] private HpUi hpUi;
        [SerializeField] private FeedbackPopup feedbackPopup;

        private float _hp;
        private float _moveSpeed;
        private Rigidbody2D _rb;
        private LayerMask _moveObstacleLayers;
        private DeadBody _currentBody;
        private bool _musicSet;
        private Sprite _ghostSprite;

        private Animator _animatorCache;

        private int _countHp;
        
        private StateMachine _stateMachine;
        private EventBusService _eventBus;
        
        public Animator PlayerAnimator { get; private set; }
        public GhostState GhostState { get; private set; }
        public AttackState AttackState { get; private set; }
        public ProtectionState ProtectionState { get; private set; }
        public WaitState WaitState { get; private set; }
        
        public StateMachine StateMachine => _stateMachine;
        public Rigidbody2D Rigidbody => _rb;
        public IPlayerInput Input;
        public PlayerIcons PlayerIcons => playerIcons;
        public Collider2D GhostCollider {get; private set;}
        public PlayerController Enemy { get; private set; }
        public bool HaveBody { get; private set; }
        public float CurrentDamage { get; private set; }
        public FeedbackPopup FeedbackPopup => feedbackPopup;
        public SpriteRenderer SpriteRenderer => playerSpriteRenderer;
        public SpriteRenderer ShieldSpriteRenderer => shieldSpriteRenderer;


        private void Awake()
        {
            _stateMachine = new StateMachine();
            _rb = GetComponent<Rigidbody2D>();
            _eventBus = EventBusService.Instance;
            _moveSpeed = ghostMoveSpeed;
            _moveObstacleLayers = ghostObstacleLayers;
            _ghostSprite = playerSpriteRenderer.sprite;
            
            GhostCollider = GetComponent<Collider2D>();
            PlayerAnimator = GetComponent<Animator>();
            
            playerIcons.Initialize(this.transform);
            playerIcons.SetOffset(GhostCollider);
            
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

            _countHp = 3;
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
            if (!_musicSet && !isItPlayerTwo)
            {
                AudioManager.Instance.SetPlayerMusic(deadBody.BodyMusic);
                _musicSet = true;
            }
            
            GhostCollider.enabled = false;

            playerSpriteRenderer.sprite = deadBody.SpriteRenderer.sprite;
            deadBody.SpriteRenderer.enabled = false;
            
            transform.position = deadBody.transform.position;
            transform.rotation = deadBody.transform.rotation;
            
            deadBody.transform.SetParent(playerAttack);
            deadBody.transform.localPosition = Vector3.zero;
            deadBody.transform.localRotation = Quaternion.identity;
            
            _currentBody = deadBody;
            _currentBody.gameObject.layer = 11;
            //_currentBody.SpriteRenderer.sortingOrder = 0;

            AttackState = new AttackState(this, _stateMachine, _eventBus, deadBody.AttackConfig, enemyLayer);
            ProtectionState = new ProtectionState(this, _stateMachine, _eventBus);
            _hp = deadBody.Hp;
            _moveSpeed = deadBody.MoveSpeed;
            HaveBody = true;
            CurrentDamage = deadBody.AttackConfig.Damage;
            _moveObstacleLayers = bodyObstacleLayers;
            playerIcons.SetOffset(_currentBody.GetComponent<Collider2D>());
            PlayerAnimator.runtimeAnimatorController = deadBody.AnimatorController;
            
            hpUi.SetHealth(_hp);

            if (deadBody.CompareTag("Worm"))
            {
                _animatorCache = PlayerAnimator;
                PlayerAnimator = deadBody.GetComponentInChildren<Animator>();
            }
        }

        public void RemoveBody()
        {
            _countHp--;
            
            if (_countHp <= 0)
            {
                Input.DisableInput();
                Enemy.Input.DisableInput();
                string winPlayer;

                if (isItPlayerTwo)
                {
                    winPlayer = "Player 1 win";
                }
                else
                {
                    winPlayer = "Player 2 win";
                }
                
                _eventBus.Publish(new GameEndEvent(winPlayer));
                return;
            }
            
            if (_currentBody != null)
            {
                _currentBody.transform.parent = null;
                _currentBody.gameObject.layer = 0;
                _currentBody.SpriteRenderer.sortingOrder = -2;
                _currentBody.SpriteRenderer.enabled = true;

                if (_currentBody.CompareTag("Worm"))
                {
                    PlayerAnimator = _animatorCache;
                }

                _currentBody = null;
            }

            PlayerAnimator.runtimeAnimatorController = null;
            _moveSpeed = ghostMoveSpeed;
            playerSpriteRenderer.sprite = _ghostSprite;
            AttackState = null;
            ProtectionState = null;
            HaveBody = false;
            CurrentDamage = 0;
            _moveObstacleLayers = ghostObstacleLayers;

            GhostCollider.enabled = true;
            playerIcons.SetOffset(GhostCollider);
            
            //_stateMachine.ChangeState(GhostState);
            GameplayPoop.Instance.StartGhostTimer();
        }

        public void TakeDamage(float damage)
        {
            _hp -= damage;
            hpUi.UpdateHealth(_hp);
            Debug.Log($"HP = {_hp}");

            if (_hp <= 0)
            {
                GameplayPoop.Instance.StopCycle();
                BeatTracker.Instance.ResetData();
                Enemy.StateMachine.ChangeState(Enemy.WaitState);
                _stateMachine.ChangeState(GhostState);
            }
        }

        public async UniTask GetDamage()
        {
            SpriteRenderer.color = Color.red;
            await UniTask.Delay(100);
            SpriteRenderer.color = Color.white;
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

            //transform.parent.transform.position = _rb.position;
        }

        private void Rotate()
        {
            Vector2 direction = Enemy.Rigidbody.position - _rb.position;

            if (direction.sqrMagnitude <= Mathf.Epsilon)
                return;

            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //
            // playerAttack.rotation = Quaternion.Euler(0f, 0f, angle);
            
            playerAttack.right = direction.normalized;
            
            playerSpriteRenderer.flipX = direction.x < 0f;
            
            // Vector2 direction = Enemy.Rigidbody.position - _rb.position;
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //     
            // playerAttack.Rotate(Vector3.forward, angle);
            //
            // if (playerAttack.rotation.z > 90f || playerAttack.rotation.z < -90f)
            // {
            //     playerSpriteRenderer.flipX = true;
            // }
            // else
            // {
            //     playerSpriteRenderer.flipX = false;
            // }
        }
        
        private void OnDrawGizmos()
        {
            //AttackState?.DrawGizmos();
        }
    }
}