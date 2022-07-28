using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    //-------FIELD
    private Camera _main;
    private Transform _myTransform;
    private Transform _cameraTransform;




    //-------METODS
    private void Start()
    {
        _main = Camera.main;
        _myTransform = transform;
        _cameraTransform = _main.transform;
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation((_cameraTransform.position - _myTransform.position).normalized);
    }
}
