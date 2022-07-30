using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BossStage
{
    Walking = 0,
    Rolling
}
public class BossAnimationControiler : MonoBehaviour
{
    //-------PROPERTY
    public Transform MyTransform => _transform = _transform ??= transform;
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());




    //-------FIELD
    [SerializeField]
    public string _rollingStageBoolName = "IsRolling";
    [SerializeField]
    public string _speedFloatName = "Speed";
    [SerializeField]
    public string _walkMultName = "RolingMult";
    [SerializeField]
    public string _rolingMultName = "WalkMult";

    [SerializeField]
    private Animator _customAnimator;
    private Animator _animator;
    private Transform _transform;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onAwake;
    public event UnityAction OnAwake
    {
        add => _onAwake.AddListener(value);
        remove => _onAwake.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onRolling;
    public event UnityAction OnRolling
    {
        add => _onRolling.AddListener(value);
        remove => _onRolling.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onWalking;
    public event UnityAction OnWalking
    {
        add => _onWalking.AddListener(value);
        remove => _onWalking.RemoveListener(value);
    }




    //-------METODS
    private void Start()
    {
        _onAwake?.Invoke();
    }

    public void IsRolling(bool isRoling)
    {
        Animator.SetBool(_rollingStageBoolName, isRoling);

        if (isRoling)
            _onRolling?.Invoke();
        else
            _onWalking?.Invoke();
    }

    public void SetSpeed(float speed)
    {
        Animator.SetFloat(_speedFloatName, speed);
    }

    public void SetWalkMult(float speed)
    {
        Animator.SetFloat(_walkMultName, speed);
    }

    public void SetRollingMult(float speed)
    {
        Animator.SetFloat(_rolingMultName, speed);
    }
}
