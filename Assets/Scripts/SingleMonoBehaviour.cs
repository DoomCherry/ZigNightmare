using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMonoBehaviour<T> : MonoBehaviour 
    where T : MonoBehaviour
{
    //-------PROPERTY
    public static T Instance { get; private set; }




    //-------FIELD




    //-------EVENTS




    //-------METODS
    protected virtual void Start()
    {
        T newInstance = GetComponent<T>();

        if (Instance != null && Instance != newInstance)
            throw new System.Exception($"{name}: {nameof(T)} is {nameof(SingleMonoBehaviour<T>)} and scene most contain a single copy");

        Instance = GetComponent<T>();
    }
}
