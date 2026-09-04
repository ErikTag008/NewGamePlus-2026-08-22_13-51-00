using KBCore.Refs;
using Project.Assets._Project._Scripts.Player;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Boss
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class AttackCollider : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Rigidbody _rb;
        [SerializeField, Self] private CapsuleCollider _collider;
        private bool _canDamage = true;
        private float _damage;

        private void Awake()
        {
            _collider.enabled = false;
            _collider.isTrigger = true;
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public void ToggleColider(bool isActive, float damage)
        {
            _damage = damage;
            _collider.enabled = isActive;
            if (isActive) _canDamage = true;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if(_canDamage && collision.TryGetComponent(out PlayerHealth health))
            {
                health.TakeDamage(_damage);
                _canDamage = false;
            }
        }

    }
}
