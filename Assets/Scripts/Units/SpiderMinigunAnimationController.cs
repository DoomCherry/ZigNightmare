using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMinigunAnimationController : MonoBehaviour
{
    //-------PROPERTY
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());




    //-------FIELD
    [SerializeField]
    private string _animationWalkBoolName = "IsWalk";
    [SerializeField]
    private Animator _customAnimator;
    private Animator _animator;
    private Vector3 _oldRotation;




    //-------EVENTS




    //-------METODS
    public void SetMovement(Rigidbody rigidbody)
    {
        if(rigidbody.velocity.x == 0 && rigidbody.velocity.z == 0 && rigidbody.rotation.eulerAngles.y == _oldRotation.y)
        {
            Animator.SetBool(_animationWalkBoolName, false);
        }
        else
        {
            Animator.SetBool(_animationWalkBoolName, true);
        }

        _oldRotation = rigidbody.rotation.eulerAngles;
    }
}
