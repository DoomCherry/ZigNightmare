using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpikeAnimationControiler))]
public class Spike : Enemy
{
    //-------PROPERTY
    public SpikeAnimationControiler SpikeAnimationControiler => _spikeAnimationController = _spikeAnimationController ??= GetComponent<SpikeAnimationControiler>();




    //-------FIELD
    [SerializeField]
    private SkinnedMeshRenderer _materialBase;
    private SpikeAnimationControiler _spikeAnimationController;




    //-------METODS
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        SpikeAnimationControiler.SetMovement(new Vector2(_currentVelocity.x, _currentVelocity.z), 1);
    }

    protected override void ActivateSkill()
    {
        base.ActivateSkill();
        SpikeAnimationControiler.UseSkill(Skill, Skill.Stop);
    }

    public override void Select(Color color)
    {
        base.Select(color);
        _materialBase.material.SetColor("_EmissionColor", color);
    }

    public override void Diselect()
    {
        base.Diselect();
        _materialBase.material.SetColor("_EmissionColor", Color.black);
    }
}
