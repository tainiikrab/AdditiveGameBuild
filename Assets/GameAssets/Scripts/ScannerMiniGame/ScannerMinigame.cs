using System;
using UnityEngine;

public class ScannerMinigame : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayMusic(MusicType.BackgroundMusic);
    }
}
