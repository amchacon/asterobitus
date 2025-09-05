using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Extensions;
using UnityEditor;
using UnityEngine;

namespace AsteroidsModern.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameState currentState;

        private ScoreManager _scoreManager;

        protected void Awake() => SubscribeToEvents();

        private void Start()
        {
            InitializeManagers();
            ChangeGameState(GameState.Menu);
        }
        
        private void Update() => HandleInput();
        
        protected override void OnDestroy()
        {
            GameEvents.OnPlayerDied -= OnPlayerDied;
            _scoreManager.Dispose();
            GameEvents.ClearAllEvents();
            base.OnDestroy();
        }

        private void SubscribeToEvents() => GameEvents.OnPlayerDied += OnPlayerDied;
        
        private void InitializeManagers() => _scoreManager = new ScoreManager();

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentState == GameState.Playing)
                {
                    PauseGame();
                }
                else if (currentState == GameState.Paused)
                {
                    ResumeGame();
                }
            }
        }
        
        public void OnMenuButtonClicked() => ChangeGameState(GameState.Menu);

        public void OnStartGameButtonClicked() => StartGame();

        public void OnPauseButtonClicked()
        {
            if (currentState == GameState.Playing)
                PauseGame();
            else if (currentState == GameState.Paused)
                ResumeGame();
        }

        public void OnRestartButtonClicked() => StartGame();

        public void OnQuitButtonClicked() => QuitGame();

        private void StartGame()
        {
            ChangeGameState(GameState.Playing);
            GameEvents.TriggerGameStarted();
        }

        private void PauseGame()
        {
            if (currentState != GameState.Playing) return;

            ChangeGameState(GameState.Paused);
            GameEvents.TriggerGamePaused();
        }

        private void ResumeGame()
        {
            if (currentState != GameState.Paused) return;

            ChangeGameState(GameState.Playing);
            GameEvents.TriggerGameResumed();
        }

        public void EndGame()
        {
            ChangeGameState(GameState.GameOver);
            GameEvents.TriggerGameOver();
        }

        private void QuitGame()
        {
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private void ChangeGameState(GameState newState)
        {
            if (currentState == newState) return;
            
            currentState = newState;

            GameEvents.TriggerGameStateChanged(currentState);
        }

        private void OnPlayerDied()
        {
            if (currentState != GameState.Playing) return;
            Invoke(nameof(EndGame), 2f); 
        }
        
        public int GetCurrentScore() => _scoreManager?.CurrentScore ?? 0;
        public int GetHighScore() => _scoreManager?.HighScore ?? 0;
        public bool IsNewHighScore() => _scoreManager?.IsNewHighScore() ?? false;
    }
}