using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UppercutState
{
    None = 0,
    Powering = 1,
    Punching = 2,
    PowerPunching = 3
}

public enum Handstages
{
    UppercutPunch = 3,
    UppercutPowerPunch = 4
}

[RequireComponent(typeof(Animator), typeof(SelfDetectDamageDealer))]
public class Uppercut : MonoBehaviour, ISkill
{
    //-------PROPERTY
    public SelfDetectDamageDealer MyDamagDealer
    {
        get
        {
            _damageDealer = _damageDealer == null ? GetComponent<SelfDetectDamageDealer>() : _damageDealer;
            return _damageDealer;
        }
    }
    public Transform MyTransform => _myTransform = _myTransform ?? transform;
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());

    public bool IsSkillActive => _skillIsActive;

    public SkillContainer SkillContainer => _skillContainer;

    public GameObject Self => gameObject;

    public bool IsCharging => true;
    public UppercutState UppercutState => _uppercutState;




    //-------FIELD
    [SerializeField]
    private Animator _customAnimator;
    [SerializeField]
    private string _handstageName = "HandStage";
    [SerializeField]
    private SkillContainer _skillContainer;
    private Animator _animator;
    private bool _skillIsActive = false;
    private Transform _myTransform;
    private SelfDetectDamageDealer _damageDealer;
    private UppercutState _uppercutState = UppercutState.None;
    private bool _punchIsPower = false;
    private Coroutine _charging;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onCharging;
    public event UnityAction OnCharging
    {
        add => _onCharging.AddListener(value);
        remove => _onCharging.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onChargeFinish;
    public event UnityAction OnChargeFinish
    {
        add => _onChargeFinish.AddListener(value);
        remove => _onChargeFinish.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onUppercutting;
    public event UnityAction OnUppercutting
    {
        add => _onUppercutting.AddListener(value);
        remove => _onUppercutting.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onPowerUppercutting;
    public event UnityAction OnPowerUppercutting
    {
        add => _onPowerUppercutting.AddListener(value);
        remove => _onPowerUppercutting.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onStop;
    public event UnityAction OnStop
    {
        add => _onStop.AddListener(value);
        remove => _onStop.RemoveListener(value);
    }




    //-------METODS
    public void Activate()
    {
        UppercutRefreshState();

        if (_uppercutState == UppercutState.Punching)
        {
            Animator.SetInteger(_handstageName, (int)Handstages.UppercutPunch);
            _damageDealer.SetDamage(SkillContainer.uppercutInfo.uppercutDamage, SkillContainer.uppercutInfo.uppercutEnemyLimits, SkillContainer.uppercutInfo.uppercutLimitTime, SkillContainer.uppercutInfo.uppercutPushingForce, SkillContainer.uppercutInfo.uppercutUpVelocity);
        }

        if (_uppercutState == UppercutState.PowerPunching)
        {
            Animator.SetInteger(_handstageName, (int)Handstages.UppercutPowerPunch);
            _damageDealer.SetDamage(SkillContainer.uppercutInfo.powerUppercutDamage, SkillContainer.uppercutInfo.powerUppercutEnemyLimits, SkillContainer.uppercutInfo.powerUppercutLimitTime, SkillContainer.uppercutInfo.powerUppercutPushingForce, SkillContainer.uppercutInfo.powerUppercutUpVelocity);
        }
    }

    public bool AnimationIsEnd()
    {
        int currentState = Animator.GetInteger(_handstageName);
        return currentState != (int)Handstages.UppercutPunch && currentState != (int)Handstages.UppercutPowerPunch;
    }

    public void Stop()
    {
        _uppercutState = UppercutState.None;
        _punchIsPower = false;
        if (_charging != null)
            StopCoroutine(_charging);
        _charging = null;
        _skillIsActive = false;
        _onStop?.Invoke();
    }

    private void UppercutRefreshState()
    {
        switch (_uppercutState)
        {
            case UppercutState.None:
                _uppercutState = UppercutState.Powering;
                if (_charging != null)
                {
                    StopCoroutine(_charging);
                    _charging = null;
                }

                _skillIsActive = true;
                _charging = this.WaitSecond(_skillContainer.uppercutInfo.chargeTime, 
                    delegate 
                    { 
                        _punchIsPower = true; 
                        _onChargeFinish?.Invoke(); 
                    });
                _onCharging?.Invoke();
                break;
            case UppercutState.Powering:

                if (_punchIsPower)
                {
                    _uppercutState = UppercutState.PowerPunching;
                    MyDamagDealer.SetDamage(_skillContainer.uppercutInfo.powerUppercutDamage);
                    _onPowerUppercutting?.Invoke();
                }
                else
                {
                    _uppercutState = UppercutState.Punching;
                    MyDamagDealer.SetDamage(_skillContainer.uppercutInfo.uppercutDamage);
                    _onUppercutting?.Invoke();
                }

                break;
            case UppercutState.Punching:
            case UppercutState.PowerPunching:
                Stop();
                break;
            default:
                break;
        }
    }
       
}
