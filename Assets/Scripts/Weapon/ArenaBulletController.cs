using UnityEngine;
using ArenaSurvivor.Core;
using ArenaSurvivor.Pool;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.Weapon
{
    public class ArenaBulletController : MonoBehaviour
    {
        [SerializeField] private float damage = 25f;
        [SerializeField] private float speed = 16f;
        [SerializeField] private float lifeTime = 2f;

        private float currentLifeTimer;
        private BulletPool poolOwner;
        private bool isReturned;

        private void OnEnable()
        {
            isReturned = false;
            currentLifeTimer = 0f;
        }

        public void Initialize(float bulletDamage, float bulletSpeed, float bulletLifeTime, BulletPool pool)
        {
            damage = bulletDamage;
            speed = bulletSpeed;
            lifeTime = bulletLifeTime;
            poolOwner = pool;
            currentLifeTimer = 0f;
            isReturned = false;
        }

        private void Update()
        {
            if (isReturned) return;
            MoveForward();
            CheckLifeTime();
        }

        private void MoveForward()
        {
            transform.Translate(Vector3.right * (speed * Time.deltaTime), Space.Self);
        }

        private void CheckLifeTime()
        {
            currentLifeTimer += Time.deltaTime;
            if (currentLifeTimer >= lifeTime)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleHit(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleHit(collision.collider);
        }

        private void HandleHit(Collider2D other)
        {
            if (isReturned || other == null) return;

            if (IsShooterOrBullet(other)) return;

            if (TryDamageTarget(other)) return;

            if (IsObstacle(other))
            {
                ReturnToPool();
            }
        }

        private bool IsShooterOrBullet(Collider2D other)
        {
            return other.CompareTag("Player") ||
                   other.CompareTag("Bullet") ||
                   other.GetComponentInParent<PlayerController>() != null ||
                   other.GetComponentInParent<PlayerHealth>() != null;
        }

        private bool TryDamageTarget(Collider2D other)
        {
            IArenaDamageable damageable = other.GetComponentInParent<IArenaDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                ReturnToPool();
                return true;
            }
            return false;
        }

        private bool IsObstacle(Collider2D other)
        {
            bool hasObstacleTag = other.CompareTag("Wall") || other.CompareTag("Box");
            bool hasObstacleName = other.gameObject.name.Contains("Wall") ||
                                   other.gameObject.name.Contains("Box") ||
                                   (other.transform.parent != null &&
                                    (other.transform.parent.name.Contains("Wall") ||
                                     other.transform.parent.name.Contains("Box")));
            bool isTilemap = other.GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>() != null ||
                             other.GetComponent<CompositeCollider2D>() != null;

            return hasObstacleTag || hasObstacleName || isTilemap || !other.isTrigger;
        }

        private void ReturnToPool()
        {
            if (isReturned) return;
            isReturned = true;

            if (poolOwner != null)
            {
                poolOwner.ReturnBullet(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
