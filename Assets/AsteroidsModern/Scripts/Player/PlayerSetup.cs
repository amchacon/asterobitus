using AsteroidsModern.Core;
using AsteroidsModern.Extensions;
using AsteroidsModern.Managers;
using UnityEngine;

namespace AsteroidsModern.Player
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerShooting playerShooting;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private Collider2D playerCollider;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private ScreenBounds screenBounds;
        
        private GameSettings GameSettings => ConfigLoader.Instance.GameSettings;
        
        private void Awake()
        {
            SubscribeToEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeToEvents();
        }
        
        private void SubscribeToEvents()
        {
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnPlayerDied += OnPlayerDied;
            GameEvents.OnGamePaused += DisablePlayer;
            GameEvents.OnGameResumed += EnablePlayer;
        }
        
        private void UnsubscribeToEvents()
        {
            GameEvents.OnGameStarted -= OnGameStarted;
            GameEvents.OnPlayerDied -= OnPlayerDied;
            GameEvents.OnGamePaused -= DisablePlayer;
            GameEvents.OnGameResumed -= EnablePlayer;
        }

        private void OnGameStarted()
        { 
            EnablePlayer();
            playerHealth.Initialize(GameSettings);
            playerMovement.Initialize(GameSettings);
        }
        
        private void OnPlayerDied()
        {
            DisablePlayer();
        }
        
        private void EnablePlayer()
        {
            playerMovement.enabled = true;
            playerShooting.enabled = true;
            playerHealth.enabled = true;
            playerCollider.isTrigger = true;
            playerSpriteRenderer.enabled = true;
            screenBounds.enabled = true;
        }
        
        private void DisablePlayer()
        {
            playerMovement.enabled = false;
            playerShooting.enabled = false;
            playerHealth.enabled = false;
            playerCollider.isTrigger = false;
            playerSpriteRenderer.enabled = false;
            screenBounds.enabled = false;
        }
    }
}