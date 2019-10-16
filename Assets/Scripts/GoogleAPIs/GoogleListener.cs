using System;
using UnityEngine;

public class GoogleListener : MonoBehaviour
{
    private static GoogleListener instance = null;

    public static GoogleListener GetInstance() { return instance; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }


    private void OnEnable()
    {
        GoogleAdsManager.GetInstance().SubscribeToRewardClaimed(AdRewardEarned);
        GoogleIAPManager.GetInstance().SubscribeToProductPurchased(IapProductPurchased);
    }

    private void OnDisable()
    {
        GoogleAdsManager.GetInstance().UnsubscribeToRewardClaimed(AdRewardEarned);
        GoogleIAPManager.GetInstance().UnsubscribeToProductPurchased(IapProductPurchased);
    }

    private void AdRewardEarned(string id)
    {
        switch(id)
        {
            case GoogleAdsManager.ATTEMPT_ID:
                Executer.GetInstance().AddJob(() =>
                {
                    FindObjectOfType<GameMode>().Attempt();
                });
                break;
            case "coins":
                Executer.GetInstance().AddJob(() =>
                {
                    FindObjectOfType<MenuManager>()?.TimedRewardedEarned();
                });
                break;
            default:
                break;
            case GoogleAdsManager.TIMED_REWARD_ID:
                Executer.GetInstance().AddJob(() =>
                {
                    FindObjectOfType<MenuManager>().TimedRewardedEarned();
                });
                break;
        }
    }

    private void IapProductPurchased(string id)
    {
        Executer.GetInstance().AddJob(() =>
        {
           CurrencyData currencyData = SaveManager.GetInstance().LoadPersistentData(SaveManager.CURRENCY_PATH).GetData<CurrencyData>();
           switch (id)
           {
               case GoogleIAPManager.PRODUCT_500_GR:
                   currencyData.gravitons += 500;
                   break;
               case GoogleIAPManager.PRODUCT_750_GR:
                   currencyData.gravitons += 750 + 100;
                   break;
               case GoogleIAPManager.PRODUCT_1000_GR:
                   currencyData.gravitons += 1000 + 200;
                   break;
               case GoogleIAPManager.PRODUCT_2000_GR:
                   currencyData.gravitons += 2000 + 350;
                   break;
               default:
                   break;
           }
           StoreManager manager = FindObjectOfType<StoreManager>();
           manager.gravitonsText.text = currencyData.gravitons.ToString();
           manager.currencyData = currencyData;
           SaveManager.GetInstance().SavePersistentData<CurrencyData>(currencyData, SaveManager.CURRENCY_PATH);
       });
    }
}
