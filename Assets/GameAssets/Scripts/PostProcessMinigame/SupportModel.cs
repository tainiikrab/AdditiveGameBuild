using UnityEngine;

public class SupportModel : MonoBehaviour
{
    public void Fall()
    {
        PostprocessMinigame.removedSupports++;
        Destroy(gameObject);
    }
}