using UnityEngine;
using ArenaSurvivor.Core;
using ArenaSurvivor.Data;
using ArenaSurvivor.Pool;

namespace ArenaSurvivor.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : MonoBehaviour, IArenaDamageable
    {
        public enum AIState
        {
            Patrol,
            Chase
        }

        [Header("State")]
        [SerializeField] private AIState currentState = AIState.Patrol;

        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Runtime Data")]
        [SerializeField] private EnemyDataSO enemyData;
        private float currentHp;
        private Transform playerTransform;
        private EnemyPool poolOwner;

        [Header("Patrol")]
        private Vector2 spawnPoint;
        private Vector2 currentPatrolTarget;
        private float patrolTimer;
        private const float PATROL_CHANGE_INTERVAL = 3f;
        private const float PATROL_RADIUS = 3.5f;

        [Header("Attack")]
        private float lastAttackTime;
        private const float DAMAGE_COOLDOWN = 1f;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Initialize(EnemyDataSO data, Transform targetPlayer, EnemyPool pool)
        {
            enemyData = data;
            playerTransform = targetPlayer;
            poolOwner = pool;
            
            currentHp = enemyData.maxHp;
            currentState = AIState.Patrol;
            spawnPoint = transform.position;
            patrolTimer = 0f;
            lastAttackTime = 0f;

            ApplyVisuals();
            PickNewPatrolTarget();
        }

        private void ApplyVisuals()
        {
            if (enemyData == null || spriteRenderer == null) return;
            if (enemyData.bodySprite != null) spriteRenderer.sprite = enemyData.bodySprite;
            spriteRenderer.color = enemyData.bodyColor;
            transform.localScale = enemyData.visualScale;
        }

        private void Update()
        {
            if (playerTransform == null) return;

            EvaluateStateTransitions();
            ExecuteCurrentState();
        }

        private void EvaluateStateTransitions()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (currentState == AIState.Patrol)
            {
                if (distanceToPlayer <= enemyData.detectRange)
                {
                    currentState = AIState.Chase;
                }
            }
            else if (currentState == AIState.Chase)
            {
                if (distanceToPlayer > enemyData.detectRange * 1.25f)
                {
                    currentState = AIState.Patrol;
                    PickNewPatrolTarget();
                }
            }
        }

        private void ExecuteCurrentState()
        {
            switch (currentState)
            {
                case AIState.Patrol:
                    HandlePatrol();
                    break;
                case AIState.Chase:
                    HandleChase();
                    break;
            }
        }

        private void HandlePatrol()
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= PATROL_CHANGE_INTERVAL || Vector2.Distance(transform.position, currentPatrolTarget) < 0.2f)
            {
                PickNewPatrolTarget();
            }

            MoveTowards(currentPatrolTarget, enemyData.moveSpeed * 0.6f);
        }

        private void PickNewPatrolTarget()
        {
            patrolTimer = 0f;
            Vector2 randomOffset = Random.insideUnitCircle * PATROL_RADIUS;
            currentPatrolTarget = spawnPoint + randomOffset;
        }

        private void HandleChase()
        {
            if (playerTransform == null) return;
            MoveTowards(playerTransform.position, enemyData.moveSpeed);
        }

        private void MoveTowards(Vector2 targetPosition, float speed)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * speed;

            // Xoay hướng mặt về phía di chuyển
            if (direction.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player")) return;
            
            if (Time.time >= lastAttackTime + DAMAGE_COOLDOWN)
            {
                IArenaDamageable playerDamageable = collision.gameObject.GetComponent<IArenaDamageable>();
                if (playerDamageable != null)
                {
                    playerDamageable.TakeDamage(enemyData.attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }
        public void TakeDamage(float amount)
        {
            currentHp -= amount;
            if (currentHp <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            if (poolOwner != null)
            {
                poolOwner.NotifyEnemyDied(this, enemyData.rewardScore);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
