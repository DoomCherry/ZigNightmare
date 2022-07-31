using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioChecker : MonoBehaviour
{

    public AudioSource _audioSource;
    public UnityEvent OnAudioEnd;
    public UnityEvent OnAudioPlay;
    public bool IsWaitPlay = true;

    void Start()
    {
        if (!IsWaitPlay)
            StartCoroutine(waitAudioEnd());
        else
            StartCoroutine(waitAudio());
    }

    IEnumerator waitAudio()
    {
        while (!_audioSource.isPlaying)
        {
            yield return null;
        }

        OnAudioPlay?.Invoke();
    }

    IEnumerator waitAudioEnd()
    {
        while (_audioSource.isPlaying)
        {
            yield return null;
        }

        OnAudioEnd?.Invoke();
    }
}
