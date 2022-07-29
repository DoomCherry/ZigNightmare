using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    //-------PROPERTY
    public IReadOnlyList<PathPoint> PathPoints => _points;
    private float GradientSpeed => (float)1 / _points.Count;
    private Vector4 GradientVector
    {
        get
        {
            Vector4 safeEquals = new Vector4(_stargLineColor.r == _endLineColor.r ? _stargLineColor.r : 0,
                                            _stargLineColor.g == _endLineColor.g ? _stargLineColor.g : 0,
                                            _stargLineColor.b == _endLineColor.b ? _stargLineColor.b : 0,
                                            _stargLineColor.a == _endLineColor.a ? _stargLineColor.a : 0);
            Color _vector = _endLineColor - _stargLineColor;
            return new Vector4(safeEquals.x == 0 ? _vector.r : safeEquals.x,
                                safeEquals.y == 0 ? _vector.g : safeEquals.y,
                                safeEquals.z == 0 ? _vector.b : safeEquals.z,
                                safeEquals.w == 0 ? _vector.a : safeEquals.w);
        }
    }




    //-------FIELDS
    [SerializeField]
    public Color _stargLineColor, _endLineColor;
    [SerializeField, HideInInspector]
    private List<PathPoint> _points = new List<PathPoint>();
    private PathPoint _singleGizmosDrawPoint = null;




    //-------METODS
    public void AddPoint()
    {
        PathPoint last = _points.LastOrDefault();
        _points.Add(new GameObject().AddComponent<PathPoint>());
        PathPoint current = _points.Last();
        current.name = $"point {_points.Count - 1}";

        Transform currentTransform = current.transform;
        currentTransform.parent = transform;

        if (last != null)
        {
            currentTransform.localPosition += last.transform.position + last.transform.forward;
        }
        else
        {
            currentTransform.localPosition = Vector3.zero;
        }
    }

    public void ClearPoint()
    {
        for (int i = 0; i < _points.Count; i++)
        {
            DestroyImmediate(_points[i].gameObject);
        }
        _points.Clear();
    }
    
    private void OnDrawGizmos()
    {
        _points = _points.Where(n => n != null).ToList();

        for (int i = 0; i < _points.Count - 1; i++)
        {
            Vector4 gradientVectorSpeed = GradientVector * GradientSpeed;
            Gizmos.color = _stargLineColor + (i * new Color(gradientVectorSpeed.x, gradientVectorSpeed.y, gradientVectorSpeed.z, gradientVectorSpeed.w));
            Gizmos.DrawLine(_points[i].MyTransform.position, _points[i + 1].MyTransform.position);
        }
    }
}
