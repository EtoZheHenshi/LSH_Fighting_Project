using Code.Gameplay.Player.Attacks;
using Code.Gameplay.Player.Blocks;
using UnityEngine;

namespace Code.Gameplay.Player.Body
{
    [CreateAssetMenu(fileName = "BodyConfig", menuName = "Gameplay/BodyConfig")]
    public class BodyConfig : ScriptableObject
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private AttackConfig attackConfig;
        [SerializeField] private BlockConfig blockConfig;
        
        public float MoveSpeed => moveSpeed;
        public AttackConfig AttackConfig => attackConfig;
        public BlockConfig BlockConfig => blockConfig;
    }
}