using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalBodyPart : MonoBehaviour
{
    //-------PROPERTY
    private ConfigurableJoint Joint
    {
        get
        {
            _joint = _joint == null ? GetComponent<ConfigurableJoint>() : _joint;
            return _joint;
        }
    }




    //-------FIELD
    [SerializeField]
    private bool _isJointControl = false;
    [SerializeField]
    private float _damper;
    [SerializeField]
    private bool _isLocalSpace = false;

    [SerializeField]
    private bool _fixByX = false;
    [SerializeField]
    private bool _fixByY = false;
    [SerializeField]
    private bool _fixByZ = false;
    [SerializeField]
    private Transform _target;
    private Vector3 _startPosition;
    private Transform _myTransform;
    private ConfigurableJoint _joint;




    //-------METODS
    private void Start()
    {
        _myTransform = transform;
        _startPosition = _target.position;
    }

    private void FixedUpdate()
    {
        Vector3 startPosition;
        if (_isLocalSpace)
        {
            startPosition = new Vector3(_fixByX ? _target.localPosition.x : _myTransform.localPosition.x,
                                            _fixByY ? _target.localPosition.y : _myTransform.localPosition.y,
                                            _fixByZ ? _target.localPosition.z : _myTransform.localPosition.z);

            if (_isJointControl)
                Joint.targetPosition = Vector3.Lerp(_myTransform.localPosition, startPosition, _damper);
            else
                _myTransform.localPosition = Vector3.Lerp(_myTransform.localPosition, startPosition, _damper);
        }
        else
        {
            startPosition = new Vector3(_fixByX ? _target.position.x : _myTransform.position.x,
                                            _fixByY ? _target.position.y : _myTransform.position.y,
                                            _fixByZ ? _target.position.z : _myTransform.position.z);
            if (_isJointControl)
                Joint.targetPosition = Vector3.Lerp(_myTransform.position, startPosition, _damper);
            else
                _myTransform.position = Vector3.Lerp(_myTransform.position, startPosition, _damper);
        }

    }
}
