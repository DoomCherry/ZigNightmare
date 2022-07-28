using UnityEngine;

namespace Doomchery
{
    public class LinearFunction3
    {
        //-------FIELDS
        public readonly float xMultiplier;
        public readonly float yMultiplier;
        public readonly float constant;
        public readonly bool xIsActievable = true;
        public readonly bool yIsActievable = true;
        public readonly bool zIsActievable = true;
        public readonly Vector3 origin;
        public readonly Vector3 direction;

        private LinearFunction2 _xyProjection, _xzProjection, _yzProjection;




        //-------METHODS
        public LinearFunction3(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
            _xyProjection = new LinearFunction2(origin, direction);
            _xzProjection = new LinearFunction2(new Vector2(origin.x, origin.z), new Vector2(direction.x, direction.z));
            _yzProjection = new LinearFunction2(new Vector2(origin.y, origin.z), new Vector2(direction.y, direction.z));
        }

        public Vector3 GetCoordsFromProjectionX(float x)
        {
            return new Vector3(x, _xyProjection.GetCoordsFromProjectionX(x, 0).y, _xzProjection.GetCoordsFromProjectionX(x, 0).y);
        }

        public Vector3 GetCoordsFromProjectionY(float y)
        {
            return new Vector3(_xyProjection.GetCoordsFromProjectionY(y, 0).x, y, _yzProjection.GetCoordsFromProjectionX(y, 0).y);
        }

        public Vector3 GetCoordsFromProjectionZ(float z)
        {
            return new Vector3(_xzProjection.GetCoordsFromProjectionY(z, 0).x, _yzProjection.GetCoordsFromProjectionY(z, 0).x, z);
        }

        public Vector3 FindNearestPointOnLine(Vector3 point)
        {
            Vector3 dir = direction.normalized;
            Vector3 lhs = point - origin;

            float dotP = Vector3.Dot(lhs, dir);
            return origin + dir * dotP;
        }
    }
}
