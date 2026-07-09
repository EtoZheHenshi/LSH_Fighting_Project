using UnityEngine;

namespace Code.Gameplay.Player.Body
{
    public class DeadBody : MonoBehaviour
    {
        [SerializeField] private BodyConfig bodyConfig;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");

        public void SetOutline(bool activate)
        {
            spriteRenderer.material.SetFloat(OutlineEnabled, activate ? 1f : 0f);
        }
    }
}