using Code.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Player.Blocks
{
    [RequireComponent(typeof(Collider2DGizmo))]
    public sealed class BlockConfig : MonoBehaviour
    {
        [SerializeField] private float blockSize;

        private Collider2DGizmo _gizmo;

        private void Awake()
        {
            _gizmo = GetComponent<Collider2DGizmo>();
        }

        public float BlockSize => blockSize;

        public void VisualizeAttack(float duration)
        {
            _gizmo.SwitchGizmosColor(Color.dodgerBlue, duration).Forget();
        }
    }
}