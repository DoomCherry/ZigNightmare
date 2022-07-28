using UnityEngine;

public interface ISkill
{
    //-------PROPERTY
    bool IsCharging { get; }
    bool IsSkillActive { get; }
    SkillContainer SkillContainer { get; }
    GameObject Self { get; }




    //-------METODS
    void Activate();

    void Stop();
}
