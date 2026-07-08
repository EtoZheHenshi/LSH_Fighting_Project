using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure
{
    [ExecuteAlways]
    [RequireComponent(typeof(Collider2D))]
    public class Collider2DGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.yellow;

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        public async UniTask SwitchGizmosColor(Color switchColor, float duration)
        {
            Color oldColor = Color.yellow;
            color = switchColor;
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            color = oldColor;
        }

        private void OnDrawGizmos()
        {
            if (_collider == null)
                _collider = GetComponent<Collider2D>();

            Gizmos.color = color;

            switch (_collider)
            {
                case BoxCollider2D box:
                    DrawBox(box);
                    break;

                case CircleCollider2D circle:
                    DrawCircle(circle);
                    break;
            }
        }

        private static void DrawBox(BoxCollider2D box)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;

            Gizmos.matrix = box.transform.localToWorldMatrix;

            Gizmos.DrawWireCube(box.offset, box.size);

            Gizmos.matrix = oldMatrix;
        }

        private static void DrawCircle(CircleCollider2D circle)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;

            Gizmos.matrix = circle.transform.localToWorldMatrix;

            Gizmos.DrawWireSphere(circle.offset, circle.radius);

            Gizmos.matrix = oldMatrix;
        }
    }
}