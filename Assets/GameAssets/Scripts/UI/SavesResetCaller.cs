using UnityEngine;

public class SavesResetCaller : MonoBehaviour
{
    public void ResetSaves()
    {
        SaveManager.ResetSaves();
        AudioManager.Instance.PlaySound(SoundType.UniversalClick);
    }
}