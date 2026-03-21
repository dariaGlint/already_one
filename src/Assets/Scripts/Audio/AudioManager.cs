using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlreadyOne.Audio
{
    [DisallowMultipleComponent]
    public sealed class AudioManager : MonoBehaviour
    {
        private const string BgmResourcePrefix = "Audio/BGM/";
        private const string SeResourcePrefix = "Audio/SE/";

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _seSource;

        private readonly Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>(StringComparer.Ordinal);

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            EnsureAudioSources();
            ConfigureAudioSources();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void PlaySE(string clipName)
        {
            AudioClip clip = LoadClip(SeResourcePrefix, clipName);
            if (clip == null || _seSource == null)
            {
                return;
            }

            _seSource.PlayOneShot(clip);
        }

        public void PlayBGM(string clipName)
        {
            AudioClip clip = LoadClip(BgmResourcePrefix, clipName);
            if (clip == null || _bgmSource == null)
            {
                return;
            }

            if (_bgmSource.clip != clip)
            {
                _bgmSource.clip = clip;
            }

            if (_bgmSource.isPlaying)
            {
                _bgmSource.Stop();
            }

            _bgmSource.Play();
        }

        public void StopBGM()
        {
            if (_bgmSource == null)
            {
                return;
            }

            _bgmSource.Stop();
        }

        private void EnsureAudioSources()
        {
            if (_bgmSource == _seSource)
            {
                _seSource = null;
            }

            AudioSource[] audioSources = GetComponents<AudioSource>();

            if (_bgmSource == null && audioSources.Length > 0)
            {
                _bgmSource = audioSources[0];
            }

            if (_seSource == null)
            {
                for (int i = 0; i < audioSources.Length; i++)
                {
                    if (audioSources[i] == _bgmSource)
                    {
                        continue;
                    }

                    _seSource = audioSources[i];
                    break;
                }
            }

            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
            }

            if (_seSource == null)
            {
                _seSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void ConfigureAudioSources()
        {
            if (_bgmSource != null)
            {
                _bgmSource.playOnAwake = false;
                _bgmSource.loop = true;
                _bgmSource.spatialBlend = 0f;
            }

            if (_seSource != null)
            {
                _seSource.playOnAwake = false;
                _seSource.loop = false;
                _seSource.spatialBlend = 0f;
            }
        }

        private AudioClip LoadClip(string prefix, string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
            {
                Debug.LogWarning($"{nameof(AudioManager)} received an empty clip name.");
                return null;
            }

            string resourcePath = prefix + clipName;
            if (_clipCache.TryGetValue(resourcePath, out AudioClip cachedClip))
            {
                return cachedClip;
            }

            AudioClip loadedClip = Resources.Load<AudioClip>(resourcePath);
            if (loadedClip == null)
            {
                Debug.LogWarning($"Audio clip not found at Resources/{resourcePath}.");
            }

            _clipCache[resourcePath] = loadedClip;
            return loadedClip;
        }
    }
}
