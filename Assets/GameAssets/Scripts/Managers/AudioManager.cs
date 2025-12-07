using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    [SerializeField] private Button resetButton;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Sound effects")]
    [SerializeField] private Sound[] sounds;
    [Header("Music list")]
    [SerializeField] private Music[] musics;

    private Dictionary<SoundType, AudioSource> soundSources;
    private Dictionary<MusicType, AudioSource> musicSources;

    private const string SfxMixerGroup = "soundEffectsVolume";
    private const string MusicMixerGroup = "musicVolume";

    private AudioSource currentMusicSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        soundSources = new Dictionary<SoundType, AudioSource>();
        musicSources = new Dictionary<MusicType, AudioSource>();

        foreach (var sound in sounds)
        {
            if (sound.clip == null)
            {
                Debug.Log("Sound clip is null");
                continue;
            }

            var source = gameObject.AddComponent<AudioSource>();
            source.clip = sound.clip;
            soundSources[sound.soundType] = source;
            source.outputAudioMixerGroup = sfxGroup;
            source.loop = false;
        }

        foreach (var music in musics)
        {
            if (music.clip == null)
            {
                Debug.Log("Music clip is null");
                continue;
            }

            var source = gameObject.AddComponent<AudioSource>();
            source.clip = music.clip;
            musicSources[music.musicType] = source;
            source.outputAudioMixerGroup = musicGroup;
            source.loop = true;
        }

        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        resetButton.onClick.AddListener(ResetVolumeSettings);
    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    private void OnDestroy()
    {
        sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        resetButton.onClick.RemoveListener(ResetVolumeSettings);
    }

    public void PlaySound(SoundType soundType, float volume = 1f)
    {
        if (soundSources.TryGetValue(soundType, out var sound))
        {
            sound.Play();
            sound.volume = volume;
        }
        else
            Debug.LogWarning($"Sound {soundType} not found");
    }

    public void PlayMusic(MusicType musicType, float volume = 1f)
    {
        if (musicSources.TryGetValue(musicType, out var musicSource))
        {
            if (currentMusicSource == musicSource && musicSource.isPlaying)
                return;

            if (currentMusicSource != null && currentMusicSource.isPlaying)
            {
                currentMusicSource.Stop();
                currentMusicSource.time = 0f;
            }

            currentMusicSource = musicSource;
            musicSource.volume = volume;
            musicSource.time = 0f;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music {musicType} not found");
        }
    }

    public void StopSound(SoundType soundType)
    {
        if (soundSources.TryGetValue(soundType, out var sound))
        {
            sound.Stop();
            sound.time = 0f;
        }
    }

    private void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MusicMixerGroup, Mathf.Log10(Mathf.Max(float.Epsilon, value)) * 20);
        SaveManager.gameData.volumeData.MusicVolume = value;
    }

    private void SetSfxVolume(float value)
    {
        audioMixer.SetFloat(SfxMixerGroup, Mathf.Log10(Mathf.Max(float.Epsilon, value)) * 20);
        SaveManager.gameData.volumeData.SfxVolume = value;
    }

    private void LoadVolumeSettings()
    {
        musicSlider.value = SaveManager.gameData.volumeData.MusicVolume;
        sfxSlider.value = SaveManager.gameData.volumeData.SfxVolume;
    }

    private void ResetVolumeSettings()
    {
        SaveManager.gameData.volumeData.Reset();
        musicSlider.value = SaveManager.gameData.volumeData.MusicVolume;
        sfxSlider.value = SaveManager.gameData.volumeData.SfxVolume;
    }

    [Serializable]
    public class Sound
    {
        public AudioClip clip;
        public SoundType soundType;
    }

    [Serializable]
    public class Music
    {
        public AudioClip clip;
        public MusicType musicType;
    }
}

// P.S. - Добавляйте новые элементы снизу, иначе всё съедет!
public enum SoundType
{
    Cancel,
    Scanning,
    Buy,
    OrderComplete,
    OrderFailed,
    Painting,
    Printing,
    Notification,
    UniversalClick,
    OpenLaptop
}

public enum MusicType
{
    MainMenuMusic,
    BackgroundMusic
}