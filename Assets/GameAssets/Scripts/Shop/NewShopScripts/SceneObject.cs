using UnityEngine;

public class SceneObject : MonoBehaviour
{
    [SerializeField] private ShopManager.OfferCategory category;
    [SerializeField] private string sceneObjectId;
    
    public ShopManager.OfferCategory Category => category;
    public string SceneObjectId => sceneObjectId;
}
