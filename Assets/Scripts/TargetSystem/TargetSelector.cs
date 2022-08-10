using Doomchery;
using System.Collections;
using UnityEngine;

public struct TargetInfo
{
    public ITarget target;
    public float conditionalDistance;
}

public class TargetSelector : MonoBehaviour
{
    //-------PROPERTY
    public ITarget CurrentTarget => _customTarget != null ? CustomTarget : _currentTarget.target;
    private ITarget CustomTarget
    {
        get
        {
            if (_customTarget != null)
            {
                _customITarget = _customITarget == null ? _customTarget.GetComponent<ITarget>() : _customITarget;
                return _customITarget;
            }

            return null;
        }
    }
    private Transform MyTransform => _myTransform = _myTransform ?? transform;




    //-------FIELD

    [SerializeField]
    private Color _selectColor;
    [SerializeField]
    private GameObject _customTarget;
    private ITarget _customITarget;

    [SerializeField]
    private float _maxTargetDistance = 50;
    [SerializeField]
    private float _radialMult = 3;
    [SerializeField]
    private float _mouseRadiusSelect = 1;
    [SerializeField]
    private float _selectAngle = 25;

    [SerializeField]
    private float _updateTime = 0.25f;

    private Camera _main;
    private Transform _myTransform;
    private TargetInfo _currentTarget;




    //-------EVENTS




    //-------METODS
    private void OnValidate()
    {
        if (_customTarget != null && _customTarget.GetComponent<ITarget>() == null)
            _customTarget = null;
    }

    private void Start()
    {
        if (SelectorHandler.Instance == null)
            throw new System.NullReferenceException($"{name}: {SelectorHandler.Instance} is missing.");

        _main = Camera.main;
        StartCoroutine(StartUpdate());
    }

    private void TargetSelectorUpdate()
    {
        Ray ray = _main.ScreenPointToRay(Input.mousePosition);

        LinearFunction3 mouseLine = new LinearFunction3(ray.origin * -1, ray.direction);

        Vector3 _mousePointInWorld = mouseLine.GetCoordsFromProjectionY(MyTransform.position.y);
        Vector3 seltPosition = MyTransform.position;
        seltPosition.y = MyTransform.position.y;

        LinearFunction3 playerToMouse = new LinearFunction3(seltPosition, seltPosition - _mousePointInWorld);

        void SelectNewTarget(ITarget target, float distance)
        {
            target.Select(_selectColor);


            if (_currentTarget.target != null)
                _currentTarget.target.Diselect();

            _currentTarget.target = target;
            _currentTarget.conditionalDistance = distance;
        }

        if (_currentTarget.target != null && _currentTarget.target.Equals(null) == false)
            _currentTarget.target.Diselect();

        _currentTarget.target = null;
        _currentTarget.conditionalDistance = float.MaxValue;

        for (int i = 0; i < SelectorHandler.Instance.AllTarget.Count; i++)
        {
            ITarget item = SelectorHandler.Instance.AllTarget[i];

            if (item.MyTransform == MyTransform)
                continue;

            item.Diselect();

            Vector3 targetPosition = item.MyTransform.position;
            targetPosition.y = MyTransform.position.y;

            if (Vector3.Distance(MyTransform.position, targetPosition) > _maxTargetDistance)
                continue;

            float angle = Vector3.Angle(playerToMouse.direction.normalized, (seltPosition - targetPosition).normalized);

            if (angle > _selectAngle)
                continue;

            if (Vector3.Distance(targetPosition, _mousePointInWorld) <= _mouseRadiusSelect)
            {
                SelectNewTarget(item, float.MaxValue);
                break;
            }

            Vector3 linePoint = playerToMouse.FindNearestPointOnLine(targetPosition);
            float pointToTarget = Vector3.Distance(linePoint, targetPosition);
            float pointToSelf = Vector3.Distance(linePoint, seltPosition);

            float totalDistance = pointToSelf + (pointToTarget * _radialMult);

            if (_currentTarget.conditionalDistance > totalDistance)
            {
                SelectNewTarget(item, totalDistance);
            }
        }
    }

    private IEnumerator StartUpdate()
    {
        while (_customTarget == null)
        {
            TargetSelectorUpdate();
            yield return new WaitForSeconds(_updateTime);
        }
    }
}
