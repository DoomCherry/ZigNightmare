using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TargetSelector))]
public class SkillStealler : MonoBehaviour
{
    //-------PROPERTY
    public List<ISkill> MySkills
    {
        get
        {
            if(_mySkills.Count == 0)
            {
                for (int i = 0; i < _allSkills.Count; i++)
                {
                    _mySkills.Add(_allSkills[i].GetComponent<ISkill>());
                }
            }
            return _mySkills;
        }
    }
    public TargetSelector Target
    {
        get
        {
            if(_target == null)
            {
                _target = GetComponent<TargetSelector>();
            }
            return _target;
        }
    }




    //-------FIELD
    [SerializeField]
    private List<GameObject> _allSkills;
    private List<ISkill> _mySkills = new List<ISkill>();
    private TargetSelector _target;




    //-------METODS
    private void OnValidate()
    {
        for (int i = 0; i < _allSkills.Count; i++)
        {
            if(_allSkills[i].GetComponent<ISkill>() == null)
            {
                _allSkills.RemoveAt(i);
                i--;
            }
        }
    }

    public ISkill GetTargetSkill()
    {
        if (Target.CurrentTarget == null)
            return null;

        ISkill currentSkill = MySkills.Where(n => n.SkillContainer == Target.CurrentTarget.Skill.SkillContainer).FirstOrDefault();

        if (currentSkill == null)
            Debug.LogWarning($"{name} not contain skill {Target.CurrentTarget.Skill.SkillContainer._name}");

        return currentSkill;
    }
}
