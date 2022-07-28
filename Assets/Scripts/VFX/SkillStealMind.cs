using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class SkillStealMind : MonoBehaviour
{
    //-------PROPERTY
    public Transform MyTransform => _myTransform = _myTransform ??= transform;
    public TrailRenderer TrailRenderer => _trailRenderer = _trailRenderer ??= GetComponent<TrailRenderer>();




    //-------FIELD
    [SerializeField]
    private TargetSelector _targeter;

    [SerializeField]
    private Transform _flollowTo;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _hideDistance = 1;
    [SerializeField]
    private float _followYAdditionalPosition = 1;
    private Transform _myTransform;
    private TrailRenderer _trailRenderer;
    private Coroutine _waitToHide;




    //-------METODS
    private void FixedUpdate()
    {
        MyTransform.position = Vector3.Lerp(MyTransform.position, new Vector3(_flollowTo.position.x, _flollowTo.position.y + _followYAdditionalPosition, _flollowTo.position.z), _speed);

        if(Vector3.Distance(MyTransform.position, _flollowTo.position) < _hideDistance)
        {
            this.WaitSecond(TrailRenderer.time, delegate { gameObject.SetActive(false); });
        }
    }

    public void GoToTarget()
    {
        if (_waitToHide != null)
            StopCoroutine(_waitToHide);

        if (_targeter != null)
        {
            MyTransform.position = _targeter.CurrentTarget.MyTransform.position;
        }

        gameObject.SetActive(true);
    }


}
