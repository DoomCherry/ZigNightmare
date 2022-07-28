using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundArray : MonoBehaviour
{
    //-------PROPERTY
    public AudioSource Audio => _audioSource = _audioSource ??= GetComponent<AudioSource>();




    //-------FIELD
    [SerializeField]
    private List<AudioClip> _variation;
    private AudioSource _audioSource;




    //-------METODS
    private void Awake()
    {
        if (_variation == null || _variation.Count == 0)
        {
            Debug.LogError($"{name}: install at least one audio clip in {nameof(SoundArray)}");
            return;
        }

        Audio.clip = GetRandomVariation();
    }

    public AudioClip GetRandomVariation()
    {
        return _variation[Random.Range(0, _variation.Count)]; ;
    }

    public void PlayRandom()
    {
        Audio.clip = GetRandomVariation();
        Audio.Play();
    }

    public void Pause()
    {
        Audio.Pause();
    }
}
