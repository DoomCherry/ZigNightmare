using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeIntegerOnExit : StateMachineBehaviour
{
    //-------FIELD
    [SerializeField]
    private string _integerSkillName;
    [SerializeField]
    private int _value;




    //-------METODS
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(_integerSkillName, _value);
    }
}
