using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum MusicType { MainMenu, Exploration, TurnCombat, SoulCombat }

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [Header("Music Settings")]
    [SerializeField] private AudioMixer _musicMixer;
    [SerializeField][Range(0f, 1f)] private float _musicVolume = 0.5f;

    [Header("Music Clips")]
    [SerializeField] private List<MusicTrack> _musicTracks = new List<MusicTrack>();

    private AudioSource _currentTrack;
    private MusicType _currentMusicType;

    [System.Serializable]
    public class MusicTrack {
        public MusicType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.5f;
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        switch (scene.name) {
            case "MainMenu":
                PlayMusic(MusicType.MainMenu);
                break;
            case "MainGame": // Nombre de tu escena del juego
                PlayMusic(MusicType.Exploration);
                break;
        }
    }

    private void InitializeAudioSources() {
        _currentTrack = gameObject.AddComponent<AudioSource>();
        _currentTrack.loop = true;
        _currentTrack.outputAudioMixerGroup = _musicMixer.FindMatchingGroups("Music")[0];
    }

    public void PlayMusic(MusicType type, float fadeDuration = .2f) {
        if (_currentMusicType == type && _currentTrack.isPlaying) return;

        MusicTrack track = _musicTracks.Find(t => t.type == type);
        if (track != null) {
            StartCoroutine(FadeMusic(track, fadeDuration));
            _currentMusicType = type;
        }
    }

    private System.Collections.IEnumerator FadeMusic(MusicTrack newTrack, float duration) {
        float startVolume = _currentTrack.volume;
        float timer = 0f;

        // Fade out current track
        while (timer < duration) {
            _currentTrack.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Switch track
        _currentTrack.Stop();
        _currentTrack.clip = newTrack.clip;
        _currentTrack.volume = newTrack.volume * _musicVolume;
        _currentTrack.Play();

        // Fade in new track
        timer = 0f;
        startVolume = 0f;
        float targetVolume = newTrack.volume * _musicVolume;

        while (timer < duration) {
            _currentTrack.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void SetMusicVolume(float volume) {
        _musicVolume = Mathf.Clamp01(volume);
        _currentTrack.volume = _musicTracks.Find(t => t.type == _currentMusicType).volume * _musicVolume;
    }
}