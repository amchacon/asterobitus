using System.Collections;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Scripts.Effects
{
    public class SimpleEffect : MonoBehaviour, IVisualEffect
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _defaultDuration = 1f;

        private void Awake()
        {
            if (_particleSystem == null)
                _particleSystem = GetComponent<ParticleSystem>();
            
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
        }

        public void PlayEffect(Vector2 position, float duration = 1f)
        {
            transform.position = position;
            
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }
            
            if (_audioSource != null && _audioSource.clip != null)
            {
                _audioSource.Play();
            }

            float effectDuration = duration > 0 ? duration : _defaultDuration;
            StartCoroutine(DestroyAfterDuration(effectDuration));
        }

        public void StopEffect()
        {
            if (_particleSystem != null && _particleSystem.isPlaying)
            {
                _particleSystem.Stop();
            }
            
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }

        private IEnumerator DestroyAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            
            StopEffect();
            
            if (transform.parent?.name != "Effects")
            {
                Destroy(gameObject);
            }
        }
    }
}