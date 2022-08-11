
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

    public bool OnFlore
    {
        get
        {
            var direction = new Vector3 { [MyCollider.direction] = 1 };
            float capsuleRadius = MyCollider.radius * _groundTestColliderRadius;

            var offset = MyCollider.height / 2 - capsuleRadius;
            var localPoint0 = (MyCollider.bounds.center - direction * offset) + _groundTestColliderOffset;
            var localPoint1 = (MyCollider.bounds.center + direction * offset) + _groundTestColliderOffset;

            Collider[] colliders = Physics.OverlapCapsule(localPoint0, localPoint1, capsuleRadius, _jumpColliderLayer, QueryTriggerInteraction.Ignore);
            if (colliders.Length > 0)
            {
                _currentSloppingFlore = colliders.FirstOrDefault(n => n.gameObject.layer == _slopingFloreLayer);
                return true;
            }

            _currentSloppingFlore = null;
            return false;
        }
    }

    public float CurrentStamina => _currentStamina;
    public float MaxStamina => _maxStamina;
    public Vector3 WalkDirection => MyTransform.position - _lastPosition;
    public bool IsSlipping => _currentSloppingFlore != null &&
                               MyRigidbody.velocity.y < 0.1f &&
                               Math.Round(WalkDirection.magnitude, 1) != 0;

    public bool IsFullSystemFreeze { get; set; }




    //-------FIELD
    [SerializeField]
    private string _horizontalAxisName = "Horizontal", _verticalAxisName = "Vertical";

    [SerializeField]
    private float _maxSkillStealDistance = 10;

    [SerializeField]
    private int _slopingFloreLayer;
    [SerializeField]
    private Vector3 _groundTestColliderOffset;
    [SerializeField]
    private float _groundTestColliderRadius = 1;

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
    private bool _dashButtomIsActive = false, _isDashing = false;
    private bool _previousOnFloreState = false;
    private bool _lastIsSlipping = false;
    private List<Vector3> _lastOnFlorePoints = new List<Vector3>();
    private int _maxOnFlorePoint = 20;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onDashNotActive;
    public event UnityAction OnDashNotActive
    {
        add => _onDashNotActive.AddListener(value);
        remove => _onDashNotActive.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onSkillNotActive;
    public event UnityAction OnSkillNotActive
    {
        add => _onSkillNotActive.AddListener(value);
        remove => _onSkillNotActive.RemoveListener(value);
    }

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseManager.Instance.SetPause(!PauseManager.Instance.IsInPause);
        }
    }

    private void FixedUpdate()
    {
        if (OnFlore)
        {
            if (_lastOnFlorePoints.Count > _maxOnFlorePoint)
                _lastOnFlorePoints.RemoveAt(0);

            _lastOnFlorePoints.Add(MyTransform.position);
        }

        _onUpdate?.Invoke();

        void Move()
        {
            Ray ray = _main.ScreenPointToRay(Input.mousePosition);
            LinearFunction3 line = new LinearFunction3(ray.origin * -1, ray.direction);

            if (!WalkingIsFreeze)
            {
                _mousePointInWorld = line.GetCoordsFromProjectionY(MyTransform.position.y);
                _mousePointInWorld.y = _lookToY;
            }

            MyRigidbody.useGravity = !FallingIsFreeze;

            _walkDirection = new Vector3(Input.GetAxisRaw(_horizontalAxisName),0, Input.GetAxisRaw(_verticalAxisName));

            Vector3 walkSpeed = Vector3.zero;

            if (Input.GetAxisRaw(_verticalAxisName) != 0 && !WalkingIsFreeze)
            {
                int axisSign = (int)Mathf.Sign(Input.GetAxisRaw(_verticalAxisName));
                float newSpeed = MyRigidbody.velocity.z + (_velocitySpeed * axisSign);
                float fixSpeed = Mathf.Clamp(newSpeed, -_speed, _speed);

                walkSpeed.z = _velocitySpeed - Mathf.Abs(newSpeed - fixSpeed);
            }

            if (Input.GetAxisRaw(_horizontalAxisName) != 0 && !WalkingIsFreeze)
            {
                int axisSign = (int)Mathf.Sign(Input.GetAxisRaw(_horizontalAxisName));
                float newSpeed = MyRigidbody.velocity.x + (_velocitySpeed * axisSign);
                float fixSpeed = Mathf.Clamp(newSpeed, -_speed, _speed);

                walkSpeed.x = _velocitySpeed - Mathf.Abs(newSpeed - fixSpeed);
            }

            if (Input.GetKey(KeyCode.Space) && !JumpIsFreeze && OnFlore)
            {
                Vector3 oldVelocity = MyRigidbody.velocity;
                MyRigidbody.velocity = new Vector3(oldVelocity.x, 0, oldVelocity.z);
                _walkDirection.y += 1;

                _onJump?.Invoke();
            }

            if (Input.GetAxisRaw(_horizontalAxisName) == 0 && Input.GetAxisRaw(_verticalAxisName) == 0)
            {
                if (_walkDirection.x == 0 && _walkDirection.y == 0 && _isDashing == false)
                {
                    StopMove();
                }
            }

            void StopMove()
            {
                MyRigidbody.velocity = new Vector3(0, MyRigidbody.velocity.y, 0);
            }

            float dashSpeed = 0;
            void UseDash()
            {
                _currentStamina = _currentStamina >= _maxStamina ? _maxStamina : _currentStamina + (_staminaRecoverPerSecond * Time.deltaTime);
                _onStaminaChange?.Invoke();

                if (Input.GetKey(KeyCode.LeftShift) && _currentStamina <= _staminaPerOnceDash && !_dashButtomIsActive)
                {
                    _onDashNotActive?.Invoke();
                    _dashButtomIsActive = true;
                }

                if (Input.GetKey(KeyCode.LeftShift) && _currentStamina >= _staminaPerOnceDash && !_dashButtomIsActive)
                {
                    Vector3 dashPosition = MyTransform.position + (MyTransform.forward);
                    dashPosition.y = 100;

                    Ray ray = new Ray(dashPosition, Vector3.down);
                    Physics.Raycast(ray, out RaycastHit info, 200, _jumpColliderLayer, QueryTriggerInteraction.Ignore);

                    if (_currentSloppingFlore != null)
                    {
                        dashSpeed = 1;
                        Vector3 findingInfo = info.point;
                        MyRigidbody.velocity += (findingInfo - MyTransform.position).normalized * _dashSpeed;
                    }
                    else
                    {
                        dashSpeed += _dashSpeed;
                        FreezeFalling();
                    }

                    _currentStamina -= _staminaPerOnceDash;
                    _dashButtomIsActive = true;
                    _isDashing = true;
                    _onDash?.Invoke();
                    this.WaitSecond(_dashTime, delegate
                    {
                        _isDashing = false;
                        UnfreezeFalling();
                        StopMove();
                    });
                }

                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    AfterDashTime();
                }

                void AfterDashTime()
                {
                    _dashButtomIsActive = false;
                }

            }

            UseDash();

            Vector2 moveNormalize = new Vector2(_walkDirection.x, _walkDirection.z).normalized;
            _walkDirection = new Vector3(moveNormalize.x, _walkDirection.y, moveNormalize.y);

            walkSpeed = _isDashing ? Vector3.zero : walkSpeed;

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
                    Vector3 targetPosition = TargetSelector.CurrentTarget.MyTransform.position;
                    targetPosition.y = _lookToY;

                    Vector3 dir = (targetPosition - myPositionDir).normalized * 100;
                    if (dir.x != 0 || dir.z != 0)
                        MyRigidbody.rotation = Quaternion.LookRotation(dir);

                }
                else
                {
                    Vector3 dir = (_mousePointInWorld - myPositionDir).normalized;

                    if (dir.x != 0 || dir.z != 0)
                        MyRigidbody.rotation = Quaternion.LookRotation(dir);

                }
            }


            void SetAnimation()
            {
                Vector3 loockDirection = MyTransform.forward;
                AnimationControiler.SetMovement(new Vector2(_walkDirection.x, _walkDirection.z), new Vector2(loockDirection.x, loockDirection.z));
                AnimationControiler.SetJump(MyRigidbody, _walkDirection.y > 0, MyRigidbody.velocity.y < 0 && !OnFlore);
                AnimationControiler.SetDash(_isDashing);
                AnimationControiler.UseSlippin(IsSlipping);
            }
            SetAnimation();
        }
        Move();

        if (_previousOnFloreState != OnFlore && _previousOnFloreState == false)
            _onFallToFlore?.Invoke();

        _previousOnFloreState = OnFlore;

        void Slipping()
        {
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
            if (Input.GetKeyDown(KeyCode.Mouse0) && Skill != null && !Skill.IsCharging)
            {
                if (CheckSkillToCooldown() && !SkillsIsFreeze)
                {
                    UseNormalSkill();
                }
                else
                {
                    _onSkillNotActive?.Invoke();
                }
            }
        }
        TryUseNormalSkill();

        void TryUseChargingSkill()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && Skill != null && Skill.IsCharging)
            {
                if (CheckSkillToCooldown() && !SkillsIsFreeze)
                {
                    _coldownList.Add(Skill.SkillContainer, Time.time);
                    UseChargingSkill();
                }
                else
                {
                    _onSkillNotActive?.Invoke();
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
            if (TargetSelector.CurrentTarget != null)
            {
                if (Vector3.Distance(TargetSelector.CurrentTarget.MyTransform.position, MyTransform.position) < _maxSkillStealDistance)
                {
                    ISkill newSkill = MySkillStealler.GetTargetSkill();
                    if (newSkill != null && !Skill.IsSkillActive)
                    {
                        Skill.Stop();
                        _skill.gameObject.SetActive(false);
                        _skill = newSkill.Self;
                        _skill.gameObject.SetActive(true);
                        _onSkillChange?.Invoke();
                    }
                }
            }
        }

        _lastPosition = MyTransform.position;
    }

    public void ReturnLastFlorePoint()
    {
        MyRigidbody.position = _lastOnFlorePoints[0];
        MyRigidbody.velocity = Vector3.zero;
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
        if (Skill != null)
            Skill.Stop();
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

    public void FullSystemFreeze()
    {
        IsFullSystemFreeze = true;
        FreezeFalling();
        FreezeRotation();
        FreezeWalking();
        FreezeSkill();
        JumpFreeze();
        AnimationControiler.SetAnimationSpeed(0);
    }

    public void FullSystemUnfreeze()
    {
        IsFullSystemFreeze = false;
        UnfreezeFalling();
        UnfreezeRotation();
        UnfreezeSkill();
        UnfreezeWalking();
        JumpUnfreeze();
        AnimationControiler.SetAnimationSpeed(1);
    }
}
