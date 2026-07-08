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