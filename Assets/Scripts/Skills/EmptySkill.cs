using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySkill : MonoBehaviour, ISkill
{
    //-------PROPERTY
    public bool IsCharging => false;

    public bool IsSkillActive => false;

    public SkillContainer SkillContainer => _skillContainer;

    public GameObject Self => gameObject;




    //-------FIELD
    [SerializeField]
    private SkillContainer _skillContainer;




    //-------METODS
    public void Activate()
    {
    }

    public void Stop()
    {
    }
}
