using System.Collections.Generic;
using UnityEngine;

namespace Doomchery
{
    public class Line2
    {
        //-------FIELDS
        public readonly Rect lineRect;
        public readonly LinearFunction2 line;
        public readonly Vector2 _pointA;
        public readonly Vector2 _pointB;




        //-------METHODS
        public Line2(Vector2 pointA, Vector2 pointB)
        {
            _pointA = pointA;
            _pointB = pointB;

            Vector2 minimalPoint = new Vector2(Mathf.Min(pointA.x, pointB.x), Mathf.Min(pointA.y, pointB.y));
            Vector2 maximalPoint = new Vector2(Mathf.Max(pointA.x, pointB.x), Mathf.Max(pointA.y, pointB.y));
            lineRect = new Rect(minimalPoint, maximalPoint - minimalPoint);

            line = new LinearFunction2(pointA, pointA - pointB);
        }

        public List<Vector2> GetPoints(float stepSize)
        {
            Vector2 direction = (_pointA - _pointB).normalized;
            Vector2 step = direction * stepSize;

            List<Vector2> result = new List<Vector2>();
            Vector2 currentStep = _pointB;

            if (_pointA == _pointB)
                return result;

            while (currentStep.x * Mathf.Sign(direction.x) <= _pointA.x * Mathf.Sign(direction.x) &&
                   currentStep.y * Mathf.Sign(direction.y) <= _pointA.y * Mathf.Sign(direction.y))
            {
                result.Add(currentStep);
                currentStep += step;
            }
            return result;
        }
    }
}
