using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxContainer : Enemy
{
    //-------FIELD
    [SerializeField]
    private MeshRenderer _materialBase;




    //-------METODS
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
