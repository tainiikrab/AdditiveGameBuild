using UnityEngine;

public class LaptopTrigger : MonoBehaviour, IRaycastInteractable
{
    [SerializeField] private Light hoverLight;
    private float lightIntensity = 3f;
    [SerializeField] private LaptopUI laptopUI;

    [SerializeField] private float blinkSpeed = 2f;

    private void Awake()
    {
        if (!hoverLight.gameObject.activeSelf) hoverLight.gameObject.SetActive(true);
        lightIntensity = hoverLight.intensity;
        hoverLight.intensity = 0;
    }

    public void OnHoverEnter()
    {
        tempBlinking = false;
        hoverLight.intensity = lightIntensity;
    }

    public void OnHoverExit()
    {
        hoverLight.intensity = 0;
        tempBlinking = true;
    }

    public void OnClick()
    {
        laptopUI.ToggleVisibility(true);
    }

    public bool isBlinking = false;
    public bool tempBlinking = true;

    public void Update()
    {
        if (isBlinking && tempBlinking) hoverLight.intensity = Mathf.PingPong(Time.time * blinkSpeed, lightIntensity);
    }
}