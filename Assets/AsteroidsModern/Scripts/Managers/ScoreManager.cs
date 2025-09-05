using System;
using AsteroidsModern.Core;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Managers
{
    public class ScoreManager : IDisposable
    {
        private const string HIGH_SCORE_KEY = "HighScore";
        
        private int _currentScore;
        private int _highScore;

        internal int CurrentScore => _currentScore;
        internal int HighScore => _highScore;
        internal bool IsNewHighScore() => _currentScore > PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        public ScoreManager()
        {
            LoadHighScore();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnAsteroidDestroyed += OnAsteroidDestroyed;
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnGameOver += OnGameOver;
        }

        private void OnAsteroidDestroyed(IAsteroid asteroid)
        {
            if (asteroid == null)
            {
                Debug.LogError("ScoreManager: OnAsteroidDestroyed received null asteroid");
                return;
            }

            int points = asteroid.ScoreValue;
            AddScore(points);
        }

        private void AddScore(int points)
        {
            if (points <= 0)
            {
                Debug.LogError($"ScoreManager: Attempted to add non-positive score: {points}");
                return;
            }

            _currentScore += points;
            GameEvents.TriggerScoreChanged(_currentScore);
        }

        private void OnGameStarted()
        {
            _currentScore = 0;
            GameEvents.TriggerScoreChanged(_currentScore);
        }

        private void LoadHighScore()
        {
            _highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        }

        private void OnGameOver()
        {
            if (_currentScore > _highScore)
            {
                _highScore = _currentScore;
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, _highScore);
                PlayerPrefs.Save();
            }
        }
        
        public void Dispose()
        {
            GameEvents.OnAsteroidDestroyed -= OnAsteroidDestroyed;
            GameEvents.OnGameStarted -= OnGameStarted;
            GameEvents.OnGameOver -= OnGameOver;
        }
    }
}