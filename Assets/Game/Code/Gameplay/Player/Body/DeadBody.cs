using Code.Gameplay.Player.Attacks;
using Code.Infrastructure.Audio;
using UnityEditor.Animations;
using UnityEngine;

namespace Code.Gameplay.Player.Body
{
    public class DeadBody : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float hp;
        [SerializeField] private float moveSpeed;
        [SerializeField] private AttackConfig attackConfig;
        [SerializeField] private SoundData bodyMusic;
        [SerializeField] private RuntimeAnimatorController bodyAnimatorController;
        
        public float Hp => hp;
        public float MoveSpeed => moveSpeed;
        public AttackConfig AttackConfig => attackConfig;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public SoundData BodyMusic => bodyMusic;
        public RuntimeAnimatorController AnimatorController => bodyAnimatorController;

        private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");

        public void SetOutline(bool activate)
        {
            spriteRenderer.material.SetFloat(OutlineEnabled, activate ? 1f : 0f);
        }
    }
}