using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slicer : MonoBehaviour
{
    [SerializeField] private SliderValues layerHeight;
    [SerializeField] private SliderValues fillDensity;
    [SerializeField] private SliderValues printSpeed;
    [SerializeField] private Button printButton;
    [SerializeField] private ModelTurner modelTurner;

    [SerializeField] private TextMeshProUGUI qualityLabel;
    private GameManager gm;
    private Action<OrderConfig> onCompletedHandler;
    private OrderConfig order;

    [SerializeField] private Material slicerMaterial;

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        SetupSlider(layerHeight);
        SetupSlider(fillDensity);
        SetupSlider(printSpeed);
        OrderManager.OnOrderAccepted += SetOrder;
        onCompletedHandler = _ => SetOrder();
        OrderManager.OnOrderCompleted += onCompletedHandler;

        SetOrder();
    }

    private void OnDestroy()
    {
        OrderManager.OnOrderAccepted -= SetOrder;
        OrderManager.OnOrderCompleted -= onCompletedHandler;
    }

    private MeshRenderer modelRenderer;

    private void SetOrder()
    {
        if (OrderManager.orderData
            == null)
        {
            printButton.interactable = false;
            modelTurner.GetComponent<RawImage>().enabled = false;
            modelTurner.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            order = OrderManager.orderData.config;
            modelTurner.SetModel(order.mesh);
            modelRenderer = ModelTurner.turningModel.GetComponent<MeshRenderer>();
            modelRenderer.material = slicerMaterial;
            printButton.onClick.AddListener(StartPrinting);
        }

        AudioManager.Instance.PlayClickSound();
    }

    private void StartPrinting()
    {
        var quality = OrderManager.orderData.quality;
        quality.fillDensity = 100 - Mathf.Pow(Mathf.Abs(fillDensity.value - order.fillDensity), 1f);
        quality.layerHeight = 100 - Mathf.Pow(Mathf.Abs(layerHeight.value - order.layerHeight), 1f);
        quality.printSpeed = 100 - Mathf.Pow(Mathf.Abs(printSpeed.value - order.printSpeed), 1f);
        Debug.Log($"Order quality: {OrderManager.orderData.quality.totalQuality}");

        OrderManager.orderData.LoadNextMinigame();

        AudioManager.Instance.PlayClickSound();
    }

    private void SetupSlider(SliderValues values)
    {
        values.slider.minValue = values.minValue;
        values.slider.maxValue = values.maxValue;
        values.slider.value = values.minValue;

        values.slider.onValueChanged.AddListener(value => UpdateSlider(values, value));
        UpdateSlider(values, values.maxValue / 2);
    }


    private MaterialPropertyBlock block;

    [Header("Sliced material settings")] [SerializeField]
    private float matLayerHeightMin = 0.01f;

    [SerializeField] private float matLayerHeightMax = 0.05f;
    [SerializeField] private float matPrintProgressMin = -8.3f;
    [SerializeField] private float matPrintProgressMax = -7.3f;
    [SerializeField] private float matMinDistortion = 0f;
    [SerializeField] private float matMaxDistortion = 0.023f;
    [SerializeField] private float matMinGap = 0.2f;
    [SerializeField] private float matMaxGap = 0.75f;

    private void UpdateModel()
    {
        if (modelRenderer == null)
        {
            Debug.LogWarning($"idk why but {nameof(modelRenderer)} is null");
            return;
        }

        // block.SetFloat("_PrintProgress", Time.time % 10f);
        block.SetFloat("_LayerHeight",
            Map(layerHeight.value, layerHeight.minValue, layerHeight.maxValue, matLayerHeightMin, matLayerHeightMax));
        block.SetFloat("_DistortionAmount",
            Map(printSpeed.value, printSpeed.minValue, printSpeed.maxValue, matMinDistortion, matMaxDistortion));
        block.SetFloat("_SliceGap",
            Map(fillDensity.value, fillDensity.minValue, fillDensity.maxValue, matMaxGap, matMinGap));

        modelRenderer.SetPropertyBlock(block);
    }

    private void UpdateSlider(SliderValues values, float rawValue)
    {
        if (values.steps <= 0)
        {
            values.slider.SetValueWithoutNotify(rawValue);
            values.label.text = $"{values.labelPrefix}{rawValue:F2}";
            return;
        }

        var stepSize = (values.maxValue - values.minValue) / values.steps;
        var snapped = Mathf.Round((rawValue - values.minValue) / stepSize) * stepSize + values.minValue;

        values.slider.SetValueWithoutNotify(snapped);
        values.label.text = $"{values.labelPrefix}{snapped:F2}{values.labelPostfix}";

        UpdateModel();
    }

    [Serializable]
    public class SliderValues
    {
        public Slider slider;
        public TextMeshProUGUI label;
        public int steps = 10;
        public float minValue;
        public float maxValue;
        private string _labelPostfix;
        private string _labelPrefix;

        public float value => slider.value;

        public string labelPrefix
        {
            get
            {
                if (_labelPrefix == null) _labelPrefix = label.text.Split("_")[0];
                return _labelPrefix;
            }
        }

        public string labelPostfix
        {
            get
            {
                // Debug.Log(label.text.Split("_"));
                if (_labelPostfix == null) _labelPostfix = label.text.Split("_")[1];
                return _labelPostfix;
            }
        }
    }

    public static float Map(float x, float a, float b, float c, float d)
    {
        return c + (x - a) * (d - c) / (b - a);
    }
}