using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerInfo
{
    //-------PROPERTY
    public Collider CurrentCollider => _collideWith;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onTriggerEnter;
    public event UnityAction OnTriggerEntered
    {
        add => _onTriggerEnter.AddListener(value);
        remove => _onTriggerEnter.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onTriggerStay;
    public event UnityAction OnTriggerStayed
    {
        add => _onTriggerStay.AddListener(value);
        remove => _onTriggerStay.RemoveListener(value);
    }




    //-------FIELD
    [SerializeField]
    private Collider _collideWith;




    //-------METODS
    public void TriggerEnter()
    {
        _onTriggerEnter?.Invoke();
    }

    public void TriggerStay()
    {
        _onTriggerStay?.Invoke();
    }
}

[RequireComponent(typeof(Collider))]
public class OnTriggerContorller : MonoBehaviour
{
    //-------EVENTS





    //-------FIELD
    [SerializeField]
    private List<TriggerInfo> _infos = new List<TriggerInfo>();




    //-------METODS
    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < _infos.Count; i++)
        {
            if (other == _infos[i].CurrentCollider)
                _infos[i].TriggerEnter();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < _infos.Count; i++)
        {
            if (other == _infos[i].CurrentCollider)
                _infos[i].TriggerStay();
        }
    }
}
