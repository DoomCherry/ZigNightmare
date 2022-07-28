using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpiderMinigunAnimationController))]
public class MinigunRobot : Enemy
{
    //-------PROPERTY
    public SpiderMinigunAnimationController SpiderAnimationController => _spiderAnimationController = _spiderAnimationController ??= GetComponent<SpiderMinigunAnimationController>();




    //-------FIELD
    [SerializeField]
    private MeshRenderer[] _materialsBase;

    [SerializeField]
    private Transform _xzRotationBone, _yRotationBone;
    private SpiderMinigunAnimationController _spiderAnimationController;




    //-------METODS
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        SpiderAnimationController.SetMovement(MyRigidbody);
    }

    protected override void UpdateRotation()
    {
        if (Selector.CurrentTarget != null)
        {
            base.UpdateRotation();

            void RoateXZBone()
            {
                Vector3 xzPosition = new Vector3(Selector.CurrentTarget.MyTransform.position.x, 0, Selector.CurrentTarget.MyTransform.position.z);
                Vector3 xzSelf = new Vector3(MyTransform.position.x, 0, MyTransform.position.z);

                Quaternion nextView = Quaternion.LookRotation((xzPosition - xzSelf).normalized * 100);
                _xzRotationBone.rotation = Quaternion.Lerp(_xzRotationBone.rotation, nextView, Viewspeed);
            }
            RoateXZBone();

            void RotateYBone()
            {
                Vector3 yPosition = new Vector3(Selector.CurrentTarget.MyTransform.position.x, Selector.CurrentTarget.MyTransform.position.y, Selector.CurrentTarget.MyTransform.position.z);
                Vector3 ySelf = new Vector3(MyTransform.position.x, MyTransform.position.y, MyTransform.position.z);

                Quaternion nextView = Quaternion.LookRotation((yPosition - ySelf).normalized * 100);
                _yRotationBone.rotation = Quaternion.Lerp(_yRotationBone.rotation, nextView, Viewspeed);
            }
            RotateYBone();
        }
    }

    public override void Select(Color color)
    {
        base.Select(color);
        if (_materialsBase == null)
            return;

        for (int i = 0; i < _materialsBase.Length; i++)
        {
            _materialsBase[i].material.SetColor("_EmissionColor", color);
        }
    }

    public override void Diselect()
    {
        base.Diselect();
        if (_materialsBase == null)
            return;

        for (int i = 0; i < _materialsBase.Length; i++)
        {
            _materialsBase[i].material.SetColor("_EmissionColor", Color.black);
        }
    }
}
