using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    private Color32 SELECTED_COLOR = new Color32(0, 225, 255, 225);
    private Color32 DESELECTED_COLOR = new Color32(225, 225, 225, 255);

    private StoreProduct highlightedProduct = null;

    [SerializeField] private GameObject store = null;
    [SerializeField] private GameObject disclaimerPanel = null;
    [SerializeField] private GameObject gravitonsStorePanel = null;
    [SerializeField] private ToastScript toast = null;
    public Text gravitonsText = null;
    [Header("Sprites")]
    [SerializeField] private Sprite grbSprite = null;
    [SerializeField] private Sprite shieldSprite = null;
    [SerializeField] private Sprite GPSprite = null;
    [SerializeField] private Sprite gravitonsSprite = null;
    [Header("Scroll View")]
    [SerializeField] private GameObject parentScrollView = null;
    [SerializeField] private GameObject powerUpsViewport = null;
    [SerializeField] private GameObject skinsViewport = null;
    [SerializeField] private GameObject gravityPointsViewport = null;
    private ScrollRect parentScrollRect;
    [Header("Buttons")]
    [SerializeField] private Image powerUpsButton = null;
    [SerializeField] private Image skinsButton = null;
    [SerializeField] private Image gravityPointsButton = null;

    public IAPProduct[] IAPProducts;
    public StoreProduct[] storeProducts;
    [HideInInspector] public CurrencyData currencyData;
    private PlayerSkillsData skillsData;

    private void Start()
    {
        IAPProducts = gravitonsStorePanel.GetComponentsInChildren<IAPProduct>(true);
        storeProducts = store.GetComponentsInChildren<StoreProduct>(true);

        currencyData = SaveManager.GetInstance().LoadPersistentData(SaveManager.CURRENCY_PATH).GetData<CurrencyData>();
        skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();

        gravitonsText.text = currencyData.gravitons.ToString("0");

        parentScrollRect = parentScrollView.GetComponent<ScrollRect>();
        parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(powerUpsViewport, "ViewContent");
        EnableViewport("PowerUps");

        InitializeProductsState();
    }


    public void ShowProductDisclaimerPanel(StoreProduct product)
    {
        if(!(product is IAPProduct))
        {
            Debug.Log("Inside");
            if (currencyData.gravitons < product.price)
            {
                toast.ShowToast("Not enough Gravitons", gravitonsSprite, 2f);
                return;
            }
        }
        disclaimerPanel.SetActive(true);
        highlightedProduct = product;
    }

    public void HideProductDisclaimerPanel()
    {
        disclaimerPanel.SetActive(false);
        highlightedProduct = null;
    }

    public void BuyHighlightedProduct()
    {
        if(highlightedProduct is IAPProduct)
        {
            GoogleIAPManager.GetInstance().BuyProduct(highlightedProduct.id);
        }
        else
        {    
            currencyData.gravitons -= (int)highlightedProduct.price;
            switch (highlightedProduct.id)
            {
                case "GRB4":
                    skillsData.gammaRayBurstPoints = 4;
                    SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
                    GetProductWithID("GRB4")?.SetProductState(StoreProduct.ProductState.PURCHASED);
                    toast.EnqueueToast("GRB 4 purchased", grbSprite, 2f);
                    break;
                case "MSU":
                    skillsData.magneticShieldBundleUnlocked = true;
                    SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
                    GetProductWithID("MSU")?.SetProductState(StoreProduct.ProductState.PURCHASED);
                    toast.EnqueueToast("Magnetic shield bundle purchased", shieldSprite, 1.5f);
                    break;
                case "1_5M":
                    currencyData.gravityPoints += 1500000;
                    SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
                    toast.EnqueueToast("1 500 000 Gravity Points purchased", GPSprite, 1.5f);
                    break;
                case "2_5M":
                    currencyData.gravityPoints += 2500000;
                    SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
                    toast.EnqueueToast("2 500 000 Gravity Points purchased", GPSprite, 1.5f);
                    break;
                case "4M":
                    currencyData.gravityPoints += 4000000;
                    SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
                    toast.EnqueueToast("4 000 000 Gravity Points purchased", GPSprite, 1.5f);
                    break;
            }
            gravitonsText.text = currencyData.gravitons.ToString();
        }
        disclaimerPanel.SetActive(false);
        highlightedProduct = null;
    }

    public void ShowGravitonsStore(bool state)
    {
        gravitonsStorePanel.SetActive(state);
    }

    public void EnableViewport(string viewPortId)
    {
        switch (viewPortId)
        {
            case "PowerUps":
                powerUpsViewport.gameObject.SetActive(true);
                skinsViewport.gameObject.SetActive(false);
                gravityPointsViewport.gameObject.SetActive(false);
                parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(powerUpsViewport, "ViewContent");
                powerUpsButton.color = SELECTED_COLOR;
                skinsButton.color = DESELECTED_COLOR;
                gravityPointsButton.color = DESELECTED_COLOR;
                break;

            case "Skins":
                powerUpsViewport.gameObject.SetActive(false);
                skinsViewport.gameObject.SetActive(true);
                gravityPointsViewport.gameObject.SetActive(false);
                parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(skinsViewport, "ViewContent");
                powerUpsButton.color = DESELECTED_COLOR;
                skinsButton.color = SELECTED_COLOR;
                gravityPointsButton.color = DESELECTED_COLOR;
                break;

            case "GravityPoints":
                powerUpsViewport.gameObject.SetActive(false);
                skinsViewport.gameObject.SetActive(false);
                gravityPointsViewport.gameObject.SetActive(true);
                parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(gravityPointsViewport, "ViewContent");
                powerUpsButton.color = DESELECTED_COLOR;
                skinsButton.color = DESELECTED_COLOR;
                gravityPointsButton.color = SELECTED_COLOR;
                break;
        }
    }

    private StoreProduct GetProductWithID(string id)
    {
        foreach(StoreProduct prod in storeProducts)
        {
            if (prod.id == id) return prod;
        }
        return null;
    }

    private void InitializeProductsState()
    {
        if(skillsData.gammaRayBurstPoints == PlayerSkillsData.GRB_MAX_POINTS)
        {
            GetProductWithID("GRB4")?.SetProductState(StoreProduct.ProductState.PURCHASED);
        }
        else if(skillsData.gammaRayBurstPoints < PlayerSkillsData.GRB_MAX_POINTS - 1)
        {
            GetProductWithID("GRB4")?.SetProductState(StoreProduct.ProductState.INACTIVE);
        }
        if(skillsData.magneticShieldBundleUnlocked)
        {
            GetProductWithID("MSU")?.SetProductState(StoreProduct.ProductState.PURCHASED);
        }
    }
}