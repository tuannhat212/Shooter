using System;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Data;
using ArenaSurvivor.Enemy;

namespace ArenaSurvivor.Pool
{
    public class EnemyPool : MonoBehaviour
    {
        public static EnemyPool Instance { get; private set; }

        [Header("Pool Setup")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int initialPoolSize = 25;

        private readonly Queue<EnemyController> poolQueue = new Queue<EnemyController>();

        public event Action<int> OnEnemyKilled;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                EnemyController enemy = CreateNewEnemyInstance();
                enemy.gameObject.SetActive(false);
                poolQueue.Enqueue(enemy);
            }
        }

        private EnemyController CreateNewEnemyInstance()
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            EnemyController enemy = obj.GetComponent<EnemyController>();
            if (enemy == null)
            {
                enemy = obj.AddComponent<EnemyController>();
            }
            return enemy;
        }

        public EnemyController SpawnEnemy(EnemyDataSO data, Vector3 spawnPosition, Transform targetPlayer)
        {
            EnemyController enemy = (poolQueue.Count > 0) ? poolQueue.Dequeue() : CreateNewEnemyInstance();

            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = Quaternion.identity;
            enemy.gameObject.SetActive(true);
            enemy.Initialize(data, targetPlayer, this);
            return enemy;
        }

        public void NotifyEnemyDied(EnemyController enemy, int rewardScore)
        {
            enemy.gameObject.SetActive(false);
            enemy.transform.SetParent(transform);
            poolQueue.Enqueue(enemy);

            OnEnemyKilled?.Invoke(rewardScore);
        }
    }
}
