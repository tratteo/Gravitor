using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class GoogleIAPManager : MonoBehaviour, IStoreListener
{
    private static GoogleIAPManager instance = null;
    public static GoogleIAPManager GetInstance() { return instance; }

    private static IStoreController storeController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.


    public const string PRODUCT_GRBLVL3 = "grb_lvl3";

    private Action<string> ProductBought;
    public void SubscribeToProductBoughtEvent(Action<string> funcToSub) { ProductBought += funcToSub; }
    public void UnSubscribeToProductBoughtEvent(Action<string> funcToUnsub) { ProductBought -= funcToUnsub; }

    private void Awake()
    {
        if(instance == null) 
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

    void Start()
    {
        if (storeController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(PRODUCT_GRBLVL3, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return storeController != null && m_StoreExtensionProvider != null;
    }


    public void BuyProduct(string id)
    {
        switch (id)
        {
            case PRODUCT_GRBLVL3:
                BuyProductID(PRODUCT_GRBLVL3);
                break;
            default:
                Debug.Log("ERROR PRODUCT NOT FOUND");
                break;
        }
        
    }


    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {

            Product product = storeController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                storeController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        storeController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_GRBLVL3, StringComparison.Ordinal))
        {
            Debug.Log("Purchased: " + args.purchasedProduct.definition.id);
            ProductBought(PRODUCT_GRBLVL3);
            PlayerSkillsData skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();
            skillsData.gammaRayBurstPoints = PlayerSkillsData.GRB_MAX_POINTS;
            SaveManager.GetInstance().SavePersistentData<PlayerSkillsData>(skillsData, SaveManager.SKILLSDATA_PATH);
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
