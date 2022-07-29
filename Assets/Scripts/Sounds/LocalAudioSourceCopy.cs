using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAudioSourceCopy : MonoBehaviour
{
    //-------FIELDS
    [SerializeField]
    private float _minimalInterval = 0.3f;
    [SerializeField]
    private SoundArray _globalSource;
    [SerializeField]
    private bool _isInstanceable = false;
    [SerializeField]
    private float _instanceLiveTime = 0.5f;

    private SoundArray _localSource;
    private float _lastTimeUse = 0;
    private LocalAudioSourceCopy _instance;




    //-------METODS
    private void Awake()
    {
        if (_globalSource == null)
        {
            Debug.LogError($"{name}: exit play mode. and set global source");
            return;
        }

        _localSource = Instantiate(_globalSource, transform);
        _localSource.transform.localPosition = Vector3.zero;
    }

    public void Play()
    {
        if (Time.time - _lastTimeUse > _minimalInterval)
        {
            if (_isInstanceable == false)
            {
                _localSource.PlayRandom();
                _lastTimeUse = Time.time;
            }
            else
            {
                _instance = Instantiate(this);
                _instance._isInstanceable = false;
                _instance.Play();

                this.WaitSecond(_instanceLiveTime, delegate { Destroy(_instance.gameObject); });
            }
        }

    }

    public void Pause()
    {
        _localSource.Pause();
    }
}
