using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalCore : MonoBehaviour
{
    //-------PROPERTY
    private PhysicalBodyHandler PhysicalHandler => _physicalHandler = _physicalHandler ??= GetComponentInChildren<PhysicalBodyHandler>();




    //-------FIELD
    [SerializeField]
    private Vector3 _savePosition = Vector3.zero;

    private bool _isOriginal = true;
    private PhysicalCore _dublicate;
    private PhysicalBodyHandler _physicalHandler;




    //-------EVENTS




    //-------METODS
    private void Start()
    {
        if (_isOriginal == false)
            return;

        if (PhysicalHandler == null)
        {
            Debug.LogError($"{name}: Missing {nameof(PhysicalBodyHandler)}! " +
                                  $"Please leave game mod, set {nameof(PhysicalBodyHandler)} in child and try again.");
        }

        _dublicate = Instantiate(this, _savePosition, Quaternion.identity, transform.parent);
        _dublicate._isOriginal = false;
        _dublicate.PhysicalHandler.DestroyPhysics();
    }

    private void FixedUpdate()
    {
        if (_isOriginal)
        {
            PhysicalHandler.UpdateBodyPart(_dublicate.PhysicalHandler);
        }
    }
}
