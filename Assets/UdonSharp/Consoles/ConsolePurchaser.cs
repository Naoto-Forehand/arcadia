using UdonSharp;
using UnityEngine;
using VRC.Economy;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Object = UnityEngine.Object;

public class ConsolePurchaser : UdonSharpBehaviour
{
    [SerializeField]
    private string PURCHASE_GROUP_ID;
    
    [SerializeField]
    private UdonProduct[] _products;

    [SerializeField]
    private GameObject _prefabPurchaser;

    [SerializeField]
    private int _indexPurchase;

    private IProduct[] _availableProducts;
    private Vector3 _spawnLocation;
    
    private void Start()
    {
        var pos = transform.position;
        _spawnLocation = new Vector3(pos.x,pos.y + 2,pos.z);
        
        var ub = (IUdonEventReceiver)this;
        Store.ListAvailableProducts(ub);
    }

    public override void Interact()
    {
        Debug.Log("CONSOLE PRESSED");
        if (!Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        foreach (var product in _availableProducts)
        {
            if (Store.DoesPlayerOwnProduct(Networking.LocalPlayer, product))
            {
                CreateToken();
            }
        }
    }

    public override void OnListAvailableProducts(IProduct[] products)
    {
        Debug.Log($"Found {products.Length} products!");
        _availableProducts = products;
        // IProduct product = null;
        // UdonProduct simp = null;
        // for (int productInd = 0; productInd < products.Length; ++productInd)
        // {
        //     if (products[productInd].ID == _products[_indexPurchase].ID)
        //     {
        //         product = products[productInd];
        //         simp = _products[_indexPurchase];
        //     }
        // }
        //
        // Debug.Log($"{(product != null)} being attached to object");
        // var purchaser = Object.Instantiate(_prefabPurchaser, _spawnLocation, Quaternion.identity);
        // purchaser.GetComponent<PickupPurchase>().Init(product,simp,PURCHASE_GROUP_ID);
    }

    private void CreateToken()
    {
        var purchaser = Object.Instantiate(_prefabPurchaser, _spawnLocation, Quaternion.identity);
        purchaser.GetComponent<Rigidbody>().AddForce(Vector3.up);
    }
}
