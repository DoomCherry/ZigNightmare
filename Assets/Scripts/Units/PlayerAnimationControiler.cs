using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BottomDraggingHandControiler;
using static CenaDraggingHandControiler;

public enum JumpStage
{
    OnFlore = 0,
    Start = 1,
    Fall = 2
}

public class PlayerAnimationControiler : MonoBehaviour
{
    //-------PROPERTY
    private Animator Animator => _customAnimator != null ? _customAnimator : (_animator = _animator ?? GetComponent<Animator>());




    //-------FIELD
    [SerializeField]
    private string _xViewFloatName = "X", _yViewFloatName = "Y";
    [SerializeField]
    private string _jumpStageViewIntName = "JumpStage";
    [SerializeField]
    private string _cenaTriggerName = "CenaPunch";
    [SerializeField]
    private string _bottomPullBoolName = "Pull";
    [SerializeField]
    private string _minigunBoolName = "Minigun";
    [SerializeField]
    private string _cenaPunchSpeedFloatName = "CenaPunchSpeed";
    [SerializeField]
    private string _upercutStateFloatName = "UppercutState";
    [SerializeField]
    private string _dashBoolName = "IsDash";

    [SerializeField]
    private string _prepareBoolName = "PrepareToSkill";

    [SerializeField]
    private Animator _customAnimator;
    [SerializeField]
    private float _compleateFrequancy = 0.5f;
    private Animator _animator;
    private Coroutine _waitSkill;

    private Coroutine _prepareToSkill;
    private Coroutine _uppercutWaitEnd;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onBlastPrepare;
    public event UnityAction OnBlastPrepare
    {
        add => _onBlastPrepare.AddListener(value);
        remove => _onBlastPrepare.RemoveListener(value);
    }




