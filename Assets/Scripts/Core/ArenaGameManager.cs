using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using ArenaSurvivor.Player;
using ArenaSurvivor.Pool;
using ArenaSurvivor.Spawner;

namespace ArenaSurvivor.Core
{
    public enum GameState
    {
        Playing,
        GameOver,
        Victory
    }
    
    // Quản lý vòng lặp 
    public class ArenaGameManager : MonoBehaviour
    {
        public static ArenaGameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private ArenaWaveSpawner waveSpawner;

        private int currentScore = 0;
        private GameState currentState = GameState.Playing;

        public int CurrentScore => currentScore;
        public GameState CurrentState => currentState;

        public event Action<int> OnScoreChanged;
        public event Action OnGameOver;
        public event Action OnVictory;

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

            Time.timeScale = 1f;
        }

        private void Start()
        {
            if (playerHealth != null)
            {
                playerHealth.OnPlayerDied += HandlePlayerDied;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnAllWavesCleared += HandleAllWavesCleared;
            }

            if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.OnEnemyKilled += AddScore;
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.OnPlayerDied -= HandlePlayerDied;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnAllWavesCleared -= HandleAllWavesCleared;
            }

            if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.OnEnemyKilled -= AddScore;
            }
        }

        public void AddScore(int amount)
        {
            if (currentState != GameState.Playing) return;
            currentScore += amount;
            OnScoreChanged?.Invoke(currentScore);
        }

        private void HandlePlayerDied()
        {
            if (currentState != GameState.Playing) return;
            currentState = GameState.GameOver;
            Time.timeScale = 0f;
            OnGameOver?.Invoke();
        }

        private void HandleAllWavesCleared()
        {
            if (currentState != GameState.Playing) return;
            currentState = GameState.Victory;
            Time.timeScale = 0f;
            OnVictory?.Invoke();
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
