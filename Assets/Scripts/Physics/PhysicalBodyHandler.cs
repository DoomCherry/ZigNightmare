using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalBodyHandler : MonoBehaviour
{
    //-------PROPERTY




    //-------FIELD
    [SerializeField]
    private List<Transform> _notPhysicsObjects;
    [SerializeField]
    private List<Rigidbody> _physicObjects;
    [SerializeField, HideInInspector]
    private List<Transform> _physicObjectTransforms = new List<Transform>();
    [SerializeField, HideInInspector]
    private List<ConfigurableJoint> _joints = new List<ConfigurableJoint>();
    private List<Quaternion> _startRotations = new List<Quaternion>();




    //-------EVENTS




    //-------METODS
    private void OnValidate()
    {
        if (_notPhysicsObjects != null && _notPhysicsObjects.Count != 0)
        {
            for (int i = 0; i < _notPhysicsObjects.Count; i++)
            {
                if (_notPhysicsObjects[i] != null && _notPhysicsObjects[i].GetComponent<Rigidbody>() != null)
                {
                    _notPhysicsObjects.RemoveAt(i);
                    i--;
                }
            }
        }

        _physicObjectTransforms.Clear();
        _joints.Clear();
        if (_physicObjects != null && _physicObjects.Count != 0)
        {
            for (int i = 0; i < _physicObjects.Count; i++)
            {
                if (_physicObjects[i] != null)
                {
                    _physicObjectTransforms.Add(_physicObjects[i].transform);
                    _joints.Add(_physicObjects[i].GetComponent<ConfigurableJoint>());
                }
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < _physicObjectTransforms.Count; i++)
        {
            _startRotations.Add(_physicObjectTransforms[i].localRotation);
        }
    }

    public void UpdateBodyPart(PhysicalBodyHandler clone)
    {
        for (int i = 0; i < _notPhysicsObjects.Count; i++)
        {

            _notPhysicsObjects[i].localRotation = clone._notPhysicsObjects[i].localRotation;
        }

        for (int i = 0; i < _joints.Count; i++)
        {
            if (_joints[i] != null)
            {
                _joints[i].targetPosition = clone._physicObjectTransforms[i].localPosition;
                _joints[i].targetRotation = Quaternion.Inverse(clone._physicObjectTransforms[i].localRotation) * _startRotations[i];
            }
        }
    }

    public void DestroyPhysics()
    {
        for (int i = 0; i < _physicObjectTransforms.Count; i++)
        {
            var joint = _physicObjectTransforms[i].GetComponent<ConfigurableJoint>();
            if (joint != null)
                Destroy(joint);

            var collider = _physicObjectTransforms[i].GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            Destroy(_physicObjects[i]);
        }
    }
}
