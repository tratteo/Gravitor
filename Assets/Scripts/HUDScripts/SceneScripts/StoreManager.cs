using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    private const string EMPTY_PRODUCT = "code_none";  
    private string highlightedProduct = EMPTY_PRODUCT;

    private Color32 PURCHASED_COLOR = new Color32(0, 255, 80, 100);

    [SerializeField] private GameObject disclaimerPanel = null;
    [SerializeField] private Image grbLVL3 = null;
    [SerializeField] private ToastScript toast = null;

    private PlayerData playerData;
    private PlayerSkillsData skillsData;

    private void OnDisable()
    {
        GoogleIAPManager.GetInstance().UnSubscribeToProductBoughtEvent(ProductBought);
    }

    private void Start()
    {
        GoogleIAPManager.GetInstance().SubscribeToProductBoughtEvent(ProductBought);
        
        playerData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();
        skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();

        if (playerData.playerState == PlayerManager.PlayerState.COMET && skillsData.gammaRayBurstPoints < PlayerSkillsData.GRB_MAX_POINTS)
        {
            grbLVL3.raycastTarget = true;
        }
        else
        {
            grbLVL3.raycastTarget = false;
            grbLVL3.color = PURCHASED_COLOR;
        }
    }

    public void ShowProductDisclaimerPanel(string productId)
    {
        disclaimerPanel.SetActive(true);
        highlightedProduct = productId;
    }

    public void HideProductDisclaimerPanel()
    {
        disclaimerPanel.SetActive(false);
        highlightedProduct = EMPTY_PRODUCT;
    }

    public void BuyHighlightedProduct()
    {
        GoogleIAPManager.GetInstance().BuyProduct(highlightedProduct);
        disclaimerPanel.SetActive(false);
        highlightedProduct = EMPTY_PRODUCT;

    }

    private void ProductBought(string id)
    {
        switch (id)
        {
            case GoogleIAPManager.PRODUCT_GRBLVL3:
                grbLVL3.raycastTarget = false;
                grbLVL3.color = PURCHASED_COLOR;
                toast.EnqueueToast("GRB level 3 purchased!", null, 2.5f);
                break;
        }
    }

}