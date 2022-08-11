using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemparalObject : MonoBehaviour
{
    //-------FIELD
    [SerializeField]
    private float _liveTime = 5;




    //-------EVENTS




    //-------METODS
    private void Start()
    {
        this.WaitSecond(_liveTime, Destroy);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
