using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    //-------FIELD
    [SerializeField]
    private DamageController _damageController;
    [SerializeField]
    private Slider _healthBar;




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
        _healthBar.value = _damageController.CurrentHp;
    }

    private void OnDestroy()
    {
        _damageController.OnTakeDamage -= RefreshHelathBar;
        _damageController.OnChangeMaxHp -= SetNewMaxHp;
    }
}
