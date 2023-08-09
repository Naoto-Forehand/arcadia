
using System;
using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using VRC.Core.Pool;
using VRC.Economy;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using Object = UnityEngine.Object;

public class ConsolePurchaser : UdonSharpBehaviour
{
    public UdonBehaviour ConsoleGraph;
    public DoorLock DoorLock;

    [SerializeField]
    private string PURCHASE_GROUP_ID;
    
    [SerializeField]
    private UdonProduct[] _products;

    [SerializeField]
    private GameObject _prefabPurchaser;

    [SerializeField]
    private int _indexPurchase;
    
    private Vector3 _spawnLocation;
    
    // private ObjectPool<GameObject> _objectPool;
    private void Start()
    {
        var pos = transform.position;
        _spawnLocation = new Vector3(pos.x,pos.y + 2,pos.z);
    }

    public override void Interact()
    {
        Debug.Log("CONSOLE PRESSED");
        Store.OpenGroupPage(PURCHASE_GROUP_ID);
        var ub = (IUdonEventReceiver)this;
        Store.ListAvailableProducts(ub);
        // Store.OpenProduct(_products[_indexPurchase]);
    }

    public void OnListAvailableProducts(IProduct[] products)
    {
        Debug.Log($"Found {products.Length} products!");
        IProduct product = null;
        UdonProduct simp = null;
        for (int productInd = 0; productInd < products.Length; ++productInd)
        {
            if (products[productInd].ID == _products[_indexPurchase].ID)
            {
                product = products[productInd];
                simp = _products[_indexPurchase];
            }
        }

        Debug.Log($"{(product != null)} being attached to object");
        var purchaser = Object.Instantiate(_prefabPurchaser, _spawnLocation, Quaternion.identity);
        purchaser.GetComponent<PickupPurchase>().Init(product,simp,PURCHASE_GROUP_ID);
        // DoorLock.Product = 
        // var myObject = _objectPool.Get();
        // Vector3 spawnLocation = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2,
        //      gameObject.transform.position.z);
        // myObject.transform.SetPositionAndRotation(spawnLocation,quaternion.identity);
    }
}
