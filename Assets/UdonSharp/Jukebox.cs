
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Jukebox : UdonSharpBehaviour
{
    public AudioClip DefaultBGM;
    [SerializeField]
    private AudioClip[] _music;
    [SerializeField]
    private string[] _productIDs;
    
    [SerializeField]
    private AudioSource _audioSource;
    
    void Start()
    {
        _audioSource.clip = DefaultBGM;
        _audioSource.Play();
    }
}
