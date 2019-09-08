using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class GoogleIAPManager : MonoBehaviour, IStoreListener
{
    private static GoogleIAPManager instance = null;
    public static GoogleIAPManager GetInstance() { return instance; }

    private static IStoreController storeController = null;          // The Unity Purchasing system.
    private static IExtensionProvider storeExtension = null; // The store-specific Purchasing subsystems.


    public const string PRODUCT_GRBLVL3 = "grblvl3";
    public const string PRODUCT_GP1_250M = "gp1_250m";

    private Action<string> ProductBought;
    public void SubscribeToProductBoughtEvent(Action<string> funcToSub) { ProductBought += funcToSub; }
    public void UnSubscribeToProductBoughtEvent(Action<string> funcToUnsub) { ProductBought -= funcToUnsub; }

    void OnEnable()
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

        if (storeController == null)
        {
            InitializePurchasing();
        }
    }


    public Product GetProductWithID(string id)
    {
        return storeController.products.WithID(id);
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(PRODUCT_GRBLVL3, ProductType.NonConsumable);
        builder.AddProduct(PRODUCT_GP1_250M, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return storeController != null && storeExtension != null;
    }


    public void BuyProduct(string id)
    {
        switch (id)
        {
            case PRODUCT_GRBLVL3:
                BuyProductID(PRODUCT_GRBLVL3);
                break;
            case PRODUCT_GP1_250M:
                BuyProductID(PRODUCT_GP1_250M);
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
        storeExtension = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_GRBLVL3, StringComparison.Ordinal))
        {
            Executer.GetInstance().AddJob(() => 
            {
                Debug.Log("Purchased: " + args.purchasedProduct.definition.id);
                ProductBought(PRODUCT_GRBLVL3);
            });  
        }
        else if(String.Equals(args.purchasedProduct.definition.id, PRODUCT_GP1_250M, StringComparison.Ordinal))
        {

        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
