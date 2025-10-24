using System;
using System.Collections.Generic;

[Serializable]
public class GlobalConfig
{
    public List<TutorialConfig> Tutorial;
    public List<OrderConfig> Orders;
    public List<CustomerConfig> Customers;
    public List<ShopItemConfig> ShopItems;

    private static GlobalConfig _instance;

    public static GlobalConfig Instance
    {
        get
        {
            if (_instance == null) _instance = ConfigManager.GetGlobalConfig();
            return _instance;
        }
    }
}