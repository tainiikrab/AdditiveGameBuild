using System;

[Serializable]
public class VolumeData
{
    private const float DefaultMusicVolume = 0.7f;
    private const float DefaultSfxVolume = 0.7f;
    
    public VolumeData()
    {
        MusicVolume = DefaultMusicVolume;
        SfxVolume = DefaultSfxVolume;
    }
    
    public float MusicVolume
    { 
        get => musicVolume;
        set
        {
            if (!float.IsNaN(value)) musicVolume = Math.Clamp(value, float.Epsilon, 1);
        }
    }

    public float SfxVolume
    {
        get => sfxVolume;
        set
        {
            if (!float.IsNaN(value)) sfxVolume = Math.Clamp(value, float.Epsilon, 1);
        }
        
    }
    
    private float musicVolume;
    private float sfxVolume;
    
    public void Reset()
    {
        MusicVolume = DefaultMusicVolume;
        SfxVolume = DefaultSfxVolume;
    }
}
