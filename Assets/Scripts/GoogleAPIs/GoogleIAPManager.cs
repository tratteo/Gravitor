using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class GoogleIAPManager : MonoBehaviour, IStoreListener
{
    private static GoogleIAPManager instance = null;
    public static GoogleIAPManager GetInstance() { return instance; }

    private static IStoreController storeController = null;          // The Unity Purchasing system.
    private static IExtensionProvider storeExtension = null; // The store-specific Purchasing subsystems.

    public const string PRODUCT_500_GR = "500_gr";
    public const string PRODUCT_750_GR = "750_gr";
    public const string PRODUCT_1000_GR = "1000_gr";
    public const string PRODUCT_2000_GR = "2000_gr";

    private Action<string> ProductPurchased;
    public void SubscribeToProductPurchased(Action<string> funcToSub) { ProductPurchased += funcToSub; }
    public void UnsubscribeToProductPurchased(Action<string> funcToUnsub) { ProductPurchased -= funcToUnsub; }

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

        builder.AddProduct(PRODUCT_500_GR, ProductType.Consumable);
        builder.AddProduct(PRODUCT_750_GR, ProductType.Consumable);
        builder.AddProduct(PRODUCT_1000_GR, ProductType.Consumable);
        builder.AddProduct(PRODUCT_2000_GR, ProductType.Consumable);

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
            case PRODUCT_500_GR:
                break;
            case PRODUCT_750_GR:
                break;
            case PRODUCT_1000_GR:
                break;
            case PRODUCT_2000_GR:
                break;
            default:
                Debug.Log("ERROR PRODUCT NOT FOUND");
                break;
        }

        BuyProductID(id);
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
        if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_500_GR, StringComparison.Ordinal))
        {
            Executer.GetInstance().AddJob(() =>
            {
                Debug.Log("Purchased: " + args.purchasedProduct.definition.id);
                ProductPurchased?.Invoke(PRODUCT_500_GR);
            });
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_750_GR, StringComparison.Ordinal))
        {
            Executer.GetInstance().AddJob(() =>
            {
                Debug.Log("Purchased: " + args.purchasedProduct.definition.id);
                ProductPurchased?.Invoke(PRODUCT_750_GR);
            });
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_1000_GR, StringComparison.Ordinal))
        {
            Executer.GetInstance().AddJob(() =>
            {
                Debug.Log("Purchased: " + args.purchasedProduct.definition.id);
                ProductPurchased?.Invoke(PRODUCT_1000_GR);
            });
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_2000_GR, StringComparison.Ordinal))
        {
            Executer.GetInstance().AddJob(() =>
            {
                Debug.Log("Purchased: " + args.purchasedProduct.definition.id);
                ProductPurchased?.Invoke(PRODUCT_2000_GR);
            });
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
