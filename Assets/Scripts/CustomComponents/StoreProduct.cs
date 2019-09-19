using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreProduct : MonoBehaviour
{
    protected static Color32 PURCHASED_COLOR = new Color32(0, 255, 80, 100);
    protected static Color32 AVAILABLE_COLOR = new Color32(0, 220, 255, 120);
    protected static Color32 INACTIVE_COLOR = new Color32(80, 80, 80, 150);

    public enum ProductState { AVAILABLE, PURCHASED, INACTIVE }


    public string id;
    public float price;
    [HideInInspector] public Image mainImage;
    [HideInInspector] public Text priceText;

    protected void OnEnable()
    {
        priceText = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<Text>(gameObject, "Price");
        priceText.text = price.ToString();
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
