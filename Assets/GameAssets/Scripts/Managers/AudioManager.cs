using System;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioMixer audioMixer;
    
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    [Range(float.Epsilon, 1), SerializeField] private float musicDefaultVolume;
    [Range(float.Epsilon, 1), SerializeField] private float sfxDefaultVolume;
    
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip musicClip;
    
    private event Action OnVolumeChange;
    
    private const string MusicMixerGroup = "MusicVolume";
    private const string SfxMixerGroup = "soundEffectsVolume";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);

        OnVolumeChange += SaveVolumeSettings;
        
        if (SaveManager.gameData.volumeSettings.Count != 0) LoadVolumeSettings();
        else
        {
            SetMusicVolume(musicDefaultVolume);
            SetSfxVolume(sfxDefaultVolume);
        }
        
        SaveVolumeSettings();
    }

    private void Start()
    {
        PlayMusic();
    }

    private void OnDestroy()
    {
        OnVolumeChange -= SaveVolumeSettings;
    }

    public void PlayClickSound()
    {
        var sfxSource = this.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(SfxMixerGroup)[1];
        sfxSource.clip = clickSound;
        sfxSource.loop = false;
        sfxSource.Play();
    }

    private void PlayMusic()
    {
        var musicSource = this.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(MusicMixerGroup)[0];
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    private void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MusicMixerGroup, Mathf.Log10(Mathf.Max(float.Epsilon, value)) * 20);
        OnVolumeChange?.Invoke();
    }

    private void SetSfxVolume(float value)
    {
        audioMixer.SetFloat(SfxMixerGroup, Mathf.Log10(Mathf.Max(float.Epsilon, value)) * 20);
        OnVolumeChange?.Invoke();
    }

    private void SaveVolumeSettings()
    {
        if (SaveManager.gameData.volumeSettings.Count < 2) return;
        SaveManager.gameData.volumeSettings.Clear();
        SaveManager.gameData.volumeSettings.Add(musicSlider.value.ToString(CultureInfo.InvariantCulture));
        SaveManager.gameData.volumeSettings.Add(sfxSlider.value.ToString(CultureInfo.InvariantCulture));
    }

    private void LoadVolumeSettings()
    {
        musicSlider.value = SaveManager.gameData.volumeSettings.IndexOf(SaveManager.gameData.volumeSettings[0]);
        sfxSlider.value = SaveManager.gameData.volumeSettings.IndexOf(SaveManager.gameData.volumeSettings[1]);
    }
}