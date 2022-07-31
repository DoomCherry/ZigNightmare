using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Waiter : MonoBehaviour
{
    public UnityEvent OnWaitEnded;
    public float waitTime = 1;

    void Start()
    {
        this.WaitSecond(waitTime, OnWaitEnd);
    }

    void OnWaitEnd()
    {

        OnWaitEnded?.Invoke();
    }
}
