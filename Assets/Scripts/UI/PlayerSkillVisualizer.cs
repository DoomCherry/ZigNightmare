using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerSkillVisualizer : MonoBehaviour
{
    //-------FIELD
    [SerializeField]
    private PlayerContorller _playerController;
    [SerializeField]
    private Image _skillIcon;
    [SerializeField]
    private Text _coldown;
    private float _lastCd;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onSkillCdEnd;
    public event UnityAction OnSkillCdEnd
    {
        add => _onSkillCdEnd.AddListener(value);
        remove => _onSkillCdEnd.RemoveListener(value);
    }




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

        if (currentCd <= 0 && _lastCd > 0)
        {
            _onSkillCdEnd?.Invoke();
        }

        _lastCd = currentCd;
        _coldown.text = isHaveCd ? (currentCd > _playerController.Skill.SkillContainer.coldown ? "" : Math.Round(_playerController.Skill.SkillContainer.coldown - currentCd,1).ToString()) : "";
    }

}
