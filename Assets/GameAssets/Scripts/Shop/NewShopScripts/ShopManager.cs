using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public List<SceneObject> SceneObjects => sceneObjects;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;

        ParseOffers();
        sceneObjects = FindObjectsByType<SceneObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        foreach (var sceneObject in sceneObjects)
            Debug.Log(sceneObject.name);

        ShowPurchasedSceneObjects(sceneObjects);
    }

    /// <summary>
    /// Чит для фарма монет
    /// </summary>
    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            GameManager.Instance.points += 1000;
        }
    }

    /// <summary>
    /// Словарь, где хранятся товары, отсортированные по категориям. 
    /// </summary>
    public Dictionary<string, List<ShopItemConfig>> offers;

    /// <summary>
    /// Список объектов сцены, которые можно приобрести в магазине
    /// </summary>
    private List<SceneObject> sceneObjects;

    /// <summary>
    /// Базовое событие, которое вызывается при покупке любого товара
    /// </summary>
    public event Action OnPurchase;

    /// <summary>
    /// Событие, которое вызывается при покупке товара - объекта сцены
    /// </summary>
    public event Action OnPurchaseSceneObject;

    /// <summary>
    /// Категории товаров
    /// </summary>
    public enum OfferCategory
    {
        Device,
        Interior,
        Worker
    }

    /// <summary>
    /// Активируем ранее купленные объекты
    /// </summary>
    private void ShowPurchasedSceneObjects(List<SceneObject> sceneObjects)
    {
        foreach (var sceneObject in sceneObjects)
        {
            if (SaveManager.gameData.purchasedOffers.Contains(sceneObject.SceneObjectId))
            {
                sceneObject.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Покупка товара
    /// </summary>
    /// <param name="offer"></param>
    public void Purchase(ShopItemConfig offer)
    {
        if (IsSceneObject(offer))
        {
            var sObj = sceneObjects.Find(x => x.SceneObjectId == offer.id);
            OnPurchaseSceneObject?.Invoke();
            AnimationSceneObject(sObj);
        }
        GameManager.Instance.points -= offer.price;
        SaveManager.gameData.purchasedOffers.Add(offer.id);
        OnPurchase?.Invoke();
    }

    /// <summary>
    /// Анимация купленного объекта сцены
    /// </summary>
    private static void AnimationSceneObject(SceneObject obj)
    {
        obj.gameObject.SetActive(true);

        var finalPosition = obj.transform.position;
        obj.transform.position = finalPosition + Vector3.up * 1f;

        obj.transform.DOMove(finalPosition, 1f)
            .SetEase(Ease.OutBounce);
    }

    /// <summary>
    /// Проверка, является ли товар объектом сцены
    /// </summary>
    /// <param name="offer"></param>
    /// <returns></returns>
    private bool IsSceneObject(ShopItemConfig offer)
    {
        var sceneObject = sceneObjects.Find(element => element.SceneObjectId == offer.id);
        return sceneObject != null;
    }

    /// <summary>
    /// Парсинг товаров из глобальной конфигурации и сортировка их по категориям
    /// </summary>
    private void ParseOffers()
    {
        offers = new Dictionary<string, List<ShopItemConfig>>();

        foreach (var offer in GlobalConfig.Instance.ShopItems)
        {
            if (!offers.ContainsKey(offer.category))
            {
                offers.Add(offer.category, new List<ShopItemConfig>());
            }
            offers[offer.category].Add(offer);
        }
    }
}
