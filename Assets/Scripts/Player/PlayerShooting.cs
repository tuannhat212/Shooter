using UnityEngine;
using ArenaSurvivor.Data;
using ArenaSurvivor.Pool;
using ArenaSurvivor.Weapon;
using ArenaSurvivor.UI;

namespace ArenaSurvivor.Player
{
    public class PlayerShooting : MonoBehaviour
    {
        [Header("Weapon Configuration")]
        [SerializeField] private WeaponDataSO weaponData;
        [SerializeField] private Transform firePoint;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip shootSound;

        [Header("Animation")]
        [SerializeField] private Animator animator;

        private float nextFireTime;
        private Collider2D playerCollider;

        private void Awake()
        {
            playerCollider = GetComponent<Collider2D>();
            if (animator == null) animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            HandleShootingInput();
        }

        private void HandleShootingInput()
        {
            // Hỗ trợ cả nhấp chuột đơn lẻ và đè giữ chuột trái
            if (Input.GetMouseButton(0))
            {
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                }
            }
        }

        private void Shoot()
        {
            if (weaponData == null || firePoint == null || BulletPool.Instance == null) return;

            GameObject bullet = BulletPool.Instance.GetBullet(firePoint.position, firePoint.rotation);

            if (playerCollider != null)
            {
                Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
                if (bulletCollider != null)
                {
                    Physics2D.IgnoreCollision(bulletCollider, playerCollider, true);
                }
            }

            ArenaBulletController bulletController = bullet.GetComponent<ArenaBulletController>();
            if (bulletController != null)
            {
                bulletController.Initialize(
                    weaponData.damage,
                    weaponData.bulletSpeed,
                    weaponData.bulletLifeTime,
                    BulletPool.Instance
                );
            }

            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
            nextFireTime = Time.time + (1f / Mathf.Max(0.01f, weaponData.fireRate));
        }
    }
}
