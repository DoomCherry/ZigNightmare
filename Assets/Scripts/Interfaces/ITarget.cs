
using UnityEngine;

public interface ITarget
{
    Transform MyTransform { get; }
    ISkill Skill { get; }

    void Select(Color color);
    void Diselect();
}
