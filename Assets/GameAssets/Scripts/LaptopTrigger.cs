using UnityEngine;

public class LaptopTrigger : MonoBehaviour, IRaycastInteractable
{
    [SerializeField] private Light hoverLight;
    [SerializeField] private float lightIntensity = 3f;
    [SerializeField] private LaptopUI laptopUI;

    private void Awake()
    {
        hoverLight.intensity = 0;
    }

    public void OnHoverEnter()
    {
        hoverLight.intensity = lightIntensity;
    }

    public void OnHoverExit()
    {
        hoverLight.intensity = 0;
    }

    public void OnClick()
    {
        laptopUI.gameObject.SetActive(true);
        laptopUI.ToggleVisibility(true);
    }
}