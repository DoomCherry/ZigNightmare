using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator), typeof(SelfDetectDamageDealer))]
public class Blast : MonoBehaviour, ISkill
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
    private Animator Animator => _animator = _animator ?? GetComponent<Animator>();

    public bool IsSkillActive => _skillIsActive;

    public SkillContainer SkillContainer => _skillContainer;

    public GameObject Self => gameObject;
    public bool IsCharging => false;




    //-------FIELD
    [SerializeField]
    private SkillContainer _skillContainer;
    [SerializeField]
    private string _exploidTriggerName = "IsExploid";
    private Animator _animator;
    private bool _skillIsActive = false;
    private Blast _instance;
    private Transform _myTransform;
    private SelfDetectDamageDealer _damageDealer;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onBlastStart;
    public event UnityAction OnBlastStart
    {
        add => _onBlastStart.AddListener(value);
        remove => _onBlastStart.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onBlastEnd;
    public event UnityAction OnBlastEnd
    {
        add => _onBlastEnd.AddListener(value);
        remove => _onBlastEnd.RemoveListener(value);
    }




    //-------METODS
    public void Activate()
    {
        _instance = Instantiate(this, transform.position, Quaternion.identity);

        _instance.Exploid(transform);
    }

    public void SetDamageByTargets()
    {
        MyDamagDealer.SetDamage(_skillContainer.blastInfo.blastDamage, _skillContainer.blastInfo.blastEnemyLimits, _skillContainer.blastInfo.blastLimitTime, _skillContainer.blastInfo.blastPushingForce);
    }

    public void Stop()
    {
        if (_instance != null)
        {
            _skillIsActive = false;
            _onBlastEnd.Invoke();

            Destroy(_instance.gameObject);
        }
    }

    public void Exploid(Transform target)
    {
        Animator.SetTrigger(_exploidTriggerName);
        _onBlastStart?.Invoke();
        _skillIsActive = true;

        IEnumerator DestroySkill()
        {
            float time = 0;
            while (time <= 2)
            {
                if (target == null)
                    break;

                MyTransform.position = target.position;

                time += Time.deltaTime;
                yield return null;
            }

            Stop();
        }

        StartCoroutine(DestroySkill());
    }
}
