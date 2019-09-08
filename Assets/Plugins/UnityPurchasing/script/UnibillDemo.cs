using System.Collections.Generic;
using Unibill.Demo;
using UnityEngine;
using UnityEngine.Purchasing;

/// <summary>
/// An example of basic Unibill functionality.
/// </summary>
[AddComponentMenu("Unibill/UnibillDemo")]
public class UnibillDemo : MonoBehaviour, IStoreListener
{
    private ComboBox m_Box;
    private GUIContent[] m_ComboBoxList;
    private IStoreController m_Controller;
    private GUIStyle m_ListStyle;
    private int m_SelectedItemIndex;
    private IAppleExtensions appleExtensions;

    /// <summary>
    /// This will be called when Unibill has finished initialising.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_Controller = controller;
        appleExtensions = extensions.GetExtension<IAppleExtensions> ();

        // Amazon User ID is available, if of interest.
        Debug.Log("Amazon User ID: " + extensions.GetExtension<IAmazonExtensions>().amazonUserId);

        Debug.Log("Available items:");
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log(string.Join(" - ",
                    new[]
                    {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString
                    }));
            }
        }
    }

    /// <summary>
    /// This will be called when a purchase completes.
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
		Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
		Debug.Log("Receipt: " + e.purchasedProduct.receipt);
        // Indicate we have handled this purchase, we will not be informed of it again.x
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// This will be called is an attempted purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
    {
        Debug.Log("Purchase failed: " + item.definition.id);
        Debug.Log(r);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    public void Awake()
    {
        var module = StandardPurchasingModule.Instance();
        module.useMockBillingSystem = true;
        var builder = ConfigurationBuilder.Instance(module);

        builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2O/9/H7jYjOsLFT/uSy3ZEk5KaNg1xx60RN7yWJaoQZ7qMeLy4hsVB3IpgMXgiYFiKELkBaUEkObiPDlCxcHnWVlhnzJBvTfeCPrYNVOOSJFZrXdotp5L0iS2NVHjnllM+HA1M0W2eSNjdYzdLmZl1bxTpXa4th+dVli9lZu7B7C2ly79i/hGTmvaClzPBNyX+Rtj7Bmo336zh2lYbRdpD5glozUq+10u91PMDPH+jqhx10eyZpiapr8dFqXl5diMiobknw9CgcjxqMTVBQHK6hS0qYKPmUDONquJn280fBs1PTeA6NMG03gb9FLESKFclcuEZtvM8ZwMMRxSLA9GwIDAQAB");
        builder.AddProduct("coins", ProductType.Consumable, new IDs
        {
            {"com.outlinegames.100goldcoins.v2.c", GooglePlay.Name, AmazonApps.Name},
            {"com.outlinegames.100goldcoins.6", AppleAppStore.Name},
            {"com.outlinegames.100goldcoins.mac", MacAppStore.Name},
            {"com.outlinegames.100goldcoins.win8", WinRT.Name}
        });

        builder.AddProduct("sword", ProductType.NonConsumable, new IDs
        {
            {"com.outlinegames.sword.c", GooglePlay.Name, AmazonApps.Name},
            {"com.outlinegames.sword.6", AppleAppStore.Name},
            {"com.outlinegames.sword.mac", MacAppStore.Name},
            {"com.outlinegames.sword", WindowsPhone8.Name}
        });
        builder.AddProduct("subscription", ProductType.Subscription, new IDs
        {
            {"com.outlinegames.unibill.subscription", GooglePlay.Name, AppleAppStore.Name}
        });

        // Write out our Amazon Sandbox JSON file.
        // This has no effect when the Amazon billing service is not in use.
        builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);

        InitCombobox(builder.products);

        // Now we're ready to initialize Unibill.
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// This will be called after a call to Unibiller.restoreTransactions().
    /// </summary>
    private void OnTransactionsRestored(bool success)
    {
        Debug.Log("Transactions restored.");
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    /// 
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }

    private void InitCombobox(HashSet<ProductDefinition> items)
    {
        m_Box = new ComboBox();
        m_ComboBoxList = new GUIContent[items.Count];
        int t = 0;
        foreach (var item in items)
        {
            m_ComboBoxList[t] = new GUIContent(string.Format("{0} - {1}", item.id, item.type));
            t++;
        }

        m_ListStyle = new GUIStyle();
        m_ListStyle.normal.textColor = Color.white;
        m_ListStyle.onHover.background =
            m_ListStyle.hover.background = new Texture2D(2, 2);
        m_ListStyle.padding.left =
            m_ListStyle.padding.right =
                m_ListStyle.padding.top =
                    m_ListStyle.padding.bottom = 4;
    }

    public void Update()
    {
        if (null != m_Controller)
        {
            for (int t = 0; t < m_Controller.products.all.Length; t++)
            {
                var item = m_Controller.products.all[t];
                m_ComboBoxList[t] =
                    new GUIContent(string.Format("{0} - {1}", item.metadata.localizedTitle, item.metadata.localizedPriceString));
            }
        }
    }

    public void OnGUI()
    {
        m_SelectedItemIndex = m_Box.GetSelectedItemIndex();
        m_SelectedItemIndex = m_Box.List(new Rect(0, 0, Screen.width, Screen.width / 20.0f),
            m_ComboBoxList[m_SelectedItemIndex].text, m_ComboBoxList, m_ListStyle);
        if (null != m_Controller)
        {
            if (GUI.Button(new Rect(0, Screen.height - Screen.width / 6.0f, Screen.width / 2.0f, Screen.width / 6.0f), "Buy"))
                m_Controller.InitiatePurchase(m_Controller.products.all[m_SelectedItemIndex]);

            if (
                GUI.Button(
                           new Rect(Screen.width / 2.0f, Screen.height - Screen.width / 6.0f, Screen.width / 2.0f, Screen.width / 6.0f),
                    "Restore transactions"))
                appleExtensions.RestoreTransactions(OnTransactionsRestored);
        }

        if (null != m_Controller)
        {
            // Draw the purchase names for our various purchasables.
            int start = (int) (Screen.height - 2 * Screen.width / 6.0f) - 50;
            foreach (Product item in m_Controller.products.all)
            {
                GUI.Label(new Rect(0, start, 500, 50), item.definition.id, m_ListStyle);
                GUI.Label(new Rect(Screen.width - Screen.width * 0.1f, start, 500, 50), item.hasReceipt.ToString(),
                    m_ListStyle);
                start -= 30;
            }

            GUI.Label(new Rect(0, start - 10, 500, 50), "Item", m_ListStyle);
            GUI.Label(new Rect(Screen.width - Screen.width * 0.2f, start - 10, 500, 50), "Count", m_ListStyle);
        }
    }
}
