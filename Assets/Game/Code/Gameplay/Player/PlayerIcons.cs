using System;
using UnityEngine;

namespace Code.Gameplay.Player
{
    public class PlayerIcons : MonoBehaviour
    {
        [SerializeField] private Transform icons;
        [SerializeField] private SpriteRenderer roleIcon;
        [SerializeField] private float baseOffset;

        private float _currentOffset;
        private Transform _player;

        private void LateUpdate()
        {
            SetIconsPosition();
        }

        public void Initialize(Transform player)
        {
            _player = player;
        }

        public void SetOffset(Collider2D bodyCollider)
        {
            _currentOffset = bodyCollider.bounds.extents.y;
        }
        
        public void SetRoleIcon(Color color)
        {
            roleIcon.color = color;
        }

        private void SetIconsPosition()
        {
            transform.position = new Vector3(
                _player.transform.position.x,
                _player.transform.position.y + _currentOffset + baseOffset,
                _player.transform.position.z);

            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}