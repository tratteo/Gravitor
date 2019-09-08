using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ProductInfo : MonoBehaviour
{

    private Color32 PURCHASED_COLOR = new Color32(0, 255, 80, 100);
    private Color32 AVAILABLE_COLOR = new Color32(0, 220, 255, 120);
    private Color32 INACTIVE_COLOR = new Color32(80, 80, 80, 150);

    public enum ProductState { AVAILABLE, PURCHASED, INACTIVE }


    public string id;
    [HideInInspector] public Image mainImage;
    [HideInInspector] public Text priceText;
    [HideInInspector] public Product product;

    private void OnEnable()
    {
        product = GoogleIAPManager.GetInstance().GetProductWithID(id);
        priceText = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<Text>(gameObject, "Price");
        priceText.text = product.metadata.localizedPrice.ToString("0.00") + " €";
        mainImage = GetComponent<Image>();
    }

    public void SetProductState(ProductState state)
    {
        switch (state)
        {
            case ProductState.AVAILABLE:
                mainImage.raycastTarget = true;
                mainImage.color = AVAILABLE_COLOR;
                break;
            case ProductState.PURCHASED:
                mainImage.raycastTarget = false;
                mainImage.color = PURCHASED_COLOR;
                break;
            case ProductState.INACTIVE:
                mainImage.raycastTarget = false;
                mainImage.color = INACTIVE_COLOR;
                break;
            default:
                break;
        }
    }
}
