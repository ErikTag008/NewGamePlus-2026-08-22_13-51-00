using KBCore.Refs;
using Project.Assets._Project._Scripts.Player;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Weapons
{
    public interface IProjectile
    {
        void Shoot(float velocity, Vector3 direction, int damage);
    }
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBase : ValidatedMonoBehaviour, IProjectile
    {
        [SerializeField, Self] private Rigidbody _rb;
        [SerializeField] private bool _useGravity = false;
        [SerializeField] private float _gravityMagnitude = 3f;
        [SerializeField] private LayerMask _interactableLayers;
        private int _damage = 1;
        private bool _gravityEnabled = false;
        private void OnDisable()
        {
            _gravityEnabled = false;
            
        }

        private void OnEnable()
        {
            _gravityEnabled = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        public void ResetProjectile()
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.Sleep();
        }
        public virtual void Shoot(float velocity, Vector3 direction, int damage)
        {
            _damage = damage;
            _rb.AddForce(direction * velocity, ForceMode.Impulse);
        }

        private void FixedUpdate()
        {
            if (_gravityEnabled && _useGravity) 
            {
                _rb.AddForce(_gravityMagnitude * Vector3.down , ForceMode.Acceleration);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsLayerInMask(collision.gameObject.layer, _interactableLayers)) return;
            collision.collider.GetComponent<IDamagable>()?.TakeDamage(_damage);
            gameObject.SetActive(false);

        }

        private void OnTriggerEnter(Collider collision)
        {
            if (!IsLayerInMask(collision.gameObject.layer, _interactableLayers)) return;
            collision.gameObject.GetComponent<IDamagable>()?.TakeDamage(_damage);
            gameObject.SetActive(false);
        }

        private bool IsLayerInMask(int layer, LayerMask mask)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}
