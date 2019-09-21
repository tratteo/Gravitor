using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreProduct : MonoBehaviour
{
    protected static Color32 PURCHASED_COLOR = new Color32(0, 255, 80, 100);
    protected static Color32 AVAILABLE_COLOR = new Color32(0, 220, 255, 100);
    protected static Color32 INACTIVE_COLOR = new Color32(80, 80, 80, 150);

    public enum ProductState { AVAILABLE, PURCHASED, INACTIVE, CONSUMED }


    public string id;
    public float price;
    [HideInInspector] public Image mainImage;
    [HideInInspector] public Text priceText;


    protected void Awake()
    {
        priceText = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<Text>(gameObject, "Price");
        if (priceText != null) priceText.text = price.ToString();
        mainImage = GetComponentInChildren<Image>(true);
    }

    public void SetProductState(ProductState state)
    {
        switch (state)
        {
            case ProductState.AVAILABLE:
                mainImage.raycastTarget = true;
                mainImage.color = AVAILABLE_COLOR;
                priceText?.gameObject.SetActive(true);
                break;
            case ProductState.PURCHASED:
                mainImage.raycastTarget = true;
                mainImage.color = AVAILABLE_COLOR;
                priceText?.gameObject.SetActive(false);
                break;
            case ProductState.INACTIVE:
                mainImage.raycastTarget = false;
                mainImage.color = INACTIVE_COLOR;
                priceText?.gameObject.SetActive(true);
                break;
            case ProductState.CONSUMED:
                mainImage.raycastTarget = false;
                mainImage.color = PURCHASED_COLOR;
                priceText?.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}
