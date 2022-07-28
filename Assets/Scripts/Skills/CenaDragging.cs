using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenaDragging : MonoBehaviour, ISkill
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
    public TargetSelector Selector => _selector;
    public float MaxWaitingTime => _maxWaitingTime > 0 ? _maxWaitingTime : 1;
    public LayerMask TargetLayer => _targetLayer;
    public string AnimationStateNameInteger => _animationStateNameInteger;
    public float TotalDamage => _totalDamage;
    public bool IsSkillActive => _skillIsActive;
    public ICharacterLimiter Limiter
    {
        get
        {
            if (_character != null)
            {
                _characterLimiter = _characterLimiter == null ? _character.GetComponent<ICharacterLimiter>() : _characterLimiter;
                return _characterLimiter;
            }

            return null;
        }
    }
    public SkillContainer SkillContainer => _skillContainer;

    public GameObject Self => gameObject;
    public CenaDraggingHandControiler CurrentHand { get; private set; }
    public bool IsCharging => false;




    //-------FIELD
    [SerializeField]
    private SkillContainer _skillContainer;
    [SerializeField]
    private bool _isInFlore = false;

    [SerializeField]
    private CenaDraggingHandControiler _hand;
    [SerializeField]
    private TargetSelector _selector;

    [SerializeField]
    private LayerMask _targetLayer;
    [SerializeField]
    private LayerMask _floreLayer;
    [SerializeField]
    private float _armHeight = 1;
    [SerializeField]
    private string _animationStateNameInteger = "HandStage";
    [SerializeField]
    private GameObject _character;
    private ICharacterLimiter _characterLimiter;

    private float _totalDamage = 50;
    private float _maxWaitingTime = 6;
    private float _punchSpeed = 3;
    private Transform _myTransform;
    private bool _skillIsActive = false;
    private CenaDraggingHandControiler _instance;




    //-------METODS
    private void OnValidate()
    {
        if (_character != null && _character.GetComponent<ICharacterLimiter>() == null)
            _character = null;
    }

    private void Start()
    {
        _totalDamage = _skillContainer.cenaPunchInfo.punchDamage;
        _maxWaitingTime = _skillContainer.cenaPunchInfo.maxWaitingTime;
        _punchSpeed = _skillContainer.cenaPunchInfo.punchSpeed;
    }

    private void FixedUpdate()
    {
        if (_isInFlore == false)
            return;

        Vector3 postion = MyTransform.position;
        postion.y = 100;

        Ray ray = new Ray(postion, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 200, _floreLayer))
        {
            Vector3 position = MyTransform.position;
            position.y = info.point.y + _armHeight;
            MyTransform.position = position;
        }
    }

    public void Activate()
    {
        _skillIsActive = true;
        _instance = Instantiate(_hand, MyTransform.position, Quaternion.identity, transform);
        _instance.MyTransform.rotation = _hand.MyTransform.rotation;
        _instance.gameObject.SetActive(true);
        _instance.GrabIt(this);


        _instance.OnPullingEnd += OnHandPullingEnd;
        _instance.OnEnd += OnHandPullingEnd;

        if (CurrentHand != null)
            Destroy(CurrentHand.gameObject);

        CurrentHand = _instance;
    }
    private void OnHandPullingEnd()
    {
        if (_instance != null)
        {
            _instance.OnPullingEnd -= OnHandPullingEnd;
            _instance.OnEnd -= OnHandPullingEnd;
            _skillIsActive = false;
            Destroy(_instance.gameObject);
        }
    }

    public void Stop()
    {
        Limiter.UnfreezeWalking();
        Limiter.UnfreezeRotation();
        _instance.Stop();
    }
}
