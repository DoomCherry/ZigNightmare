using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathAgent : MonoBehaviour
{
    //-------PROPERTY
    public Transform MyTransform => _transform = _transform ??= transform;
    public float SpeedMult
    {
        get
        {
            return _speedMult;
        }

        set
        {
            _speedMult = value;
        }
    }
    public bool IsLoockToPoint
    {
        get
        {
            return _isLockToPoint;
        }

        set
        {
            _isLockToPoint = value;
        }
    }




    //-------FIELD
    private Transform _transform;
    [SerializeField]
    private PathCreator _path;
    [SerializeField]
    private float _speed = 1, _viewSpeed = 0.5f, _speedMult = 1;
    [SerializeField]
    private float _minPointDistance = 1;
    [SerializeField]
    public bool _isLockToPoint = true;
    [SerializeField]
    private bool _isStrictlyGo = true, _isLooping = true;
    private int _currentPoint = 0, _nextPoint = 1;




    //-------EVENTS




    //-------METODS
    private void Start()
    {
        if (_path.PathPoints == null || _path.PathPoints.Count <= 0)
            return;

        MyTransform.position = _path.PathPoints[0].MyTransform.position;
        _path.PathPoints[0].CompleatePath(this);
        _currentPoint = 0;
        _nextPoint = 1;
    }

    private void Update()
    {
        if (_path.PathPoints == null || _path.PathPoints.Count <= 0)
            return;

        if (_nextPoint >= _path.PathPoints.Count && _isLooping == false)
            return;

        if (_nextPoint >= _path.PathPoints.Count)
            _nextPoint = 0;

        Vector3 nextPoint = _path.PathPoints[_nextPoint].MyTransform.position;

        Vector3 direction = nextPoint - MyTransform.position;
        if (_isLockToPoint)
        {
            MyTransform.rotation = Quaternion.Slerp(MyTransform.rotation, Quaternion.LookRotation(direction), _viewSpeed);
        }

        if (_isStrictlyGo)
        {
            MyTransform.position += direction.normalized * _speed * _speedMult * Time.deltaTime;
        }
        else
        {
            _isLockToPoint = true;
            MyTransform.position += MyTransform.forward * _speed * _speedMult * Time.deltaTime;
        }

        if (Vector3.Distance(MyTransform.position, nextPoint) <= _minPointDistance)
        {
            _path.PathPoints[_nextPoint].CompleatePath(this);
            _nextPoint++;
            _currentPoint++;
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
