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
        Audio.clip = GetRandomVariation();
    }

    public AudioClip GetRandomVariation()
    {
        if (_variation == null || _variation.Count < 1)
            return null;
        return _variation[Random.Range(0, _variation.Count)];
    }

    public void PlayRandom()
    {
        Audio.clip = GetRandomVariation();

        if (Audio.clip != null)
            Audio.Play();
    }

    public void Pause()
    {
        if (Audio.clip != null)
            Audio.Pause();
    }

    public void Stop()
    {
        Audio.Stop();
    }
}
