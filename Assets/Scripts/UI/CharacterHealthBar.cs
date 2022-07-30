using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    //-------FIELD
    [SerializeField]
    private DamageController _damageController;
    [SerializeField]
    private Slider _healthBar;
    [SerializeField]
    private float _hpBoard = 30;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onLowHp;
    public event UnityAction OnLowHp
    {
        add => _onLowHp.AddListener(value);
        remove => _onLowHp.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent _onNormalHp;
    public event UnityAction OnNormalHp
    {
        add => _onNormalHp.AddListener(value);
        remove => _onNormalHp.RemoveListener(value);
    }




    //-------METODS
    private void Start()
    {
        if(_damageController == null)
        {
            Debug.LogWarning($"{name}: {_damageController} is missing!");
            return;
        }

        SetNewMaxHp(_damageController.MaxHp);
        _healthBar.value = _damageController.MaxHp;

        _damageController.OnTakeDamage += RefreshHelathBar;
        _damageController.OnChangeMaxHp += SetNewMaxHp;
    }

    private void SetNewMaxHp(float newMaxHp)
    {
        _healthBar.minValue = 0;
        _healthBar.maxValue = newMaxHp;
        RefreshHelathBar();
    }

    private void RefreshHelathBar()
    {
        if (_healthBar.value > _hpBoard && _damageController.CurrentHp <= _hpBoard)
            _onLowHp?.Invoke();

        if (_healthBar.value < _hpBoard && _damageController.CurrentHp >= _hpBoard)
            _onNormalHp?.Invoke();

        _healthBar.value = _damageController.CurrentHp;
    }

    private void OnDestroy()
    {
        _damageController.OnTakeDamage -= RefreshHelathBar;
        _damageController.OnChangeMaxHp -= SetNewMaxHp;
    }
}
