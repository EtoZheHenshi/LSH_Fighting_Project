using System;
using System.Collections;
using Code.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Player.Attacks
{
    [RequireComponent(typeof(Collider2DGizmo))]
    public sealed class AttackConfig : MonoBehaviour
    {
        [SerializeField] private Vector2 attackSize;
        [SerializeField] private float damage;

        private Collider2DGizmo _gizmo;

        private void Awake()
        {
            _gizmo = GetComponent<Collider2DGizmo>();
        }

        public Vector2 AttackSize => attackSize;
        public float Damage => damage;

        public void VisualizeAttack()
        {
            _gizmo.SwitchGizmosColor(Color.red, 0.3f).Forget();
        }
    }
}