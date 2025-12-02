using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    [SerializeField] private Button resetButton;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private Sound[] sounds;

    private Dictionary<SoundType, AudioSource> soundSources;

    private const string SfxMixerGroup = "soundEffectsVolume";
    private const string MusicMixerGroup = "musicVolume";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        soundSources = new Dictionary<SoundType, AudioSource>();

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

            if (sound.soundType == SoundType.BackgroundMusic)
            {
                source.outputAudioMixerGroup = musicGroup;
                source.loop = true;
                source.Play();
            }
            else
            {
                source.outputAudioMixerGroup = sfxGroup;
                source.loop = false;
            }
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

    public void PlaySound(SoundType soundType)
    {
        soundSources.TryGetValue(soundType, out var sound);
        sound.Play();
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
}

public enum SoundType
{
    BackgroundMusic,
    Switch,
    Open,
    Close,
    Accept,
    Cancel,
    Scanning,
    Buy,
    OrderComplete,
    Nippers,
    Sandpaper,
    Painting,
    Printing,
    Other
}