using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArenaSurvivor.Core;
using ArenaSurvivor.Player;
using ArenaSurvivor.Spawner;

namespace ArenaSurvivor.UI
{
    public class ArenaUIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI enemiesRemainingText;

        [Header("Panels")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;

        [Header("Buttons")]
        [SerializeField] private Button restartButtonGameOver;
        [SerializeField] private Button restartButtonVictory;

        [Header("References")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private ArenaWaveSpawner waveSpawner;

        private void Start()
        {
            SetupButtons();
            SubscribeEvents();
            HidePanels();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SetupButtons()
        {
            if (restartButtonGameOver != null)
            {
                restartButtonGameOver.onClick.AddListener(OnRestartClicked);
            }

            if (restartButtonVictory != null)
            {
                restartButtonVictory.onClick.AddListener(OnRestartClicked);
            }
        }

        private void SubscribeEvents()
        {
            if (ArenaGameManager.Instance != null)
            {
                ArenaGameManager.Instance.OnScoreChanged += UpdateScoreDisplay;
                ArenaGameManager.Instance.OnGameOver += ShowGameOverModal;
                ArenaGameManager.Instance.OnVictory += ShowVictoryModal;
            }

            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthDisplay;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnWaveChanged += UpdateWaveDisplay;
                waveSpawner.OnEnemiesRemainingChanged += UpdateEnemiesDisplay;
            }
        }

        private void UnsubscribeEvents()
        {
            if (ArenaGameManager.Instance != null)
            {
                ArenaGameManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
                ArenaGameManager.Instance.OnGameOver -= ShowGameOverModal;
                ArenaGameManager.Instance.OnVictory -= ShowVictoryModal;
            }

            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealthDisplay;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnWaveChanged -= UpdateWaveDisplay;
                waveSpawner.OnEnemiesRemainingChanged -= UpdateEnemiesDisplay;
            }
        }

        private void HidePanels()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
        }

        private void UpdateHealthDisplay(float currentHp, float maxHp)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHp;
                healthSlider.value = currentHp;
            }

            if (healthText != null)
            {
                healthText.text = $"HP: {Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(maxHp)}";
            }
        }

        private void UpdateScoreDisplay(int newScore)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {newScore}";
            }
        }

        private void UpdateWaveDisplay(int currentWave, int totalWaves)
        {
            if (waveText != null)
            {
                waveText.text = $"Wave: {currentWave} / {totalWaves}";
            }
        }

        private void UpdateEnemiesDisplay(int remainingCount)
        {
            if (enemiesRemainingText != null)
            {
                enemiesRemainingText.text = $"Enemies: {remainingCount}";
            }
        }

        private void ShowGameOverModal()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }

        private void ShowVictoryModal()
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }

        private void OnRestartClicked()
        {
            if (ArenaGameManager.Instance != null)
            {
                ArenaGameManager.Instance.RestartGame();
            }
        }
    }
}
