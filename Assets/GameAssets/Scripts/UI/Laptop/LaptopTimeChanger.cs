using TMPro;
using UnityEngine;

public class LaptopTimeChanger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeLabel;

    // Static: survives across scene loads, doesn't need a GameObject
    private static float s_startTime = -1f;
    private static bool s_hasBeenInitialized = false;

    private void OnEnable()
    {
        if (!s_hasBeenInitialized)
        {
            s_startTime = Time.realtimeSinceStartup;
            s_hasBeenInitialized = true;
        }

        RefreshDisplay();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(RefreshDisplay));
    }

    private void RefreshDisplay()
    {
        if (!s_hasBeenInitialized || timeLabel == null) return;

        var elapsed = Time.realtimeSinceStartup - s_startTime;
        var elapsedSeconds = Mathf.FloorToInt(elapsed);
        var displayHour = 9 + elapsedSeconds / 60;
        var displayMinute = elapsedSeconds % 60;
        timeLabel.text = $"{displayHour:D2}:{displayMinute:D2}";

        if (isActiveAndEnabled) Invoke(nameof(RefreshDisplay), 1f);
    }
}