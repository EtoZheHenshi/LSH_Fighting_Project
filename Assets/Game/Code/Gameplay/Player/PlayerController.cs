using Code.Gameplay.Player.PlayerStateSystem;
using Code.Gameplay.Player.PlayerStateSystem.Attacks;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private bool isItPlayerTwo;
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private CircleCollider2D legsCollider;
        
        [Header("AttackConfigs")] [SerializeField]
        private AttackConfig groundAttack;
    
        public Rigidbody2D Body
        {
            get { return body; }
            private set { body = value; }
        }
        
        public CircleCollider2D LegsCollider
        {
            get { return legsCollider; }
            private set { legsCollider = value; }
        }

        public IPlayerInput Input;

        public float MoveSpeed => moveSpeed;
        public float JumpForce => jumpForce;

        /// <summary>Стоит ли игрок на земле. Обновляется через события коллизий.</summary>
        public bool IsGrounded { get; private set; }

        // Все состояния создаются ОДИН РАЗ в Awake и переиспользуются.
        // Если писать new IdleState(...) при каждом переходе, мы будем
        // создавать мусор для сборщика (GC) десятки раз в секунду.
        public IdleState IdleState { get; private set; }
        public MoveState MoveState { get; private set; }
        public JumpState JumpState { get; private set; }
        public FallState FallState { get; private set; }
        
        public CrouchState CrouchState { get; private set; }
        public GroundAttackState GroundAttackState { get; private set; }

        private StateMachine _stateMachine;


        private void Awake()
        {
            _stateMachine = new StateMachine();

            IdleState = new IdleState(this, _stateMachine);
            MoveState = new MoveState(this, _stateMachine);
            JumpState = new JumpState(this, _stateMachine);
            FallState = new FallState(this, _stateMachine);
            CrouchState = new CrouchState(this, _stateMachine);
            
            GroundAttackState = new GroundAttackState(this, _stateMachine, groundAttack, enemyLayer);
            Body = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (isItPlayerTwo)
            {
                Input = InputService.Instance.Player2;
            }
            else
            {
                Input = InputService.Instance.Player1;
            }

            // У машины всегда должно быть начальное состояние.
            _stateMachine.ChangeState(IdleState);
        }

        private void Update()
        {
            IsGrounded = LegsCollider.IsTouchingLayers(groundLayer);
            _stateMachine.Tick();
            Debug.Log($"IsGrounded = {IsGrounded}");
        }

        public void TakeDamage(float damage)
        {
            Debug.Log($"Take {damage} damage");
        }

        // --- Упрощённое определение земли (для учебного примера) ---
        // Любое касание коллайдера считаем землёй. В реальном проекте
        // здесь была бы проверка слоя (LayerMask) и направления контакта,
        // иначе "землёй" станет и стена, и потолок.
    }
}