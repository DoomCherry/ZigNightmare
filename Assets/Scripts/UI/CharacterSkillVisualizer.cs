using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterSkillVisualizer : MonoBehaviour
{
    //-------PROPERTY
    private ITarget ITarget => _iTarget = _iTarget ??= _iTargetSkillContainer.GetComponent<ITarget>();




    //-------FIELD
    [SerializeField]
    private GameObject _iTargetSkillContainer;
    [SerializeField]
    private Image _skillIcon;
    [HideInInspector, SerializeField]
    private ITarget _iTarget;




    //-------EVENTS




    //-------METODS
    private void OnValidate()
    {
        if (_iTargetSkillContainer.GetComponent<ITarget>() == null)
            _iTargetSkillContainer = null;
    }

    private void Start()
    {
        if (_iTargetSkillContainer == null)
        {
            Debug.LogWarning($"{name}: {_iTargetSkillContainer} is missing!");
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
        _skillIcon.sprite = ITarget.Skill.SkillContainer._icon;
    }
}
