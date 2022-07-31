using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Minigun : MonoBehaviour, ISkill
{
    //-------PROPERTY
    private Transform MyTransform
    {
        get
        {
            _myTransform = _myTransform == null ? transform : _myTransform;
            return _myTransform;
        }
    }
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());

    public bool IsSkillActive => _skillIsActive;

    public SkillContainer SkillContainer => _skillContainer;

    public GameObject Self => gameObject;
    public MinigunStage CurrentStage => _currentState;
    public bool IsCharging => false;




    //-------FIELD
    [SerializeField]
    private Animator _customAnimator;

    [SerializeField]
    private SkillContainer _skillContainer;

    [SerializeField]
    private TargetSelector _targeter;

    [SerializeField]
    private Bullet _bulletPrefab;

    [SerializeField]
    private Transform[] _bulletSpawnPositions;

    [SerializeField]
    private string _minigunStateName = "MiniganStady";
    [SerializeField]
    private string _minigunSpeedName = "MiniganStady";
    [SerializeField]
    private bool _loockToTarget = false;

    [SerializeField]
    private GameObject _hidebleSkillObject;
    [SerializeField]
    private Transform _bulletLoker;

    private float _minigunSpeed = 2;
    private float _minigunWaitToShoting = 1;
    private float _shotingTime = 2;
    private float _bulletSpeed = 1000;
    private float _bulletDamage;
    private float _shotSpeed = 0.1f;

    private Transform _myTransform;
    private Animator _animator;
    private Coroutine _wait, _shoting;
    private int _lastIndex = 0;
    private bool _skillIsActive = false;
    private MinigunStage _currentState;
    private float _currentShotTime = 0;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onLoad;
    public event UnityAction OnLoad
    {
        add => _onLoad.AddListener(value);
        remove => _onLoad.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onShot;
    public event UnityAction OnShot
    {
        add => _onShot.AddListener(value);
        remove => _onShot.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onStop;
    public event UnityAction OnStop
    {
        add => _onStop.AddListener(value);
        remove => _onStop.RemoveListener(value);
    }



    //-------METODS
    private void Start()
    {
        _minigunSpeed = _skillContainer.minigunInfo.torsionSpeed;
        _minigunWaitToShoting = _skillContainer.minigunInfo.waitToShoting;
        _shotingTime = _skillContainer.minigunInfo.shotingTime;
        _bulletSpeed = _skillContainer.minigunInfo.bulletSpeed;
        _bulletDamage = _skillContainer.minigunInfo.bulletDamage;
        _shotSpeed = _skillContainer.minigunInfo.shotSpeed;
        _myTransform = transform;

        if (_hidebleSkillObject)
            _hidebleSkillObject.SetActive(false);
    }

    public enum MinigunStage
    {
        Rest = 0,
        Load,
        Shoting
    }

    private void Update()
    {
        Vector3 myPositionDir = MyTransform.position;
        if (_targeter.CurrentTarget != null && _targeter.CurrentTarget.GameObject != null && _loockToTarget)
        {
            MyTransform.rotation = Quaternion.LookRotation(_targeter.CurrentTarget.MyTransform.position - myPositionDir);
        }

        if (_currentState == MinigunStage.Shoting)
        {
            if (_currentShotTime > _shotSpeed)
            {
                Shot();
                _currentShotTime = 0;
            }


            _currentShotTime += Time.deltaTime;
        }
    }

    public void Activate()
    {
        _currentShotTime = 0;
        _currentState = MinigunStage.Load;
        Animator.SetInteger(_minigunStateName, (int)_currentState);
        Animator.SetFloat(_minigunSpeedName, _minigunSpeed);
        _skillIsActive = true;

        if (_hidebleSkillObject)
            _hidebleSkillObject.SetActive(true);

        void Shoting()
        {
            _currentState = MinigunStage.Shoting;
            Animator.SetInteger(_minigunStateName, (int)_currentState);

            if (_shoting != null)
                StopCoroutine(_shoting);

            _onShot?.Invoke();
            _shoting = this.WaitSecond(_shotingTime, Stop);
        }

        if (_wait != null)
            StopCoroutine(_wait);

        _onLoad?.Invoke();

        _wait = this.WaitSecond(_minigunWaitToShoting, Shoting);
    }

    public void Stop()
    {
        _currentState = MinigunStage.Rest;
        Animator.SetInteger(_minigunStateName, (int)_currentState);
        _skillIsActive = false;

        _onStop?.Invoke();

        if (_hidebleSkillObject)
            _hidebleSkillObject.SetActive(false);
    }

    public void Shot()
    {
        if (_bulletSpawnPositions.Length == 0)
            return;

        Vector3 bulletSpawnPosition = _bulletSpawnPositions[_lastIndex].position;

        _lastIndex++;

        if (_lastIndex >= _bulletSpawnPositions.Length)
            _lastIndex = 0;

        if (_bulletPrefab != null)
        {
            Bullet bullet = Instantiate(_bulletPrefab, bulletSpawnPosition, Quaternion.identity);
            bullet.gameObject.SetActive(true);
            bullet.SetDamage(_bulletDamage);
            bullet.ShotDirection(_bulletLoker ? _bulletLoker.forward : MyTransform.forward, _bulletSpeed);
        }
    }


}
