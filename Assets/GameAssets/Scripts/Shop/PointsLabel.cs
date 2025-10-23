using TMPro;
using UnityEngine;

public class PointsLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsLabel;

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;

        gm.OnPointsChanged += UpdatePointsLabel;
        UpdatePointsLabel(gm.points);
    }

    private void UpdatePointsLabel(int points)
    {
        pointsLabel.text = points.ToString();
    }
}