using System.Collections;
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
    
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip musicClip;
    
    private const string SfxMixerGroup = "soundEffectsVolume"; 
    private const string MusicMixerGroup = "musicVolume";
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        sfxSlider.onValueChanged.AddListener(SetSfxVolume); 
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        resetButton.onClick.AddListener(ResetVolumeSettings);
    }

    private void Start()
    {
        LoadVolumeSettings();
        PlayMusic();
    }

    private void OnDestroy()
    {
        sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        resetButton.onClick.RemoveListener(ResetVolumeSettings);
    }

    public void PlayClickSound()
    {
        StartCoroutine(ClickSound());
    }

    private IEnumerator ClickSound()
    {
        var sfxSource = this.AddComponent<AudioSource>();
        sfxSource.clip = clickSound;
        sfxSource.loop = false;
        sfxSource.outputAudioMixerGroup = sfxGroup;
        sfxSource.Play();
        yield return new WaitForSeconds(sfxSource.clip.length);
        Destroy(sfxSource);
    }

    private void PlayMusic()
    {
        var musicSource = this.AddComponent<AudioSource>();
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.Play();
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
}