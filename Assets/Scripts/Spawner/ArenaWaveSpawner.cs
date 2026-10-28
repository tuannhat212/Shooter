using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Data;
using ArenaSurvivor.Pool;

namespace ArenaSurvivor.Spawner
{
    [Serializable]
    public class WaveConfig
    {
        public string waveTitle = "Wave 1";
        public int fastZombieCount = 6;
        public int tankZombieCount = 0;
        public float spawnInterval = 0.8f;
    }
    // Quản lý 3 Wave
    public class ArenaWaveSpawner : MonoBehaviour
    {
        [Header("Target & Spawn Points")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform[] spawnPoints;

        [Header("Enemy Configurations")]
        [SerializeField] private EnemyDataSO fastZombieData;
        [SerializeField] private EnemyDataSO tankZombieData;

        [Header("Wave Progression")]
        [SerializeField] private WaveConfig[] waves = new WaveConfig[]
        {
            new WaveConfig { waveTitle = "Wave 1", fastZombieCount = 6, tankZombieCount = 0, spawnInterval = 0.8f },
            new WaveConfig { waveTitle = "Wave 2", fastZombieCount = 8, tankZombieCount = 3, spawnInterval = 0.6f },
            new WaveConfig { waveTitle = "Wave 3", fastZombieCount = 10, tankZombieCount = 6, spawnInterval = 0.5f }
        };

        private int currentWaveIndex = -1;
        private int aliveEnemiesCount = 0;
        private bool isSpawningCurrentWave = false;

        public event Action<int, int> OnWaveChanged;
        public event Action<int> OnEnemiesRemainingChanged;
        public event Action OnAllWavesCleared;

        private void Start()
        {
            if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.OnEnemyKilled += HandleEnemyKilled;
            }

            StartCoroutine(StartFirstWaveRoutine());
        }

        private void OnDestroy()
        {
            if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.OnEnemyKilled -= HandleEnemyKilled;
            }
        }

        private IEnumerator StartFirstWaveRoutine()
        {
            yield return new WaitForSeconds(1.5f);
            StartNextWave();
        }

        public void StartNextWave()
        {
            currentWaveIndex++;
            if (currentWaveIndex >= waves.Length)
            {
                OnAllWavesCleared?.Invoke();
                return;
            }

            OnWaveChanged?.Invoke(currentWaveIndex + 1, waves.Length);
            StartCoroutine(SpawnWaveRoutine(waves[currentWaveIndex]));
        }

        private IEnumerator SpawnWaveRoutine(WaveConfig config)
        {
            isSpawningCurrentWave = true;
            List<EnemyDataSO> spawnQueue = GenerateSpawnQueue(config);
            aliveEnemiesCount = spawnQueue.Count;
            OnEnemiesRemainingChanged?.Invoke(aliveEnemiesCount);

            foreach (EnemyDataSO enemyData in spawnQueue)
            {
                if (playerTransform == null) yield break;

                Transform spawnPoint = GetRandomSpawnPoint();
                EnemyPool.Instance.SpawnEnemy(enemyData, spawnPoint.position, playerTransform);

                yield return new WaitForSeconds(config.spawnInterval);
            }

            isSpawningCurrentWave = false;
        }

        private List<EnemyDataSO> GenerateSpawnQueue(WaveConfig config)
        {
            List<EnemyDataSO> list = new List<EnemyDataSO>();

            for (int i = 0; i < config.fastZombieCount; i++) list.Add(fastZombieData);
            for (int i = 0; i < config.tankZombieCount; i++) list.Add(tankZombieData);

            // Xáo trộn thứ tự quái ngẫu nhiên
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }

            return list;
        }

        private Transform GetRandomSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return transform;
            return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        }

        private void HandleEnemyKilled(int rewardScore)
        {
            aliveEnemiesCount = Mathf.Max(0, aliveEnemiesCount - 1);
            OnEnemiesRemainingChanged?.Invoke(aliveEnemiesCount);

            if (!isSpawningCurrentWave && aliveEnemiesCount <= 0)
            {
                StartCoroutine(TransitionToNextWaveRoutine());
            }
        }

        private IEnumerator TransitionToNextWaveRoutine()
        {
            // Chờ 3 giây trước khi chuyển sang Wave tt
            yield return new WaitForSeconds(3f);

            if (currentWaveIndex < waves.Length - 1)
            {
                StartNextWave();
            }
            else
            {
                OnAllWavesCleared?.Invoke();
            }
        }
    }
}
