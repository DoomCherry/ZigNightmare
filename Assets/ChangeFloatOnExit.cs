using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloatOnExit : StateMachineBehaviour
{
    //-------FIELD
    [SerializeField]
    private string _floatSkillName;
    [SerializeField]
    private float _value;




    //-------METODS
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(_floatSkillName, _value);
    }
}
