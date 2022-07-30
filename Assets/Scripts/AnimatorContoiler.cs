using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorContoiler : MonoBehaviour
{
    //-------FIELD
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private string _triggerName;




    //-------EVENTS




    //-------METODS
    public void PlayTrigger()
    {
        _animator.SetTrigger(_triggerName);
    }
}
