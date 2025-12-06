using UnityEngine;

public class NippersTool : AbstractTool
{
    [Header("Cut Settings")] [SerializeField]
    private LayerMask supportLayer;

    [SerializeField] private float rayDistance = 1f;
    [SerializeField] private Animation nippersAnimation;
    [SerializeField] private string cutAnimName = "Nippers";

    private bool isCutting;
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
                Debug.Log("Cutting");
                supportModel = support;
                return;
            }
        }

        isCutting = false;
    }

    protected override void OnUse()
    {
        if (!isCutting) return;
        nippersAnimation.Play(cutAnimName);
        Invoke(nameof(FinishCut), waitTime);
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