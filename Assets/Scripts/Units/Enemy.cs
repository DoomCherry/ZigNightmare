using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(TargetSelector))]
[RequireComponent(typeof(DamageController))]
public class Enemy : MonoBehaviour, ITarget, ICharacterLimiter
{
    //-------PROPERTY
    public GameObject GameObject => gameObject;
    public DamageController DamageController => _damageController == null ? (gameObject == null ? null : GetComponent<DamageController>()) : _damageController;
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());

    public Rigidbody MyRigidbody
    {
        get
        {
            _rigidbody = _customRigidbody == null ? (_rigidbody == null ? GetComponent<Rigidbody>() : _rigidbody) : _customRigidbody;
            return _rigidbody;
        }
    }

    public TargetSelector Selector
    {
        get
        {
            _selector = _selector == null ? GetComponent<TargetSelector>() : _selector;
            return _selector;
        }
    }

    public Transform MyTransform
    {
        get
        {
            _myTransform = _myTransform == null ? transform : _myTransform;
            return _myTransform;
        }
    }

    public ISkill Skill => _totalSkill = _skill.GetComponent<ISkill>();

    public bool WalkingIsFreeze { get; set; } = false;
    public bool FallingIsFreeze { get; set; } = false;
    public bool JumpIsFreeze { get; set; } = false;
    public bool RotationIsFreeze { get; set; } = false;
    public bool SkillsIsFreeze { get; set; } = false;
    public bool IsPhysicTargetsDesable { get; set; } = false;
    public float Walkspeed => Skill != null && Skill.IsSkillActive ? _movespeedOnSkill : _movespeed;
    public float Viewspeed => Skill != null && Skill.IsSkillActive ? _viewSpeedOnSkill : _viewSpeed;




    //-------FIELD
    [SerializeField]
    private Animator _customAnimator;

    [SerializeField]
    private UnitContainer _unitContainer;

    [SerializeField]
    private string _prepareTriggerName = "PrepareToSkill";
    [SerializeField]
    private string _skillIsReadyBool = "BlastActivate";

    [SerializeField]
    private GameObject _skill;

    private ISkill _totalSkill;

    [SerializeField]
    private Rigidbody _customRigidbody;

    [SerializeField]
    private PhysicalBodyPart[] _physicTargets;

    [SerializeField]
    private float _lookToY = 1;

    protected float _viewSpeed = 0.1f;
    protected float _viewSpeedOnSkill = 0.1f;

    protected float _minDistanceToTarget = 3;

    protected float _movespeed = 2;
    protected float _movespeedOnSkill = 2;
    protected float _agressiveZoneRadius = 10;

    private TargetSelector _selector;
    private Transform _myTransform;
    private Rigidbody _rigidbody;
    private Coroutine _waitSkill;
    private Animator _animator;
    private Coroutine _prepareToSkill;
    private DamageController _damageController;
    private float _lastRigidYVelocity;

    protected Vector3 _currentVelocity;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onFallToFlore;
    public event UnityAction OnFallToFlore
    {
        add => _onFallToFlore.AddListener(value);
        remove => _onFallToFlore.RemoveListener(value);
    }




    //-------METODS
    protected virtual void Start()
    {
        Diselect();

        if (SelectorHandler.Instance == null)
            throw new System.NullReferenceException($"{name}: {SelectorHandler.Instance} is missing.");

        DamageController.SetMaxHp(_unitContainer._maxHp);
        _movespeed = _unitContainer._movespeed;
        _viewSpeed = _unitContainer.robotEnemy.viewSpeed;
        _minDistanceToTarget = _unitContainer.robotEnemy._minDistanceToTarget;
        _viewSpeedOnSkill = _unitContainer.robotEnemy._viewSpeedOnSkill;
        _movespeedOnSkill = _unitContainer.robotEnemy._speedOnSkill;
        _agressiveZoneRadius = _unitContainer.robotEnemy._agressiveZoneRadius;

        LevelControler.Instance.RegistryEnemy(this);

        SelectorHandler.Instance.RegisterTarget(this);
    }

    private void OnDestroy()
    {
        SelectorHandler.Instance.ForgetTarget(this);
    }
    protected virtual void ActivateSkill()
    {
        Skill.Activate();
        StopCoroutine(_waitSkill);
        _waitSkill = null;
    }

    protected virtual void FixedUpdate()
    {
        if (Selector.CurrentTarget == null)
            return;

        if (Vector3.Distance(Selector.CurrentTarget.MyTransform.position, MyTransform.position) > _agressiveZoneRadius)
            return;

        void Move()
        {
            if (Math.Abs(Math.Round(MyRigidbody.velocity.y, 1)) > 0)
                return;

            if (_minDistanceToTarget < Vector3.Distance(Selector.CurrentTarget.MyTransform.position, MyTransform.position))
            {
                _currentVelocity = (WalkingIsFreeze ? Vector3.zero : MyRigidbody.velocity + MyTransform.forward).normalized * Walkspeed;
                MyRigidbody.velocity = new Vector3(_currentVelocity.x, !FallingIsFreeze ? MyRigidbody.velocity.y : 0, _currentVelocity.z);
            }
            else
            {
                _currentVelocity = Vector3.zero;
            }
        }

        Move();

        if (Math.Round(_lastRigidYVelocity,2) != 0 && MyRigidbody.velocity.y == 0)
            _onFallToFlore?.Invoke();

        _lastRigidYVelocity = MyRigidbody.velocity.y;

        void UseSkill()
        {
            if (Skill.IsSkillActive == false && _waitSkill == null)
            {
                _waitSkill = this.WaitSecond(Skill.SkillContainer.coldown, ActivateSkill);
            }
        }

        if (!SkillsIsFreeze)
            UseSkill();

        if (!RotationIsFreeze)
        {
            UpdateRotation();
        }
    }

    protected virtual void UpdateRotation()
    {
        Vector3 myPositionDir = MyTransform.position;
        myPositionDir.y = _lookToY;

        Vector3 targetDir = Selector.CurrentTarget.MyTransform.position;
        targetDir.y = _lookToY;

        Quaternion nextView = Quaternion.LookRotation((targetDir - myPositionDir).normalized * 100);

        MyRigidbody.rotation = Quaternion.Slerp(MyRigidbody.rotation, nextView, Viewspeed);
    }

    public virtual void Select(Color color)
    {
    }

    public virtual void Diselect()
    {
    }

    public void FreezeRotation()
    {
        RotationIsFreeze = true;
    }

    public void UnfreezeRotation()
    {
        RotationIsFreeze = false;
    }

    public void PrepareToSkill(ISkill skill, Action afterPrepare)
    {
        if (_prepareToSkill != null)
            return;

        Animator.SetTrigger(_prepareTriggerName);

        IEnumerator PreparedState()
        {
            while (Animator.GetBool(_skillIsReadyBool) == false)
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            Animator.SetBool(_skillIsReadyBool, false);
            _prepareToSkill = null;
            afterPrepare?.Invoke();
        }

        _prepareToSkill = StartCoroutine(PreparedState());
    }

    public void FreezeWalking()
    {
        WalkingIsFreeze = true;
    }

    public void UnfreezeWalking()
    {
        WalkingIsFreeze = false;
    }

    public void FreezeSkill()
    {
        SkillsIsFreeze = true;
    }

    public void UnfreezeSkill()
    {
        SkillsIsFreeze = false;
    }

    public void FreezeFalling()
    {
        FallingIsFreeze = true;
    }

    public void UnfreezeFalling()
    {
        FallingIsFreeze = false;
    }

    public void JumpFreeze()
    {
        JumpIsFreeze = true;
    }

    public void JumpUnfreeze()
    {
        JumpIsFreeze = false;
    }

    public void DisablePhysicsTarget()
    {
        foreach (var item in _physicTargets)
        {
            item.enabled = false;
        }

        IsPhysicTargetsDesable = true;
    }

    public void EnablePhysicsTarget()
    {
        foreach (var item in _physicTargets)
        {
            item.enabled = true;
        }

        IsPhysicTargetsDesable = false;
    }

    public void TakeDamage(float damage)
    {
        if (DamageController != null)
            DamageController.TakeDamage(damage);
    }
}
