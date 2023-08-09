
using UdonSharp;
using UnityEngine;
using VRC.Economy;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PickupPurchase : UdonSharpBehaviour
{
    private IProduct _product;
    private UdonProduct _simpleProduct;
    private string _groupID;
    
    public void Init(IProduct product, UdonProduct simpleProduct, string groupID)
    {
        _product = product;
        _simpleProduct = simpleProduct;
        _groupID = groupID;
        var eventReceiver = (IUdonEventReceiver)this;
        Store.UsePurchase(eventReceiver,_product);
    }

    public override void OnPickup()
    {
        var id = (_product != null) ? _product.ID : "";
        Debug.Log($"{id} is IN HAND");
    }

    public override void OnPickupUseUp()
    {
        var id = (_product != null) ? _product.ID : "";
        Debug.Log($" UseUp {id}");
        var eventReceiver = (IUdonEventReceiver)this;
        Store.UsePurchase(eventReceiver,_product);
        
        Store.OpenGroupPage(_groupID);
    }

    public override void OnPickupUseDown()
    {
        var id = (_product != null) ? _product.ID : "";
        Debug.Log($" UseDown {id}");
    }
    
    
    // public void OnPurchaseUse(IProduct product)
    // {
    //     Debug.Log($"{product.Buyer.playerId} {product.ID}");
    // }
}
