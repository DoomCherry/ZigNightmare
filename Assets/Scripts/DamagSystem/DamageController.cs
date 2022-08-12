using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageController : MonoBehaviour
{
    //-------PROPERTY
    public List<DamageDetector> AllDamageDetector
    {
        get
        {
            if (_allDamageDetectors == null)
            {
                _allDamageDetectors = new List<DamageDetector>();

                if (_additionalDamageDetectors != null)
                    _allDamageDetectors.AddRange(_additionalDamageDetectors);

                _allDamageDetectors.AddRange(GetComponentsInChildren<DamageDetector>());

                DamageDetector self = GetComponent<DamageDetector>();
                if (self != null)
                    _allDamageDetectors.Add(self);
            }

            return _allDamageDetectors;
        }
    }
    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;




    //-------FIELD
    [SerializeField]
    private bool _isDestroyAfterDeath = true;
    [SerializeField]
    private float _destroyTime = 2;

    [SerializeField]
    private float _maxHp;

    [SerializeField]
    private DamageDetector[] _additionalDamageDetectors;
    private List<DamageDetector> _allDamageDetectors;
    private float _currentHp;




    //-------EVENTS
    [SerializeField]
    private UnityEvent<float> _onChangeMaxHp;
    public event UnityAction<float> OnChangeMaxHp
    {
        add => _onChangeMaxHp.AddListener(value);
        remove => _onChangeMaxHp.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onDeath;
    public event UnityAction OnDeath
    {
        add => _onDeath.AddListener(value);
        remove => _onDeath.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onTakeDamage;
    public event UnityAction OnTakeDamage
    {
        add => _onTakeDamage.AddListener(value);
        remove => _onTakeDamage.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onTakeHeal;
    public event UnityAction OnTakeHeal
    {
        add => _onTakeHeal.AddListener(value);
        remove => _onTakeHeal.RemoveListener(value);
    }




    //-------METODS
    private void Start()
    {
        for (int i = 0; i < AllDamageDetector.Count; i++)
        {
            AllDamageDetector[i].OnTakeDamage += TakeDamage;
            AllDamageDetector[i].OnTakeHeal += TakeDamage;
        }

        _currentHp = _maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (_currentHp <= 0)
        {
            this.WaitSecond(0.1f, Demolish);
            _onDeath?.Invoke();
        }

        if (damage > 0)
        {
            _currentHp = Mathf.Clamp(_currentHp - damage, -1 ,_maxHp);
            _onTakeDamage?.Invoke();
        }

        if (damage < 0)
        {
            _currentHp = Mathf.Clamp(_currentHp - damage, -1, _maxHp);
            _onTakeHeal?.Invoke();
        }
    }

    public void SetMaxHp(float hp, bool isFitCurrentHp = true)
    {
        _maxHp = hp;

        if (isFitCurrentHp)
            _currentHp = hp;

        _onChangeMaxHp?.Invoke(hp);
    }


    public void Demolish()
    {
        if (_isDestroyAfterDeath)
            this.WaitSecond(_destroyTime, delegate { Destroy(gameObject); });            
    }
}
