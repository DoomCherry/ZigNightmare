using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BottomDraggingHandControiler;
using static CenaDraggingHandControiler;

public enum SpikeAtackType
{
    Idle = 0,
    Pull = 1,
    CenaPunch = 2,
    Uppercut = 3
}

public class SpikeAnimationControiler : MonoBehaviour
{
    //-------PROPERTY
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());




    //-------FIELD
    [SerializeField]
    private string _animationWalkSpeedName = "WalkSpeed";
    [SerializeField]
    private string _integerAttackStageName = "AttackState";
    [SerializeField]
    private string _walkBoolStageName = "IsWalk";
    [SerializeField]
    private string _cenaPunchSpeedFloatName = "CenaPunchSpeed";
    [SerializeField]
    private Animator _customAnimator;
    [SerializeField]
    private float _compleateFrequancy = 0.5f;
    private Animator _animator;
    private Coroutine _waitSkill;




    //-------EVENTS




    //-------METODS
    /// <summary>
    /// Set movement animation in ainmator
    /// </summary>
    /// <param name="walkDirection"> unit walk direction</param>
    /// <param name="walkAnimationSpeed"> walk type <idle / walk / run>. set value from 0 to 1 </param>
    public void SetMovement(Vector2 walkDirection, float walkAnimationSpeed = 1)
    {
        walkAnimationSpeed = Mathf.Clamp01(walkAnimationSpeed);

        if (walkDirection.x == 0 && walkDirection.y == 0)
        {
            Animator.SetFloat(_animationWalkSpeedName, 0);
            Animator.SetBool(_walkBoolStageName, false);
        }
        else
        {
            Animator.SetFloat(_animationWalkSpeedName, walkAnimationSpeed);
            Animator.SetBool(_walkBoolStageName, true);
        }
    }

    public void UseSkill(ISkill skill, Action onAnimationEnd)
    {
        if (skill is CenaDragging)
        {
            UseCenaDrag(skill as CenaDragging, onAnimationEnd);
        }

        if (skill is BottomDragging)
        {
            UseBottomDrag(skill as BottomDragging, onAnimationEnd);
        }
    }

    private void UseBottomDrag(BottomDragging skill, Action onAnimationEnd)
    {
        IEnumerator WaitSkill()
        {
            while (skill.CurrentHand != null)
            {
                if (skill.CurrentHand.CurrentStage == BottomDraggingState.Grab)
                {
                    Animator.SetInteger(_integerAttackStageName, (int)SpikeAtackType.Pull);
                }
                else
                {
                    Animator.SetInteger(_integerAttackStageName, (int)SpikeAtackType.Idle);
                }

                yield return new WaitForSeconds(_compleateFrequancy);
            }

            Animator.SetInteger(_integerAttackStageName, (int)SpikeAtackType.Idle);
            onAnimationEnd?.Invoke();
        }

        if (_waitSkill != null)
        {
            StopCoroutine(_waitSkill);
            _waitSkill = null;
        }
        _waitSkill = StartCoroutine(WaitSkill());
    }

    private void UseCenaDrag(CenaDragging skill, Action onAnimationEnd)
    {
        IEnumerator WaitSkill()
        {
            Animator.SetInteger(_integerAttackStageName, (int)SpikeAtackType.Idle);

            while (skill.CurrentHand != null)
            {
                if (skill.CurrentHand.CurrentState == CenaDraggingState.Punch)
                {
                    Animator.SetInteger(_integerAttackStageName, (int)SpikeAtackType.CenaPunch);
                    break;
                }

                yield return new WaitForSeconds(_compleateFrequancy);
            }

            while (Animator.GetInteger(_integerAttackStageName) == (int)SpikeAtackType.CenaPunch)
            {
                yield return new WaitForSeconds(_compleateFrequancy);
            }

            onAnimationEnd?.Invoke();
        }

        if (_waitSkill != null)
        {
            StopCoroutine(_waitSkill);
            _waitSkill = null;
        }

        Animator.SetFloat(_cenaPunchSpeedFloatName, skill.SkillContainer.cenaPunchInfo.punchSpeed);
        _waitSkill = StartCoroutine(WaitSkill());
    }
}
