using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class BottomDraggingHandControiler : MonoBehaviour
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
    public BottomDraggingState CurrentStage { get; private set; }
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());




    //-------FIELD
    [SerializeField]
    private Animator _customAnimator;
    private BottomDragging _bottomDragging;

    private Transform _myTransform;
    private Collider _collider;
    private Coroutine _releaseHand, _pullHand;
    private Animator _animator;
    private ICharacterLimiter _graggableTarget;
    private ITarget _target;
    private Vector3 _lastGrabPosition;



    //-------EVENTS
    [SerializeField]
    private UnityEvent _onProjecttiling = new UnityEvent();
    public event UnityAction OnProjecttiling
    {
        add => _onProjecttiling.AddListener(value);
        remove => _onProjecttiling.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onGrabEmpty = new UnityEvent();
    public event UnityAction OnGrabEmpty
    {
        add => _onGrabEmpty.AddListener(value);
        remove => _onGrabEmpty.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onGrabWithEnemy = new UnityEvent();
    public event UnityAction OnGrabWithEnemy
    {
        add => _onGrabWithEnemy.AddListener(value);
        remove => _onGrabWithEnemy.RemoveListener(value);
    }


    [SerializeField]
    private UnityEvent _onPulling = new UnityEvent();
    public event UnityAction OnPulling
    {
        add => _onPulling.AddListener(value);
        remove => _onPulling.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onPullingEnd = new UnityEvent();
    public event UnityAction OnPullingEnd
    {
        add => _onPullingEnd.AddListener(value);
        remove => _onPullingEnd.RemoveListener(value);
    }




    //-------METODS
    public enum BottomDraggingState
    {
        NotReady = -1,
        Grab = 0,
        Pull,
    }

    public void ReleaseHand(BottomDragging bottomDragging)
    {
        _bottomDragging = bottomDragging;
        ReleaseHand();
    }

    private void ReleaseHand()
    {
        //if (CheckDraggingIsDestroy())
        //    return;

        if (_bottomDragging.Selector.CurrentTarget != null)
        {
            _target = _bottomDragging.Selector.CurrentTarget;
            Vector3 targetPosition = _target.MyTransform.position;

            Vector3 selfPosition =  MyTransform.position;

            Vector3 lockTo = targetPosition - selfPosition;
            lockTo.y = _bottomDragging.HandHeightStart;

            MyTransform.rotation = Quaternion.LookRotation(lockTo, Vector3.up);

            CurrentStage = BottomDraggingState.Grab;
            Animator.SetInteger(_bottomDragging.AnimationStateNameInteger, (int)CurrentStage);
            _releaseHand = StartCoroutine(Projectiling(_target));
        }
        else
        {
            Vector3 targetPosition = MyTransform.TransformDirection(MyTransform.forward) * float.MaxValue;
            targetPosition.y = _bottomDragging.HandHeightStart;

            Vector3 selfPosition = _bottomDragging.MyTransform.position;

            CurrentStage = BottomDraggingState.Grab;
            Animator.SetInteger(_bottomDragging.AnimationStateNameInteger, (int)CurrentStage);
            _releaseHand = StartCoroutine(Projectiling());
        }

        _onProjecttiling?.Invoke();
    }

    private void BackHand()
    {
        //if (CheckDraggingIsDestroy())
        //    return;

        CurrentStage = BottomDraggingState.Pull;
        Animator.SetInteger(_bottomDragging.AnimationStateNameInteger, (int)CurrentStage);
        _pullHand = StartCoroutine(Grab(null, null, false));
    }

    private void BackWithTarget()
    {
        //if (CheckDraggingIsDestroy())
        //    return;

        if (CheckTargetIsDestroy())
            return;

        CurrentStage = BottomDraggingState.Pull;
        Animator.SetInteger(_bottomDragging.AnimationStateNameInteger, (int)CurrentStage);

        void DraggTarget()
        {
            if (CheckTargetIsDestroy())
                return;

            _graggableTarget.FreezeSkill();
        }

        void OnPullEnd()
        {
            if (CheckTargetIsDestroy())
                return;

            _graggableTarget.UnfreezeSkill();
        }

        _pullHand = StartCoroutine(Grab(DraggTarget, OnPullEnd, true));
    }

    private IEnumerator Grab(Action onPull, Action onPullEnd, bool withTarget)
    {
        //if (CheckDraggingIsDestroy() == false)
        //{

        _lastGrabPosition = MyTransform.position;
        while (Vector3.Distance(MyTransform.position, _bottomDragging.transform.position) > _bottomDragging.StopDistance)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);

            //if (CheckDraggingIsDestroy())
            //    break;

            if (withTarget)
            {
                if (CheckTargetIsDestroy())
                    break;

                float lastDistance = Vector3.Distance(_lastGrabPosition, MyTransform.position);

                _graggableTarget.TakeDamage(_bottomDragging.DamagePerOneUnit * lastDistance);
                _graggableTarget.MyRigidbody.position = MyTransform.position;
                _lastGrabPosition = MyTransform.position;
            }

            MyTransform.position -= (MyTransform.position - _bottomDragging.MyTransform.position).normalized * _bottomDragging.GrabSpeed;
            onPull?.Invoke();
            _onPulling?.Invoke();
        }

        onPullEnd?.Invoke();
        _onPullingEnd?.Invoke();
        Destroy(gameObject);
        //}
    }

    private IEnumerator Projectiling(ITarget target)
    {
        bool isCollide = false;
        ICharacterLimiter[] targets = new ICharacterLimiter[0];

        Vector3 targetPosition = target.MyTransform.position;
        Vector3 position = MyTransform.position;
        position.y = _bottomDragging.HandHeightStart;
        MyTransform.position = position;

        //if (CheckDraggingIsDestroy() == false)
        //{
        while (isCollide == false && Vector3.Distance(MyTransform.position, _bottomDragging.MyTransform.position) < _bottomDragging.MaxDistance)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);

            //if (CheckDraggingIsDestroy())
            //    break;

            targets = Physics.OverlapBox(MyCollider.bounds.center, MyCollider.bounds.extents / 2, Quaternion.identity, _bottomDragging.TargetLayer)
                             .Select(n => n.GetComponent<ICharacterLimiter>()).ToArray();
            isCollide = targets.Length > 0;
            Vector3 lockTo = target.MyTransform.position - MyTransform.position;



            if (_bottomDragging.IsFollowToTarget)
                MyTransform.rotation = Quaternion.LookRotation(lockTo);
            else
                MyTransform.rotation = Quaternion.LookRotation( (targetPosition - position).normalized * 1000 );

            MyTransform.position += MyTransform.forward * _bottomDragging.ProjectileSpeed;

        }
        //}

        if (isCollide)
        {
            _graggableTarget = targets.First();
            BackWithTarget();
            _onGrabWithEnemy?.Invoke();
        }
        else
        {
            BackHand();
            _onGrabEmpty?.Invoke();
        }
    }

    private IEnumerator Projectiling()
    {
        bool isCollide = false;
        ICharacterLimiter[] targets = new ICharacterLimiter[0];
        Vector3 targetPosition = _bottomDragging.Selector.transform.position;
        float dist = 1000;
        targetPosition += _bottomDragging.Selector.transform.forward * dist;
        targetPosition.y = dist / 1.5f;
        Vector3 direction = (_bottomDragging.MyTransform.position - targetPosition).normalized * -1;
        direction.y = 0;
        Vector3 position = MyTransform.position;
        position.y = _bottomDragging.HandHeightStart;
        MyTransform.position = position;
        //if (CheckDraggingIsDestroy() == false)
        //{
        while (isCollide == false && Vector3.Distance(MyTransform.position, _bottomDragging.MyTransform.position) < _bottomDragging.MaxDistance)
        {
            //if (CheckDraggingIsDestroy())
            //    break;

            targets = Physics.OverlapBox(MyCollider.bounds.center, MyCollider.bounds.extents / 2, Quaternion.identity, _bottomDragging.TargetLayer)
                             .Select(n => n.GetComponent<ICharacterLimiter>()).ToArray();
            isCollide = targets.Length > 0;

            MyTransform.position += direction * _bottomDragging.ProjectileSpeed;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        //}

        if (isCollide)
        {
            _graggableTarget = targets.First();
            BackWithTarget();
            _onGrabWithEnemy?.Invoke();
        }
        else
        {
            BackHand();
            _onGrabEmpty?.Invoke();
        }
    }

    //private bool CheckDraggingIsDestroy()
    //{
    //    return false;

    //    if (_bottomDragging == null)
    //    {
    //        Destroy(gameObject);
    //        return true;
    //    }

    //    // это тут нахер не нужно, но без него верхний if иногда не работает
    //    if (_bottomDragging.Equals(null))
    //    {
    //        Destroy(gameObject);
    //        return true;
    //    }

    //    return false;
    //}

    private bool CheckTargetIsDestroy()
    {
        if (_graggableTarget == null)
        {
            Destroy(gameObject);
            return true;
        }

        // это тут нахер не нужно, но без него верхний if иногда не работает
        if (_graggableTarget.Equals(null))
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}
