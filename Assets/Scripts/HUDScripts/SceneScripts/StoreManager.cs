using System;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    private ProductInfo highlightedProduct = null;

    [SerializeField] private GameObject UICanvas = null;
    [SerializeField] private GameObject disclaimerPanel = null;
    [SerializeField] private ToastScript toast = null;

    private PlayerData playerData;
    private PlayerSkillsData skillsData;

    private ProductInfo[] products;

    private void OnDisable()
    {
        GoogleIAPManager.GetInstance().UnSubscribeToProductBoughtEvent(ProductBought);
    }

    private void Start()
    {
        GoogleIAPManager.GetInstance().SubscribeToProductBoughtEvent(ProductBought);

        products = UICanvas.GetComponentsInChildren<ProductInfo>();

        playerData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();
        skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();
      
        if (playerData.playerState != PlayerManager.PlayerState.COMET || 
            Application.internetReachability == NetworkReachability.NotReachable)
        {
            GetProductByID(GoogleIAPManager.PRODUCT_GRBLVL3).SetProductState(ProductInfo.ProductState.INACTIVE);
        }
        else if( skillsData.gammaRayBurstPoints >= PlayerSkillsData.GRB_MAX_POINTS)
        {
            GetProductByID(GoogleIAPManager.PRODUCT_GRBLVL3).SetProductState(ProductInfo.ProductState.PURCHASED);
        }
        else
        {
            GetProductByID(GoogleIAPManager.PRODUCT_GRBLVL3).SetProductState(ProductInfo.ProductState.AVAILABLE);
        }

        GetProductByID(GoogleIAPManager.PRODUCT_GP1_250M).SetProductState(ProductInfo.ProductState.AVAILABLE);
    }

    public void ShowProductDisclaimerPanel(ProductInfo product)
    {
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
        GoogleIAPManager.GetInstance().BuyProduct(highlightedProduct.id);
        disclaimerPanel.SetActive(false);
        highlightedProduct = null;
    }

    private void ProductBought(string id)
    {
        switch (id)
        {
            case GoogleIAPManager.PRODUCT_GRBLVL3:

                PlayerSkillsData skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();
                skillsData.gammaRayBurstPoints = PlayerSkillsData.GRB_MAX_POINTS;
                SaveManager.GetInstance().SavePersistentData<PlayerSkillsData>(skillsData, SaveManager.SKILLSDATA_PATH);

                GetProductByID(GoogleIAPManager.PRODUCT_GRBLVL3).SetProductState(ProductInfo.ProductState.PURCHASED);

                toast.EnqueueToast("GRB level 3 purchased", null, 2.5f);
                break;

            case GoogleIAPManager.PRODUCT_GP1_250M:

                int gravityPoints = SaveManager.GetInstance().LoadPersistentData(SaveManager.GRAVITYPOINTS_PATH).GetData<int>();
                gravityPoints += 1250000;
                SaveManager.GetInstance().SavePersistentData<int>(gravityPoints, SaveManager.GRAVITYPOINTS_PATH);

                GetProductByID(GoogleIAPManager.PRODUCT_GRBLVL3).SetProductState(ProductInfo.ProductState.PURCHASED);

                toast.EnqueueToast("1 250 000 Gravity Points purchased", null, 2.5f);
                break;
        }
    }

    private ProductInfo GetProductByID(string id)
    {
        ProductInfo product = Array.Find(products, prod => prod.id == id);
        return product;
    }
}