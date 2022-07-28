using UnityEngine;

public class BottomDragging : MonoBehaviour, ISkill
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
    public float HandHeightStart => _handHeightEnd;
    public bool IsFollowToTarget => _isFollowToTarget;
    public float MaxDistance => _maxDistance;
    public LayerMask TargetLayer => _targetLayer;
    public float ProjectileSpeed => _projectileSpeed;
    public float GrabSpeed => _grabSpeed;
    public float StopDistance => _stopDistance;
    public string AnimationStateNameInteger => _animationStateNameInteger;
    public float DamagePerOneUnit => _damagePerOneUnit;
    public bool IsSkillActive => _skillIsActive;

    public SkillContainer SkillContainer => _skillContainer;
    public BottomDraggingHandControiler CurrentHand => _handInstance;

    public GameObject Self => gameObject;
    public bool IsCharging => false;




    //-------FIELD

    [SerializeField]
    private SkillContainer _skillContainer;
    [SerializeField]
    private BottomDraggingHandControiler _hand;
    [SerializeField]
    private float _handHeightEnd = 2;
    [SerializeField]
    private bool _isFollowToTarget = false;
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
    private bool _armIsFollowToUser = true;

    private float _damagePerOneUnit = 1;
    private float _maxDistance = 6;
    private float _projectileSpeed = 1;
    private float _grabSpeed = 1;
    private float _stopDistance = 1;

    private Transform _myTransform;
    private bool _skillIsActive = false;
    private BottomDraggingHandControiler _handInstance;




    //-------METODS
    private void Start()
    {
        _damagePerOneUnit = _skillContainer.downScorpionInfo.damagePerUnit;
        _maxDistance = _skillContainer.downScorpionInfo.maxDistance;
        _projectileSpeed = _skillContainer.downScorpionInfo.projectileSpeed;
        _grabSpeed = _skillContainer.downScorpionInfo.grabSpeed;
        _stopDistance = _skillContainer.downScorpionInfo.stopDistance;
    }

    private void FixedUpdate()
    {
        if (_armIsFollowToUser == false)
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

    public enum BottomDraggingState
    {
        Grab = 0,
        Pull,
        NotReady
    }

    public void Activate()
    {
        _handInstance = Instantiate(_hand, MyTransform.position, Quaternion.identity);
        _handInstance.MyTransform.rotation = _hand.MyTransform.rotation;
        _handInstance.MyTransform.position = MyTransform.position;
        _handInstance.gameObject.SetActive(true);
        _handInstance.ReleaseHand(this);
        _skillIsActive = true;
        _handInstance.OnPullingEnd += Stop;
    }
    public void Stop()
    {
        if (_handInstance != null)
        {
            _handInstance.OnPullingEnd -= Stop;
            _skillIsActive = false;
            Destroy(_handInstance.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_handInstance != null)
            Destroy(_handInstance.gameObject);
    }
}
