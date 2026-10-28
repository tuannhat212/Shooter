using System;
using UnityEngine;
using ArenaSurvivor.Core;

namespace ArenaSurvivor.Player
{
    public class PlayerHealth : MonoBehaviour, IArenaDamageable
    {
        [Header("Health Configuration")]
        [SerializeField] private float maxHp = 100f;
        private float currentHp;

        public float CurrentHp => currentHp;
        public float MaxHp => maxHp;
        public bool IsDead => currentHp <= 0f;

        public event Action<float, float> OnHealthChanged;
        public event Action OnPlayerDied;

        private void Awake()
        {
            currentHp = maxHp;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHp, maxHp);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;

            currentHp = Mathf.Max(0f, currentHp - amount);
            OnHealthChanged?.Invoke(currentHp, maxHp);

            if (currentHp <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            OnPlayerDied?.Invoke();
            gameObject.SetActive(false);
        }

        public void ResetHealth()
        {
            currentHp = maxHp;
            gameObject.SetActive(true);
            OnHealthChanged?.Invoke(currentHp, maxHp);
        }
    }
}
