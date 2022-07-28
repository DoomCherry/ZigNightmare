using UnityEngine;

namespace Doomchery
{
    public class LinearFunction2
    {
        //-------FIELDS
        private readonly float _xMultiplier;
        private readonly float _constant;
        private readonly bool _xIsConstant;
        private readonly bool _yIsConstant;
        private readonly Vector2 _origin;



        //-------METHODS
        public LinearFunction2(Vector2 origin, Vector2 direction)
        {
            _origin = origin;
            if (direction.y == 0)
                _yIsConstant = true;

            if (direction.x == 0)
            {
                _constant = float.MaxValue;
                _xMultiplier = float.MaxValue;
                _xIsConstant = true;
            }
            else
            {
                _xMultiplier = direction.y / direction.x;
                _constant = (direction.x * origin.y - direction.y * origin.x) / direction.x;
            }
        }

        public Vector2 GetCoordsFromProjectionX(float x, float y)
        {
            if (!_yIsConstant && !_xIsConstant)
                return new Vector2(x, _xMultiplier * x - _constant);
            else if (_yIsConstant && !_xIsConstant)
                return new Vector2(x, _origin.y);
            else if (!_yIsConstant && _xIsConstant)
                return new Vector2(_origin.x, y);
            else
                return new Vector2(_origin.x, _origin.y);
        }

        public Vector2 GetCoordsFromProjectionY(float y, float x)
        {
            if (!_yIsConstant && !_xIsConstant)
                return new Vector2((y + _constant) / _xMultiplier, y);
            else if (_yIsConstant && !_xIsConstant)
                return new Vector2(x, _origin.y);
            else if (!_yIsConstant && _xIsConstant)
                return new Vector2(_origin.x, y);
            else
                return new Vector2(_origin.x, _origin.y);
        }
    }
}