    //-------METODS
    public void SetDash(bool isDashing)
    {
        Animator.SetBool(_dashBoolName, isDashing);
    }
    public void SetMovement(Vector2 walkDirection, Vector2 loockDirection)
    {
        float walkAngle = Vector2.SignedAngle(walkDirection, Vector2.up);
        float lookAngle = Vector2.SignedAngle(loockDirection, Vector2.up);
        float angle = walkAngle - lookAngle;

        Vector2 lookTo = walkDirection.x == 0 && walkDirection.y == 0 ? Vector2.zero : new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle)).normalized;

        Animator.SetFloat(_xViewFloatName, lookTo.x);
        Animator.SetFloat(_yViewFloatName, lookTo.y);
    }

    public void SetJump(Rigidbody rigidbody, bool isJump)
    {
        if (System.Math.Round(rigidbody.velocity.y, 2) == 0)
        {
            Animator.SetInteger(_jumpStageViewIntName, (int)JumpStage.OnFlore);
            return;
        }

        if (rigidbody.velocity.y < 0)
        {
            Animator.SetInteger(_jumpStageViewIntName, (int)JumpStage.Fall);
            return;
        }

        if (isJump)
        {
            Animator.SetInteger(_jumpStageViewIntName, (int)JumpStage.Start);
        }
    }

    public void UseSkill(ISkill skill)
    {
        if (skill is Minigun)
        {
            skill.Activate();
            UseMinigun(skill as Minigun);
        }

        if (skill is CenaDragging)
        {
            skill.Activate();
            UseCenaDrag(skill as CenaDragging);
        }

        if (skill is BottomDragging)
        {
            skill.Activate();
            UseBottomDrag(skill as BottomDragging);
        }
    }

    public void UsePrepareToSkillAnimation(ISkill skill, ICharacterLimiter limiter)
    {
        if (skill is Blast)
        {
            _onBlastPrepare?.Invoke();
            UsePrepareBlast(skill as Blast, limiter);
        }
    }

    private void UseBottomDrag(BottomDragging skill)
    {
        IEnumerator WaitSkill()
        {
            while (skill.CurrentHand != null)
            {
                if (skill.CurrentHand.CurrentStage == BottomDraggingState.Grab)
                {
                    Animator.SetBool(_bottomPullBoolName, true);
                }
                else
                {
                    Animator.SetBool(_bottomPullBoolName, false);
                }

                yield return new WaitForSeconds(_compleateFrequancy);
            }

            Animator.SetBool(_bottomPullBoolName, false);
        }

        if (_waitSkill != null)
        {
            StopCoroutine(_waitSkill);
            _waitSkill = null;
        }
        _waitSkill = StartCoroutine(WaitSkill());
    }

    private void UseCenaDrag(CenaDragging skill)
    {
        IEnumerator WaitSkill()
        {
            while (skill.CurrentHand != null)
            {
                if (skill.CurrentHand.CurrentState == CenaDraggingState.Punch)
                {
                    Animator.SetTrigger(_cenaTriggerName);
                    break;
                }

                yield return new WaitForSeconds(_compleateFrequancy);
            }
        }

        if (_waitSkill != null)
        {
            StopCoroutine(_waitSkill);
            _waitSkill = null;
        }

        Animator.SetFloat(_cenaPunchSpeedFloatName, skill.SkillContainer.cenaPunchInfo.punchSpeed);
        _waitSkill = StartCoroutine(WaitSkill());
    }

    private void UseMinigun(Minigun skill)
    {
        IEnumerator WaitSkill()
        {
            while (skill.CurrentStage != Minigun.MinigunStage.Rest)
            {
                if (skill.CurrentStage == Minigun.MinigunStage.Shoting)
                {
                    Animator.SetBool(_minigunBoolName, true);
                }
                else
                {
                    Animator.SetBool(_minigunBoolName, false);
                }
                yield return new WaitForSeconds(_compleateFrequancy);
            }

            Animator.SetBool(_minigunBoolName, false);
        }

        if (_waitSkill != null)
        {
            StopCoroutine(_waitSkill);
            _waitSkill = null;
        }

        _waitSkill = StartCoroutine(WaitSkill());
    }

    private void UsePrepareBlast(Blast blast, ICharacterLimiter limiter)
    {
        if (_prepareToSkill != null)
            return;

        Animator.SetBool(_prepareBoolName, true);

        void AfterPreparedState()
        {
            Animator.SetBool(_prepareBoolName, false);
            _prepareToSkill = null;
            blast.Activate();
            limiter.UnfreezeFalling();
            limiter.UnfreezeRotation();
            limiter.UnfreezeSkill();
            limiter.UnfreezeWalking();
            limiter.JumpUnfreeze();
        }

        limiter.FreezeFalling();
        limiter.FreezeRotation();
        limiter.FreezeSkill();
        limiter.FreezeWalking();
        limiter.JumpFreeze();
        _prepareToSkill = this.WaitSecond(blast.SkillContainer.blastInfo.blastPrepareTime, AfterPreparedState);
    }

    public void UseChargeSkill(ISkill skill, ICharacterLimiter limiter)
    {
        if (skill is Uppercut)
        {
            UseUppercut(skill as Uppercut, limiter);
        }
    }

    private void UseUppercut(Uppercut uppercut, ICharacterLimiter limiter)
    {
        if (uppercut.UppercutState == UppercutState.None)
        {
            SetLimit(limiter);
            uppercut.Activate();
            Animator.SetFloat(_upercutStateFloatName, (int)uppercut.UppercutState);
            return;
        }

        if (uppercut.UppercutState == UppercutState.Powering)
        {
            uppercut.Activate();
            UnsetLimit(limiter);

            Vector3 oldVelocity = limiter.MyRigidbody.velocity;

            if (uppercut.UppercutState == UppercutState.Punching)
            {
                limiter.MyRigidbody.velocity = new Vector3(oldVelocity.x, uppercut.SkillContainer.uppercutInfo.uppercutUpVelocity,oldVelocity.z);
                Animator.SetFloat(_upercutStateFloatName, (int)uppercut.UppercutState);
            }

            if (uppercut.UppercutState == UppercutState.PowerPunching)
            {
                limiter.MyRigidbody.velocity = new Vector3(oldVelocity.x, uppercut.SkillContainer.uppercutInfo.powerUppercutUpVelocity, oldVelocity.z);
                Animator.SetFloat(_upercutStateFloatName, (int)uppercut.UppercutState);
            }

            if(_uppercutWaitEnd != null)
            {
                StopCoroutine(_uppercutWaitEnd);
                _uppercutWaitEnd = null;
            }

            _uppercutWaitEnd = StartCoroutine(WaitHandStage((int)UppercutState.None));

            return;
        }

        IEnumerator WaitHandStage(float stageNum)
        {
            while (!uppercut.AnimationIsEnd())
            {
                yield return new WaitForSeconds(_compleateFrequancy);
            }

            uppercut.Activate();
        }

        void SetLimit(ICharacterLimiter limiter)
        {
            limiter.JumpFreeze();
            limiter.FreezeWalking();
            limiter.FreezeRotation();
        }

        void UnsetLimit(ICharacterLimiter limiter)
        {
            limiter.UnfreezeRotation();
            limiter.JumpUnfreeze();
            limiter.UnfreezeWalking();
        }
    }
}
