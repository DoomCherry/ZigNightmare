using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private float _liveTime = -1;

    private SoundArray _localSource;
    private float _lastTimeUse = 0;
    private List<LocalAudioSourceCopy> _instance = new List<LocalAudioSourceCopy>();




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
                LocalAudioSourceCopy instance = Instantiate(this);
                instance._isInstanceable = false;
                instance.Play();
                instance.SetLiveTime(_instanceLiveTime);

                _instance = _instance.Where(n => n != null).ToList();

                _instance.Add(instance);

                _lastTimeUse = Time.time;
            }
        }

    }

    private void SetLiveTime(float time)
    {
        this.WaitSecond(time,
                    delegate
                    {
                        Stop();
                        Destroy(gameObject);
                    });
    }

    public void Pause()
    {
        _localSource.Pause();
    }

    public void Stop()
    {
        _localSource.Stop();
    }
}
