using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class CenaDraggingHandControiler : MonoBehaviour
{
    //-------PROPERTY
    public Transform MyTransform
    {
        get
        {
            _myTransform = _myTransform == null ? transform : _myTransform;
            return _myTransform;
        }
    }
    public Collider MyCollider
    {
        get
        {
            _collider = _collider == null ? GetComponent<Collider>() : _collider;
            return _collider;
        }
    }
    public CenaDraggingState CurrentState { get; private set; }

    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());




    //-------FIELD
    [SerializeField]
    private Animator _customAnimator;
    [SerializeField]
    private Transform _customTargetPosition;
    [SerializeField]
    private string _speedPunchFloatName = "CenaPunchSpeed";
    private CenaDragging _cenaDragging;
    private Transform _myTransform;
    private Collider _collider;
    private Coroutine _grabing, _punch;
    private Animator _animator;
    private ICharacterLimiter _graggableTarget;



    //-------EVENTS
    [SerializeField]
    private UnityEvent _onDragStart = new UnityEvent();
    public event UnityAction OnPulling
    {
        add => _onDragStart.AddListener(value);
        remove => _onDragStart.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onPunchEnd = new UnityEvent();
    public event UnityAction OnPullingEnd
    {
        add => _onPunchEnd.AddListener(value);
        remove => _onPunchEnd.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onEnd = new UnityEvent();
    public event UnityAction OnEnd
    {
        add => _onEnd.AddListener(value);
        remove => _onEnd.RemoveListener(value);
    }




    //-------METODS
    public enum CenaDraggingState
    {
        NotReady = -1,
        Grab = 0,
        Punch = 2,
    }

    public void GrabIt(CenaDragging bottomDragging)
    {
        //CheckDragging();
        CurrentState = CenaDraggingState.Grab;
        _cenaDragging = bottomDragging;
        Animator.SetFloat(_speedPunchFloatName, _cenaDragging.SkillContainer.cenaPunchInfo.punchSpeed);

        if (_grabing != null)
            StopCoroutine(_grabing);
        _grabing = StartCoroutine(WaitToGrab());
    }

    private IEnumerator WaitToGrab()
    {
        float time = 0;
        CheckDragging();

        while (time < _cenaDragging.MaxWaitingTime)
        {
            CheckDragging();
            ICharacterLimiter[] targets = Physics.OverlapBox(MyCollider.bounds.center, MyCollider.bounds.extents / 2, Quaternion.identity, _cenaDragging.TargetLayer,QueryTriggerInteraction.Ignore)
                                         .Select(n => n.GetComponent<ICharacterLimiter>()).ToArray();

            if (targets.Length > 0)
            {
                _graggableTarget = targets.First();
                break;
            }

            time += Time.deltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        if (_graggableTarget != null)
        {
            if (_punch != null)
                StopCoroutine(_punch);

            _punch = StartCoroutine(Punch());
        }
        else
        {
            CurrentState = CenaDraggingState.NotReady;
            _onEnd.Invoke();
        }

        _onDragStart?.Invoke();
    }

    private IEnumerator Punch()
    {
        CheckDragging();
        CheckTarget();
        CurrentState = CenaDraggingState.Punch;
        Animator.SetInteger(_cenaDragging.AnimationStateNameInteger, (int)CurrentState);
        _graggableTarget.DisablePhysicsTarget();
        _graggableTarget.FreezeSkill();
        _graggableTarget.FreezeFalling();
        _cenaDragging.Limiter.FreezeRotation();
        _cenaDragging.Limiter.FreezeWalking();

        while (Animator.GetInteger(_cenaDragging.AnimationStateNameInteger) == (int)CenaDraggingState.Punch)
        {
            CheckDragging();
            CheckTarget();
            _graggableTarget.MyRigidbody.position = _customTargetPosition == null ? MyTransform.position : _customTargetPosition.position;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        CheckDragging();
        CheckTarget();
        Vector3 moveTo = _graggableTarget.MyRigidbody.transform.position - _cenaDragging.Limiter.MyRigidbody.transform.position;
        moveTo.y = 0;
        _graggableTarget.MyRigidbody.velocity = (-moveTo).normalized * 10;
        _graggableTarget.TakeDamage(_cenaDragging.TotalDamage);
        _graggableTarget.EnablePhysicsTarget();
        _graggableTarget.UnfreezeSkill();
        _graggableTarget.UnfreezeFalling();
        _cenaDragging.Limiter.UnfreezeWalking();
        _cenaDragging.Limiter.UnfreezeRotation();
        _onPunchEnd?.Invoke();
    }

    public void Stop()
    {
        if (Animator != null)
            Animator.SetInteger(_cenaDragging.AnimationStateNameInteger, (int)CenaDraggingState.Grab);

        _cenaDragging.Limiter.UnfreezeWalking();
        _cenaDragging.Limiter.UnfreezeRotation();
    }

    private void CheckDragging()
    {
        if (_cenaDragging == null)
        {
            Destroy(gameObject);
        }
    }

    private void CheckTarget()
    {
        if (_graggableTarget == null)
        {
            Destroy(gameObject);
        }
    }
}
