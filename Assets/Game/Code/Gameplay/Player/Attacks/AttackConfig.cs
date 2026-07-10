using System;
using System.Collections;
using Code.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Player.Attacks
{
    public sealed class AttackConfig : MonoBehaviour
    {
        [SerializeField] private Transform attackPosition;
        [SerializeField] private Vector2 attackSize;
        [SerializeField] private float damage;
        [SerializeField] private Collider2DGizmo gizmo;

        public Transform AttackPosition => attackPosition;
        public Vector2 AttackSize => attackSize;
        public float Damage => damage;

        public void VisualizeAttack()
        {
            gizmo.SwitchGizmosColor(Color.red, 0.3f).Forget();
        }
    }
}