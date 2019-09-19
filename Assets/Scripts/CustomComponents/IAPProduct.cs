using UnityEngine;
using UnityEngine.Purchasing;

public class IAPProduct : StoreProduct
{
    [HideInInspector] public Product product;

    private void OnEnable()
    {
        product = GoogleIAPManager.GetInstance().GetProductWithID(id);
        price = (float)product.metadata.localizedPrice;
        base.OnEnable();
        priceText.text += " €";
    }

    public void SetProductState(ProductState state)
    {
        base.SetProductState(state);
    }
}
