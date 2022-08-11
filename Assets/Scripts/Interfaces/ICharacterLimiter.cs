using System;
using UnityEngine;

[System.Flags]
public enum Limits
{
    Walking = 1,
    Falling = 2,
    Jump = 4,
    Rotation = 8,
    Skills = 16
}

public interface ICharacterLimiter
{
    //-------PROPERTY
    Rigidbody MyRigidbody { get; }
    bool WalkingIsFreeze { get; set; }
    bool FallingIsFreeze { get; set; }
    bool JumpIsFreeze { get; set; }
    bool RotationIsFreeze { get; set; }
    bool SkillsIsFreeze { get; set; }
    bool IsPhysicTargetsDesable { get; set; }
    bool IsFullSystemFreeze { get; set; }




    //-------METODS
    void FreezeRotation();
    void UnfreezeRotation();
    void PrepareToSkill(ISkill skill, Action afterPrepare);
    void TakeDamage(float damage);
    void FreezeWalking();
    void UnfreezeWalking();
    void FreezeFalling();
    void UnfreezeFalling();
    void FreezeSkill();
    void UnfreezeSkill();
    void JumpFreeze();
    void JumpUnfreeze();

    void FullSystemFreeze();
    void FullSystemUnfreeze();

    void DisablePhysicsTarget();
    void EnablePhysicsTarget();
}
