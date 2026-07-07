using System;
using UnityEngine;

namespace Code.Gameplay.Player
{
    public sealed class AttackConfig : MonoBehaviour
    {
        [SerializeField] private Vector2 attackSize;
        [SerializeField] private float damage;

        public Vector2 AttackSize => attackSize;
        public float Damage => damage;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            
            Gizmos.DrawWireCube(transform.position, attackSize);
        }
    }
}