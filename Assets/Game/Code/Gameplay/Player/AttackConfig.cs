using System;
using System.Collections;
using UnityEngine;

namespace Code.Gameplay.Player
{
    public sealed class AttackConfig : MonoBehaviour
    {
        [SerializeField] private Vector2 attackSize;
        [SerializeField] private float damage;

        public Vector2 AttackSize => attackSize;
        public float Damage => damage;
        
        private Color _color = Color.yellow;

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            
            Gizmos.DrawWireCube(transform.position, attackSize);
        }

        public IEnumerator SwitchGizmosColor()
        {
            _color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _color = Color.yellow;
        }
    }
}