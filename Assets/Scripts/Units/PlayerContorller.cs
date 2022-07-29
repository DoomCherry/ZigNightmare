
using UnityEngine;
using Doomchery;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(CapsuleCollider))]
[RequireComponent(typeof(DamageController), typeof(SkillStealler), typeof(PlayerAnimationControiler))]
[RequireComponent(typeof(TargetSelector))]
public class PlayerContorller : MonoBehaviour, ICharacterLimiter, ITarget
{
    //-------PROPERTY
    public GameObject GameObject => gameObject;
    public PlayerAnimationControiler AnimationControiler => _playerAnimationControiler == null ? GetComponent<PlayerAnimationControiler>() : _playerAnimationControiler;
    public DamageController DamageController => _damageController == null ? GetComponent<DamageController>() : _damageController;
    public SkillStealler MySkillStealler => _skillStealler == null ? GetComponent<SkillStealler>() : _skillStealler;
    public TargetSelector TargetSelector => _selector == null ? GetComponent<TargetSelector>() : _selector;
    public Transform MyTransform => _myTransform = _myTransform ?? transform;
    public Rigidbody MyRigidbody => _rigidbody = _rigidbody ?? GetComponent<Rigidbody>();
    public CapsuleCollider MyCollider => _collider = _collider ?? GetComponent<CapsuleCollider>();
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());
    public ISkill Skill => _totalSkill = _skill.GetComponent<ISkill>();
    public bool WalkingIsFreeze { get; set; } = false;
    public bool RotationIsFreeze { get; set; } = false;
    public bool SkillsIsFreeze { get; set; } = false;
    public bool FallingIsFreeze { get; set; } = false;
    public bool JumpIsFreeze { get; set; } = false;
    public bool IsPhysicTargetsDesable { get; set; } = false;
    public IReadOnlyDictionary<SkillContainer, float> ColdownList => _coldownList;
    //public bool OnFlore
    //{
    //    get
    //    {
    //        Vector3 postion = MyTransform.position;
    //        postion.y = 100;

    //        Ray ray = new Ray(postion, Vector3.down);

    //        Vector3 colliderCenter = MyCollider.center + MyTransform.position;
    //        float pointHeight = colliderCenter.y - (MyCollider.height * MyTransform.lossyScale.y / 2);

    //        if (Physics.Raycast(ray, out RaycastHit info, 200, _jumpColliderLayer, QueryTriggerInteraction.Ignore))
    //        {
    //            if (Math.Round(Mathf.Abs(info.point.y - pointHeight), 2) <= 0)
    //            {
    //                return true;
    //            }
    //        }

    //        return false;
    //    }
    //}

    public bool OnFlore
    {
        get
        {
            if (_groundTestCollider == null)
            {
                var direction = new Vector3 { [MyCollider.direction] = 1 };
                var offset = MyCollider.height / 2 - MyCollider.radius;
                var localPoint0 = MyCollider.bounds.center - direction * offset;
                var localPoint1 = MyCollider.bounds.center + direction * offset;

                Collider[] colliders = Physics.OverlapCapsule(localPoint0, localPoint1, MyCollider.radius, _jumpColliderLayer, QueryTriggerInteraction.Ignore);
                if (colliders.Length > 0)
                {
                    _currentSloppingFlore = colliders.FirstOrDefault(n => n.gameObject.layer == _slopingFloreLayer);
                    return true;
                }

                _currentSloppingFlore = null;
                return false;
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(_groundTestCollider.bounds.center, _groundTestCollider.radius, _jumpColliderLayer, QueryTriggerInteraction.Ignore);
                if (colliders.Length > 0)
                {
                    _currentSloppingFlore = colliders.FirstOrDefault(n => n.gameObject.layer == _slopingFloreLayer);
                    return true;
                }

                _currentSloppingFlore = null;
                return false;
            }
        }
    }

    public float CurrentStamina => _currentStamina;
    public float MaxStamina => _maxStamina;
    public Vector3 WalkDirection => MyTransform.position - _lastPosition;
    public bool IsSlipping => _currentSloppingFlore != null && MyRigidbody.velocity.y < 0.1f && Math.Round(WalkDirection.magnitude, 1) != 0;




    //-------FIELD
    [SerializeField]
    private int _slopingFloreLayer;
    [SerializeField]
    private SphereCollider _groundTestCollider;

    [SerializeField]
    private UnitContainer _unitContainer;

    [SerializeField]
    private LayerMask _jumpColliderLayer;

    [SerializeField]
    private Animator _customAnimator;

    [SerializeField]
    private GameObject _skill;

    private ISkill _totalSkill;

    [SerializeField]
    private float _lookToY = 1;

    private Collider _currentSloppingFlore;

    private float _jumpPower = 5;

    private float _speed = 1;
    private float _dashSpeed = 1;
    private float _maxStamina = 1;
    private float _currentStamina = 1;
    private float _staminaPerOnceDash = 1;
    private float _staminaRecoverPerSecond = 1;
    private float _velocitySpeed = 1;
    private float _dashTime = 1;

    private TargetSelector _selector;
    private PlayerAnimationControiler _playerAnimationControiler;
    private Vector3 _walkDirection;
    private Vector3 _lastPosition;
    private Transform _myTransform;
    private Rigidbody _rigidbody;
    private Vector3 _mousePointInWorld;
    private Animator _animator;
    private Camera _main;
    private Coroutine _prepareToSkill;
    private CapsuleCollider _collider;
    private DamageController _damageController;
    private SkillStealler _skillStealler;
    private Dictionary<SkillContainer, float> _coldownList = new Dictionary<SkillContainer, float>();
    private bool _dashButtomIsActive = false;
    private bool _previousOnFloreState = false;
    private bool _lastIsSlipping = false;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onUpdate;
    public event UnityAction OnUpdate
    {
        add => _onUpdate.AddListener(value);
        remove => _onUpdate.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onNotSlipping;
    public event UnityAction OnNotSlipping
    {
        add => _onNotSlipping.AddListener(value);
        remove => _onNotSlipping.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onSlipping;
    public event UnityAction OnSlippin
    {
        add => _onSlipping.AddListener(value);
        remove => _onSlipping.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onJump;
    public event UnityAction OnJump
    {
        add => _onJump.AddListener(value);
        remove => _onJump.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onFallToFlore;
    public event UnityAction OnFallToFlore
    {
        add => _onFallToFlore.AddListener(value);
        remove => _onFallToFlore.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onDash;
    public event UnityAction OnDash
    {
        add => _onDash.AddListener(value);
        remove => _onDash.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onSkillChange;
    public event UnityAction OnSkillChange
    {
        add => _onSkillChange.AddListener(value);
        remove => _onSkillChange.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onStaminaChange;
    public event UnityAction OnStaminaChange
    {
        add => _onStaminaChange.AddListener(value);
        remove => _onStaminaChange.RemoveListener(value);
    }




    //-------METODS
    private void OnValidate()
    {
        if (_skill.GetComponent<ISkill>() == null)
        {
            _skill = null;
            Debug.LogWarning($"{name}: skill most be a {nameof(ISkill)}");
        }
    }

    private void Start()
    {
        DamageController.SetMaxHp(_unitContainer._maxHp);
        _jumpPower = _unitContainer.playerInfo.jumpPower;
        _speed = _unitContainer._movespeed;
        _velocitySpeed = _unitContainer._velocity;
        _dashSpeed = _unitContainer.playerInfo.dashPower;
        _dashTime = _unitContainer.playerInfo._dashTime;
        _maxStamina = _unitContainer.playerInfo.maxStamina;
        _currentStamina = _maxStamina;
        _staminaPerOnceDash = _unitContainer.playerInfo.staminaToOnceDash;
        _staminaRecoverPerSecond = _unitContainer.playerInfo.staminaRecoverPerSecond;

        _main = Camera.main;
    }


    private void Update()
    {
        _onUpdate?.Invoke();

        void Move()
        {
            Ray ray = _main.ScreenPointToRay(Input.mousePosition);

            LinearFunction3 line = new LinearFunction3(ray.origin * -1, ray.direction);

            if (!WalkingIsFreeze)
            {
                _mousePointInWorld = line.GetCoordsFromProjectionY(0);
                _mousePointInWorld.y = _lookToY;
            }

            MyRigidbody.useGravity = !FallingIsFreeze;

            _walkDirection = Vector3.zero;
            Vector3 walkSpeed = Vector3.zero;

            if (Input.GetKey(KeyCode.W) && !WalkingIsFreeze)
            {
                _walkDirection += Vector3.forward;
                float newSpeed = MyRigidbody.velocity.z + _velocitySpeed;
                float fixSpeed = Mathf.Clamp(newSpeed, -_speed, _speed);

                if (Mathf.Abs(newSpeed) >= _speed)
                    walkSpeed.z = 0;
                else
                    walkSpeed.z = _velocitySpeed - Mathf.Abs(newSpeed - fixSpeed);
            }

            if (Input.GetKey(KeyCode.A) && !WalkingIsFreeze)
            {
                _walkDirection += -1 * Vector3.right;
                float newSpeed = MyRigidbody.velocity.x - _velocitySpeed;
                float fixSpeed = Mathf.Clamp(newSpeed, -_speed, _speed);

                if (Mathf.Abs(newSpeed) >= _speed)
                    walkSpeed.z = 0;
                else
                    walkSpeed.x = _velocitySpeed - Mathf.Abs(newSpeed - fixSpeed);
            }

            if (Input.GetKey(KeyCode.S) && !WalkingIsFreeze)
            {
                _walkDirection += -1 * Vector3.forward;
                float newSpeed = MyRigidbody.velocity.z - _velocitySpeed;
                float fixSpeed = Mathf.Clamp(newSpeed, -_speed, _speed);

                if (Mathf.Abs(newSpeed) >= _speed)
                    walkSpeed.z = 0;
                else
                    walkSpeed.z = _velocitySpeed - Mathf.Abs(newSpeed - fixSpeed);
            }

            if (Input.GetKey(KeyCode.D) && !WalkingIsFreeze)
            {
                _walkDirection += Vector3.right;
                float newSpeed = MyRigidbody.velocity.x + _velocitySpeed;
                float fixSpeed = Mathf.Clamp(newSpeed, -_speed, _speed);

                if (Mathf.Abs(newSpeed) >= _speed)
                    walkSpeed.z = 0;
                else
                    walkSpeed.x = _velocitySpeed - Mathf.Abs(newSpeed - fixSpeed);
            }

            if (Input.GetKey(KeyCode.Space) && !JumpIsFreeze && OnFlore)
            {
                Vector3 oldVelocity = MyRigidbody.velocity;
                MyRigidbody.velocity = new Vector3(oldVelocity.x, 0, oldVelocity.z);
                _walkDirection.y += 1;

                _onJump?.Invoke();
            }

            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A))
            {
                if (_walkDirection.x == 0 && _walkDirection.y == 0 && _dashButtomIsActive == false && !IsSlipping)
                {
                    StopMove();
                }
            }

            void StopMove()
            {
                Vector3 oldVelocity = MyRigidbody.velocity;
                MyRigidbody.velocity = new Vector3(0, oldVelocity.y, 0);
            }

            float dashSpeed = 0;
            void UseDash()
            {
                _currentStamina = _currentStamina >= _maxStamina ? _maxStamina : _currentStamina + (_staminaRecoverPerSecond * Time.deltaTime);
                _onStaminaChange?.Invoke();


                if (Input.GetKey(KeyCode.LeftShift) && _currentStamina >= _staminaPerOnceDash && !_dashButtomIsActive)
                {
                    Vector3 dashPosition = MyTransform.position + (MyTransform.forward * _dashSpeed);
                    dashPosition.y = 100;
                    Ray ray = new Ray(dashPosition, Vector3.down);
                    Physics.Raycast(ray, out RaycastHit info, 200, _jumpColliderLayer, QueryTriggerInteraction.Ignore);

                    
                    if (_currentSloppingFlore != null)
                    {
                        dashSpeed = 1;

                        MyRigidbody.velocity += (info.point - MyTransform.position).normalized * 3;
                    }
                    else
                    {
                        dashSpeed += _dashSpeed;
                    }

                    _currentStamina -= _staminaPerOnceDash;
                    _dashButtomIsActive = true;
                    FreezeFalling();
                    _onDash?.Invoke();
                    this.WaitSecond(_dashTime, delegate
                    {
                        UnfreezeFalling();
                        AfterDashTime();
                        StopMove();
                    });
                }

                void AfterDashTime()
                {
                    _dashButtomIsActive = false;
                }

            }
            UseDash();

            Vector2 moveNormalize = new Vector2(_walkDirection.x, _walkDirection.z).normalized;
            _walkDirection = new Vector3(moveNormalize.x, _walkDirection.y, moveNormalize.y);

            walkSpeed = _dashButtomIsActive ? Vector3.zero : walkSpeed;

            MyRigidbody.velocity = new Vector3(
                                    MyRigidbody.velocity.x + (_walkDirection.x * dashSpeed) + (_walkDirection.x * Mathf.Abs(walkSpeed.x)),
                                    FallingIsFreeze ? 0.2f : Mathf.Clamp(MyRigidbody.velocity.y + _walkDirection.y * _jumpPower, -_jumpPower, _jumpPower),
                                    MyRigidbody.velocity.z + (_walkDirection.z * dashSpeed) + (_walkDirection.z * Mathf.Abs(walkSpeed.z)));

            if (!RotationIsFreeze && (Mathf.Abs(_walkDirection.x) > 0 || Mathf.Abs(_walkDirection.z) > 0))
            {
                Vector3 myPositionDir = MyTransform.position;
                myPositionDir.y = _lookToY;

                if (TargetSelector.CurrentTarget != null)
                {
                    MyRigidbody.rotation = Quaternion.LookRotation(TargetSelector.CurrentTarget.MyTransform.position - myPositionDir);
                }
                else
                {
                    MyRigidbody.rotation = Quaternion.LookRotation((_mousePointInWorld - myPositionDir).normalized * 100);
                }
            }

            void SetAnimation()
            {
                Vector3 loockDirection = MyTransform.forward;
                AnimationControiler.SetMovement(new Vector2(_walkDirection.x, _walkDirection.z), new Vector2(loockDirection.x, loockDirection.z));
                AnimationControiler.SetJump(MyRigidbody, _walkDirection.y > 0);
                AnimationControiler.SetDash(_dashButtomIsActive);
            }
            SetAnimation();
        }
        Move();

        if (_previousOnFloreState != OnFlore && _previousOnFloreState == false)
            _onFallToFlore?.Invoke();

        _previousOnFloreState = OnFlore;

        void Slipping()
        {
            Vector3 dir = WalkDirection;
            dir.y = 0;

            if (IsSlipping)
            {
                MyRigidbody.rotation = Quaternion.LookRotation(dir.normalized);
            }

            if (IsSlipping != _lastIsSlipping)
            {
                if (IsSlipping)
                {
                    _onSlipping?.Invoke();
                }
                else
                {
                    _onNotSlipping?.Invoke();
                }

                _lastIsSlipping = IsSlipping;
            }
        }
        Slipping();

        void TryUseNormalSkill()
        {
            if (Input.GetKey(KeyCode.Mouse0) && !SkillsIsFreeze)
            {
                if (CheckSkillToCooldown())
                {
                    UseNormalSkill();
                }
            }
        }
        TryUseNormalSkill();

        void TryUseChargingSkill()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !SkillsIsFreeze)
            {
                if (CheckSkillToCooldown())
                {
                    _coldownList.Add(Skill.SkillContainer, Time.time);
                    UseChargingSkill();
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && !SkillsIsFreeze && Skill != null && Skill.IsSkillActive)
            {
                UseChargingSkill();
            }
        }
        TryUseChargingSkill();

        if (Input.GetKey(KeyCode.Mouse1))
        {
            ISkill newSkill = MySkillStealler.GetTargetSkill();
            if (newSkill != null)
            {

                _skill.gameObject.SetActive(false);
                _skill = newSkill.Self;
                _skill.gameObject.SetActive(true);
                _onSkillChange?.Invoke();
            }
        }

        _lastPosition = MyTransform.position;
    }

    private bool CheckSkillToCooldown()
    {
        if (!_coldownList.TryGetValue(Skill.SkillContainer, out float lastTime))
        {
            return true;
        }
        else
        {
            if (Time.time - lastTime > Skill.SkillContainer.coldown)
            {
                _coldownList.Remove(Skill.SkillContainer);
                return true;
            }
        }
        return false;
    }

    private void UseNormalSkill()
    {
        if (Skill != null && !Skill.IsCharging)
        {
            _coldownList.Add(Skill.SkillContainer, Time.time);
            AnimationControiler.UseSkill(Skill);
            AnimationControiler.UsePrepareToSkillAnimation(Skill, this);
        }
    }

    private void UseChargingSkill()
    {
        if (Skill != null && Skill.IsCharging)
        {
            AnimationControiler.UseChargeSkill(Skill, this);
        }
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
        IsPhysicTargetsDesable = true;
    }

    public void EnablePhysicsTarget()
    {
        IsPhysicTargetsDesable = false;
    }

    public void Select(Color color)
    {

    }

    public void Diselect()
    {

    }

    public void TakeDamage(float damage)
    {
        DamageController.TakeDamage(damage);
    }
}
