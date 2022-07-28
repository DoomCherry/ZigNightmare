using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStaminaBar : MonoBehaviour
{
    //-------FIELD
    [SerializeField]
    private PlayerContorller _playerController;
    [SerializeField]
    private Slider _staminaBar;




    //-------METODS
    private void Start()
    {
        if (_playerController == null)
        {
            Debug.LogWarning($"{name}: {_playerController} is missing!");
            return;
        }

        _staminaBar.minValue = 0;
        _staminaBar.maxValue = _playerController.MaxStamina;
        _staminaBar.value = _playerController.CurrentStamina;

        _playerController.OnStaminaChange += RefreshStaminaBar;
    }

    private void RefreshStaminaBar()
    {
        _staminaBar.value = _playerController.CurrentStamina;
    }

    private void OnDestroy()
    {
        _playerController.OnStaminaChange -= RefreshStaminaBar;
    }
}
