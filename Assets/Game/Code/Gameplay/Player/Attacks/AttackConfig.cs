using System.Collections;
using UnityEngine;

namespace Code.Gameplay.Player.Attacks
{
    public sealed class AttackConfig : MonoBehaviour
    {
        [SerializeField] private Vector2 attackSize;
        [SerializeField] private float damage;

        public Vector2 AttackSize => attackSize;
        public float Damage => damage;
    }
}