using UdonSharp;
using UnityEngine;
using VRC.Economy;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Object = UnityEngine.Object;

// TODO: Update name to TokenDispenser
public class ConsolePurchaser : UdonSharpBehaviour
{
    [SerializeField]
    private AudioSource _sfx;
    
    [SerializeField]
    private string PURCHASE_GROUP_ID;
    
    [SerializeField]
    private UdonProduct[] _products;

    [SerializeField]
    private GameObject _prefabPurchaser;

    [SerializeField]
    private int _tokenMax;

    private IProduct[] _availableProducts;
    private Vector3 _spawnLocation;
    
    [UdonSynced, FieldChangeCallback(nameof(TokensGenerated))]
    private int _tokensGenerated;

    public int TokensGenerated
    {
        get { return _tokensGenerated; }
        set
        {
            _tokensGenerated = value;
            if (Networking.IsOwner(gameObject))
            {
                RequestSerialization();
            }
        }
    }
    
    private void Start()
    {
        var pos = transform.position;
        _spawnLocation = new Vector3(pos.x,pos.y + 2,pos.z);
        
        var ub = (IUdonEventReceiver)this;
        Store.ListAvailableProducts(ub);

        if (Networking.IsOwner(gameObject))
        {
            TokensGenerated = 0;
        }
    }

    public override void Interact()
    {
        Debug.Log($"CONSOLE PRESSED {TokensGenerated} {_tokenMax}");
        if (!Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        if (TokensGenerated < _tokenMax)
        {
            foreach (var product in _availableProducts)
            {
                if (Store.DoesPlayerOwnProduct(Networking.LocalPlayer, product))
                {
                    CreateToken();
                    break;
                }
            }
        }
    }

    public override void OnListAvailableProducts(IProduct[] products)
    {
        Debug.Log($"Found {products.Length} products!");
        _availableProducts = products;
    }

    private void CreateToken()
    {
        _sfx.PlayOneShot(_sfx.clip);
        var purchaser = Object.Instantiate(_prefabPurchaser, _spawnLocation, Quaternion.identity);
        purchaser.GetComponent<Token>().Init(this);
        purchaser.GetComponent<Rigidbody>().AddForce(Vector3.up);
    }
}
