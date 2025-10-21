using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Drag here the sliders from volume settings")] [Header("Music slider")] [SerializeField]
    private Slider musicSlider;

    [Header("SFX slider")] [SerializeField]
    private Slider sfxSlider;

    [Header("Default slider values")] [Range(0, 1)]
    public float musicDefaultVolume;

    [Range(0, 1)] public float sfxDefaultVolume;

    public AudioMixer audioMixer;

    // [Space(50)]
    public AudioSource[] clickSounds;

    // [Space(50)] 
    public float delay;
    public AudioSource[] musicPlaylist;
    private int _currentMusicIndex;

    private const string MUSIC_VOLUME_KEY = "musicVolume";
    private const string SFX_VOLUME_KEY = "sfxVolume";

    private void Awake()
    {
        if (Instance == null) Instance = this;

        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    public void Start()
    {
        if (Instance != this) return;

        if (!PlayerPrefs.HasKey(MUSIC_VOLUME_KEY) || !PlayerPrefs.HasKey(SFX_VOLUME_KEY))
        {
            musicSlider.value = musicDefaultVolume;
            sfxSlider.value = sfxDefaultVolume;
            SaveVolumeValues();
        }
        else
        {
            LoadVolumeValues();
        }

        PlayMusicList();
    }

    public void PlayClickSound()
    {
        if (clickSounds.Length == 0 || clickSounds == null) return;
        var randomInt = Random.Range(0, clickSounds.Length);
        clickSounds[randomInt].Play();
    }

    private void PlayMusicList()
    {
        if (musicPlaylist.Length == 0 || musicPlaylist == null) return;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        while (true)
        {
            var currentAudioSource = musicPlaylist[_currentMusicIndex];
            //currentAudioSource.volume = musicSlider.value;
            currentAudioSource.Play();

            yield return new WaitForSeconds(musicPlaylist[_currentMusicIndex].clip.length);

            yield return new WaitForSeconds(delay);

            _currentMusicIndex = (_currentMusicIndex + 1) % musicPlaylist.Length;
        }
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Max(float.Epsilon, value)) * 20);
        SaveVolumeValues();
    }

    public void SetSfxVolume(float value)
    {
        audioMixer.SetFloat("soundEffectsVolume", Mathf.Log10(Mathf.Max(float.Epsilon, value)) * 20);
        SaveVolumeValues();
    }

    private void LoadVolumeValues()
    {
        if (musicSlider == null || sfxSlider == null) return;
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);

        // SetMusicVolume(musicSlider.value);
        // SetSfxVolume();
    }

    private void SaveVolumeValues()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicSlider.value);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxSlider.value);
    }
}