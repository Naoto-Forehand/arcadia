
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.Economy;
using VRC.SDKBase;
using VRC.Udon;

public class BannerDisplay : UdonSharpBehaviour
{
    private AudioSource _audio;
    private Animator _animator;

    [SerializeField]
    private int _sfxRepeats;
    [SerializeField]
    private string _textAFormat;
    [SerializeField]
    private string _textBFormat;

    [SerializeField]
    Text _textA;
    [SerializeField]
    Text _textB;

    [UdonSynced, FieldChangeCallback(nameof(TextAText))]
    private string _textAText;
    [UdonSynced, FieldChangeCallback(nameof(TextBText))]
    private string _textBText;

    public string TextAText
    {
        get { return _textAText; }
        set
        {
            _textAText = value;
            _textA.text = (!string.IsNullOrWhiteSpace(_textAText)) ? string.Format(_textAFormat,_textAText) : string.Empty;
        }
    }
    
    public string TextBText
    {
        get { return _textBText; }
        set
        {
            _textBText = value;
            _textB.text = (!string.IsNullOrWhiteSpace(_textBText)) ? string.Format(_textBFormat,_textBText) : string.Empty;
        }
    }
    
    void Start()
    {
        _audio = gameObject.GetComponent<AudioSource>();
        _animator = gameObject.GetComponent<Animator>();
        
        if (Networking.IsOwner(gameObject))
        {
            TextAText = string.Empty;
            TextBText = string.Empty;
            
            RequestSerialization();
        }
        else
        {
            _textAText = string.Empty;
            _textBText = string.Empty;
        }
    }

    public override void OnPurchaseConfirmed(IProduct product, VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            Debug.Log($"{player.playerId} Purchased, update the banner please");
            TextAText = player.displayName;
            TextBText = product.Name;
            for (int index = 0; index < _sfxRepeats; ++index)
            {
                _audio.PlayDelayed(index);
            }
            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
        {
            _textA.text = (!string.IsNullOrWhiteSpace(_textAText)) ? string.Format(_textAFormat,_textAText) : string.Empty;
            _textB.text = (!string.IsNullOrWhiteSpace(_textBText)) ? string.Format(_textBFormat,_textBText) : string.Empty;
            if (!string.IsNullOrWhiteSpace(_textA.text) && !string.IsNullOrWhiteSpace(_textB.text))
            {
                for (int index = 0; index < _sfxRepeats; ++index)
                {
                    _audio.PlayDelayed(index);
                }
            }
        }
    }
}
