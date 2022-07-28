
using UnityEngine;

public interface ITarget
{
    GameObject GameObject { get; }
    Transform MyTransform { get; }
    ISkill Skill { get; }

    void Select(Color color);
    void Diselect();
}
