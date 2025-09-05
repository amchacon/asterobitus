using System;
using System.Collections.Generic;
using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Extensions;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Scripts.Audio
{
    [Serializable]
    public class AudioClipData
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop;
    }

    public class AudioManager : Singleton<AudioManager>
    {
        private const int SFX_POOL_SIZE = 10;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Audio Clips")]
        [SerializeField] private AudioClipData[] audioClips;
        
        [Header("Settings")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

        private readonly Dictionary<string, AudioClipData> _clipDatabase = new();
        private readonly List<AudioSource> _sfxPool = new();

        private void Awake()
        {
            InitializeAudioManager();
            SubscribeToEvents();

        }

        private void Start()
        {
            LoadAudioSettings();
        }

        private void InitializeAudioManager()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            CreateSfxPool();

            BuildClipDatabase();
        }

        private void CreateSfxPool()
        {
            for (int i = 0; i < SFX_POOL_SIZE; i++)
            {
                GameObject sfxGo = new GameObject($"SFX_AudioSource_{i}");
                sfxGo.transform.SetParent(transform);
                AudioSource source = sfxGo.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                _sfxPool.Add(source);
            }
        }

        private void BuildClipDatabase()
        {
            _clipDatabase.Clear();
            foreach (var clipData in audioClips)
            {
                if (!string.IsNullOrEmpty(clipData.name) && clipData.clip != null)
                {
                    _clipDatabase[clipData.name] = clipData;
                }
            }
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnProjectileFired += OnProjectileFired;
            GameEvents.OnAsteroidDestroyed += OnAsteroidDestroyed;
            GameEvents.OnPlayerDied += PlayerDied;
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnGameOver += OnGameOver;
        }

        protected override void OnDestroy()
        {
            GameEvents.OnProjectileFired -= OnProjectileFired;
            GameEvents.OnAsteroidDestroyed -= OnAsteroidDestroyed;
            GameEvents.OnPlayerDied -= PlayerDied;
            GameEvents.OnGameStarted -= OnGameStarted;
            GameEvents.OnGameOver -= OnGameOver;
            
            base.OnDestroy();
        }

        // Event handlers
        private void OnProjectileFired(Vector2 position)
        {
            PlaySfx("shoot");
        }

        private void OnAsteroidDestroyed(IAsteroid asteroid)
        {
            string soundName = asteroid.Size switch
            {
                AsteroidSize.Large => "asteroid_large_destroy",
                AsteroidSize.Medium => "asteroid_medium_destroy", 
                AsteroidSize.Small => "asteroid_small_destroy",
                _ => "asteroid_destroy"
            };
            PlaySfx(soundName);
        }

        private void PlayerDied()
        {
            PlaySfx("player_destroy");
        }

        private void OnGameStarted()
        {
            PlayMusic("game_music");
        }

        private void OnGameOver()
        {
            StopMusic();
            PlaySfx("game_over");
        }

        private void PlaySfx(string clipName)
        {
            if (!_clipDatabase.TryGetValue(clipName, out var clipData))
            {
                Debug.LogWarning($"AudioManager: Clip '{clipName}' not found!");
                return;
            }

            AudioSource availableSource = GetAvailableSfxSource();
            
            if (availableSource != null)
            {
                availableSource.clip = clipData.clip;
                availableSource.volume = clipData.volume * sfxVolume * masterVolume;
                availableSource.pitch = clipData.pitch;
                availableSource.loop = clipData.loop;
                availableSource.PlayOneShot(clipData.clip);
            }
        }

        private void PlayMusic(string clipName)
        {
            if (!_clipDatabase.TryGetValue(clipName, out var clipData))
            {
                Debug.LogWarning($"AudioManager: SFX '{clipName}' not found!");
                return;
            }

            musicSource.clip = clipData.clip;
            musicSource.volume = clipData.volume * musicVolume * masterVolume;
            musicSource.pitch = clipData.pitch;
            musicSource.loop = true;
            musicSource.Play();
        }

        private void StopMusic()
        {
            if (musicSource.isPlaying)
                musicSource.Stop();
        }

        public void PauseMusic()
        {
            if (musicSource.isPlaying)
                musicSource.Pause();
        }

        public void ResumeMusic()
        {
            if (!musicSource.isPlaying && musicSource.clip != null)
                musicSource.UnPause();
        }

        private AudioSource GetAvailableSfxSource()
        {
            foreach (var source in _sfxPool)
            {
                if (!source.isPlaying)
                    return source;
            }

            return _sfxPool.Count > 0 ? _sfxPool[0] : sfxSource;
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
            SaveAudioSettings();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
            SaveAudioSettings();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            SaveAudioSettings();
        }

        private void UpdateAllVolumes()
        {
            UpdateMusicVolume();
        }

        private void UpdateMusicVolume()
        {
            if (musicSource != null && musicSource.clip != null)
            {
                string currentClipName = GetClipName(musicSource.clip);
                if (!string.IsNullOrEmpty(currentClipName) && _clipDatabase.TryGetValue(currentClipName, out var clipData))
                {
                    musicSource.volume = clipData.volume * musicVolume * masterVolume;
                }
            }
        }

        private string GetClipName(AudioClip clip)
        {
            foreach (var kvp in _clipDatabase)
            {
                if (kvp.Value.clip == clip)
                    return kvp.Key;
            }
            return null;
        }

        private void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            UpdateAllVolumes();
        }

        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.Save();
        }
        
        public bool IsPlayingMusic()
        {
            return musicSource != null && musicSource.isPlaying;
        }

        public bool IsPlayingSfx(string clipName)
        {
            if (!_clipDatabase.TryGetValue(clipName, out var value)) return false;

            var clip = value.clip;
            foreach (var source in _sfxPool)
            {
                if (source.isPlaying && source.clip == clip)
                    return true;
            }

            return sfxSource.isPlaying && sfxSource.clip == clip;
        }

        public void StopAllSfx()
        {
            foreach (var source in _sfxPool)
            {
                if (source.isPlaying)
                    source.Stop();
            }

            if (sfxSource.isPlaying)
                sfxSource.Stop();
        }
        
    }
}
        