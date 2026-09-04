using Alchemy.Inspector;
using Reflex.Attributes;
using System;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Player
{
    public class PlayerHealth : MonoBehaviour, IDamagable
    {
        [Inject] private readonly IHealthStats _stats;
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;
        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;

        public bool IsDead => _currentHealth <= 0f;

        public event Action OnKilled;
        public event Action OnDamageTaken;
        private void Awake()
        {
            _maxHealth = _stats.MaxHealth;
            _currentHealth = _maxHealth;
        }

        [Button]
        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            _currentHealth = Mathf.Max(0f, _currentHealth - damage);
            OnDamageTaken?.Invoke();
            if (IsDead) OnKilled?.Invoke();
        }
    }
}
