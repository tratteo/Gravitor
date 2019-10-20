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
    [SerializeField] private ToastScript iconToast = null;
    [SerializeField] private ToastScript toast = null;
    private GameObject[] skinsPreviews = null;
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

    public StoreProduct[] storeProducts;
    [HideInInspector] public CurrencyData currencyData;
    private PlayerSkillsData skillsData;
    private PlayerAspectData aspectData;

    private void Start()
    {
        storeProducts = store.GetComponentsInChildren<StoreProduct>(true);

        List<GameObject> list = SharedUtilities.GetInstance().GetGameObjectsInChildrenWithTag(store, "Preview");
        skinsPreviews = list.ToArray();

        currencyData = SaveManager.GetInstance().LoadPersistentData(SaveManager.CURRENCY_PATH).GetData<CurrencyData>();
        aspectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ASPECTDATA_PATH).GetData<PlayerAspectData>();
        skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();

        gravitonsText.text = currencyData.gravitons.ToString("0");

        parentScrollRect = parentScrollView.GetComponent<ScrollRect>();
        parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(powerUpsViewport, "ViewContent");
        EnableViewport("PowerUps");

        InitializeProductsState();
    }

    private void FixedUpdate()
    {
        foreach(GameObject obj in skinsPreviews)
        {
            obj.transform.Rotate(new Vector3(0f, 1f, 0f) * 0.35f);
        }
    }

    public void AspectSelected(StoreProduct product)
    {
        if(!aspectData.IsAspectUnlocked(product.id))
        {
            ShowProductDisclaimerPanel(product);
        }
        else
        {
            GetProductWithID(aspectData.equippedSkinId)?.SetProductState(StoreProduct.ProductState.PURCHASED);
            aspectData.equippedSkinId = product.id;
            SaveManager.GetInstance().SavePersistentData(aspectData, SaveManager.ASPECTDATA_PATH);
            product.SetProductState(StoreProduct.ProductState.CONSUMED);
            Debug.Log(aspectData.equippedSkinId);
        }
    }

    public void ShowProductDisclaimerPanel(StoreProduct product)
    {
        if(!(product is IAPProduct) && currencyData.gravitons < product.price)
        {
            iconToast.ShowToast("Not enough Gravitons", gravitonsSprite, 2f);
            return;
        }
        disclaimerPanel.SetActive(true);
        highlightedProduct = product;
    }

    public void HideProductDisclaimerPanel()
    {
        disclaimerPanel.SetActive(false);
        highlightedProduct = null;
    }

    public void BuyIAPPProduct(IAPProduct product)
    {
        Executer.GetInstance().AddJob(() =>
        {
            GoogleIAPManager.GetInstance().BuyProduct(product.id);
        });
    }

    public void BuyHighlightedProduct()
    { 
        currencyData.gravitons -= (int)highlightedProduct.price;
        switch (highlightedProduct.type)
        {
            case StoreProduct.ProductType.GRAVITY_POINT:
                currencyData.gravityPoints += highlightedProduct.amount;
                iconToast.EnqueueToast(highlightedProduct.amount + " Gravity Points purchased", GPSprite, 1.5f);
                break;

            case StoreProduct.ProductType.ASPECT:
                aspectData.UnlockAspect(highlightedProduct.id);
                SaveManager.GetInstance().SavePersistentData(aspectData, SaveManager.ASPECTDATA_PATH);
                GetProductWithID(highlightedProduct.id)?.SetProductState(StoreProduct.ProductState.PURCHASED);
                toast.ShowToast("Aspect purchased: " + aspectData.AspectStringFromId(highlightedProduct.id), null, 2f);
                break;

            case StoreProduct.ProductType.POWER_UP:

                if(highlightedProduct.id.Equals("GRB4"))
                {
                    skillsData.gammaRayBurstPoints = 4;
                    SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
                    GetProductWithID("GRB4")?.SetProductState(StoreProduct.ProductState.CONSUMED);
                    iconToast.EnqueueToast("GRB 4 purchased", grbSprite, 2f);
                }
                else if(highlightedProduct.id.Equals("MSU"))
                {
                    skillsData.magneticShieldBundleUnlocked = true;
                    SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
                    GetProductWithID("MSU")?.SetProductState(StoreProduct.ProductState.CONSUMED);
                    iconToast.EnqueueToast("Magnetic shield bundle purchased", shieldSprite, 1.5f);
                }
                break;
            default:
                currencyData.gravitons += (int)highlightedProduct.price;
                break;
        }
        SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
        gravitonsText.text = currencyData.gravitons.ToString();
        disclaimerPanel.SetActive(false);
        highlightedProduct = null;
    }

    public void ShowGravitonsStore(bool state)
    {
        gravitonsStorePanel.SetActive(state);
        parentScrollRect.content.gameObject.SetActive(!state);
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
            GetProductWithID("GRB4")?.SetProductState(StoreProduct.ProductState.CONSUMED);
        }
        else if(skillsData.gammaRayBurstPoints < PlayerSkillsData.GRB_MAX_POINTS - 1)
        {
            GetProductWithID("GRB4")?.SetProductState(StoreProduct.ProductState.INACTIVE);
        }

        if(skillsData.magneticShieldBundleUnlocked)
        {
            GetProductWithID("MSU")?.SetProductState(StoreProduct.ProductState.CONSUMED);
        }

        foreach(StoreProduct product in storeProducts)
        {
            if(product.type == StoreProduct.ProductType.ASPECT)
            {
                if(aspectData.IsAspectUnlocked(product.id))
                {
                    product.SetProductState(StoreProduct.ProductState.PURCHASED);
                }
            }
        }
        GetProductWithID(aspectData.equippedSkinId)?.SetProductState(StoreProduct.ProductState.CONSUMED);
    }
}