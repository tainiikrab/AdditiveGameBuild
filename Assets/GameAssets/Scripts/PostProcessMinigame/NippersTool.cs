using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NippersTool : AbstractTool
{
    [Header("Cut Settings")] [SerializeField]
    private LayerMask supportLayer;

    [SerializeField] private float rayDistance = 1f;
    [SerializeField] private Animation nippersAnimation;
    [SerializeField] private string cutAnimName = "Nippers";

    public static bool isCutting { get; private set; }
    private SupportModel supportModel;
    [SerializeField] private float waitTime = 0.65f;

    protected override void OnActiveInstrument()
    {
        // if (isCutting) return;

        if (Physics.Raycast(transform.position, transform.forward, out var hit, rayDistance, supportLayer))
        {
            var support = hit.collider.GetComponent<SupportModel>();
            if (support != null)
            {
                isCutting = true;
                // Debug.Log("Cutting");
                if (supportModel == null || supportModel != support) OnTargetChanged?.Invoke();

                supportModel = support;
                return;
            }
        }

        isCutting = false;
    }

    public static event Action OnNippersUse;
    public static event Action OnTargetChanged;

    protected override void OnUse()
    {
        if (!isCutting) return;
        nippersAnimation.Play(cutAnimName);
        OnNippersUse?.Invoke();

        Invoke(nameof(FinishCut), waitTime);
    }

    protected override void OnStopActiveInstrument()
    {
        isCutting = false;
    }

    private void FinishCut()
    {
        if (supportModel != null)
        {
            audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
            supportModel.Fall();
            supportModel = null;
        }

        isCutting = false;
    }
}