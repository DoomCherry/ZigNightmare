using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillVisualizer : MonoBehaviour
{
    //-------FIELD
    [SerializeField]
    private PlayerContorller _playerController;
    [SerializeField]
    private Image _skillIcon;
    [SerializeField]
    private Text _coldown;




    //-------METODS
    private void Start()
    {
        if (_playerController == null)
        {
            Debug.LogWarning($"{name}: {_playerController} is missing!");
            return;
        }

        RefreshSkill();
    }

    private void Update()
    {
        RefreshSkill();
    }

    private void RefreshSkill()
    {
        _skillIcon.sprite = _playerController.Skill.SkillContainer._icon;

        bool isHaveCd = _playerController.ColdownList.TryGetValue(_playerController.Skill.SkillContainer, out float lastTime);
        float currentCd = Time.time - lastTime;
        _coldown.text = isHaveCd ? (currentCd > _playerController.Skill.SkillContainer.coldown ? "" : Math.Round(_playerController.Skill.SkillContainer.coldown - currentCd,1).ToString()) : "";
    }

}
