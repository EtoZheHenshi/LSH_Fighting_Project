using System;
using Code.Gameplay.Player.Attacks;
using Code.Gameplay.Player.Blocks;
using UnityEngine;

namespace Code.Gameplay.Player.Body
{
    public class DeadBody : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float hp;
        [SerializeField] private float moveSpeed;
        [SerializeField] private AttackConfig attackConfig;
        [SerializeField] private BlockConfig blockConfig;
        
        public float Hp => hp;
        public float MoveSpeed => moveSpeed;
        public AttackConfig AttackConfig => attackConfig;
        public BlockConfig BlockConfig => blockConfig;

        private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");

        public void SetOutline(bool activate)
        {
            spriteRenderer.material.SetFloat(OutlineEnabled, activate ? 1f : 0f);
        }
    }
}