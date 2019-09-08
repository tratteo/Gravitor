using UnityEngine;
using UnityEngine.Purchasing;

public class InventoryDemo : MonoBehaviour, IStoreListener
{
    private IStoreController controller;

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        foreach (var p in this.controller.products.all)
            Debug.Log(p.metadata.localizedTitle);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
    }

    // Use this for initialization
    private void Start()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        TextAsset text = ((TextAsset) Resources.Load("inventory"));
        if (null == text)
        {
            Debug.LogError("No inventory defined!");
            return;
        }

        new JSONInventoryParser().Parse(text.text, builder);
        UnityPurchasing.Initialize(this, builder);
    }
}
