using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBoolOnExit : StateMachineBehaviour
{
    //-------FIELD
    [SerializeField]
    private string _boolSkillName;
    [SerializeField]
    private bool _value;




    //-------METODS
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(_boolSkillName, _value);
    }
}
