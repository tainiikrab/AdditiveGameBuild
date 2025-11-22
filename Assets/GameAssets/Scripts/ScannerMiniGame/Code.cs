using UnityEngine;

public class Code : MonoBehaviour
{
    private void Start()
    {
        Material = GetComponentInParent<PrintingMaterial>();
    }
    
    public PrintingMaterial Material { get; private set; }
}
