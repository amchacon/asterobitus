using System;
using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AsteroidsModern.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Game UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private GameObject gameUI;

        [Header("Menu UI")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI highScoreText;

        [Header("Game Over UI")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI newHighScoreText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        [Header("Pause UI")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button pauseMenuButton;

        private GameManager _gameManager;
        
        private void Awake()
        {
            InitializeReferences();
            SubscribeToEvents();
        }

        private void InitializeReferences()
        {
            _gameManager = GameManager.Instance;
            SetupButtons();
        }

        private void SetupButtons()
        {
            startButton.onClick.AddListener(() => _gameManager.OnStartGameButtonClicked());
            quitButton.onClick.AddListener(() => _gameManager.OnQuitButtonClicked());
            restartButton.onClick.AddListener(() => _gameManager.OnRestartButtonClicked());
            menuButton.onClick.AddListener(() => _gameManager.OnMenuButtonClicked());
            resumeButton.onClick.AddListener(() => _gameManager.OnPauseButtonClicked());
            pauseMenuButton.onClick.AddListener(() => _gameManager.OnMenuButtonClicked());
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnScoreChanged += UpdateScore;
            GameEvents.OnPlayerHealthChanged += UpdateHealth;
            GameEvents.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            GameEvents.OnScoreChanged -= UpdateScore;
            GameEvents.OnPlayerHealthChanged -= UpdateHealth;
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score:N0}";
        }

        private void UpdateHealth(int health)
        {
            if (healthText != null)
                healthText.text = $"Lives: {health}";
        }
        
        private void UpdateHighScore()
        {
            highScoreText.text = $"High Score: {_gameManager.GetHighScore():N0}";
        }

        private void OnGameStateChanged(GameState newState)
        {
            ShowUIForState(newState);
        }

        private void ShowUIForState(GameState state)
        {
            SetPanelActive(menuPanel, false);
            SetPanelActive(gameUI, false);
            SetPanelActive(gameOverPanel, false);
            SetPanelActive(pausePanel, false);

            switch (state)
            {
                case GameState.Menu:
                    SetPanelActive(menuPanel, true);
                    UpdateHighScore();
                    break;

                case GameState.Playing:
                    SetPanelActive(gameUI, true);
                    break;

                case GameState.Paused:
                    SetPanelActive(gameUI, true);
                    SetPanelActive(pausePanel, true);
                    break;

                case GameState.GameOver:
                    SetPanelActive(gameOverPanel, true);
                    ShowGameOverInfo();
                    break;
            }
        }

        private void ShowGameOverInfo()
        {
            int finalScore = _gameManager.GetCurrentScore();
            finalScoreText.text = $"Final Score: {finalScore:N0}";
            
            bool isNewHighScore = _gameManager.IsNewHighScore();
            newHighScoreText.gameObject.SetActive(isNewHighScore);
            newHighScoreText.text = isNewHighScore ? "NEW HIGH SCORE!" : String.Empty;
        }
        
        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
                panel.SetActive(active);
        }
    }
}
        