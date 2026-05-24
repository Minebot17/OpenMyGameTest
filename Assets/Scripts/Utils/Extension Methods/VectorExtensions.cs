using UnityEngine;

namespace Utils.Extension_Methods
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this Vector2 vec, float z = 0)
        {
            return new Vector3(vec.x, vec.y, z);
        }
        
        public static Vector3 ToVector3Plane(this Vector2 vec, float y = 0)
        {
            return new Vector3(vec.x, y, vec.y);
        }

        public static Vector2 ToVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
        public static Vector2 ToVector2Plane(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
        public static Vector2Int ToVector2Int(this Vector2 vec)
        {
            return new Vector2Int((int) vec.x, (int) vec.y);
        }

        public static Vector3 Scale(this Vector3 vec, float val)
        {
            return new Vector3(vec.x * val, vec.y * val, vec.z * val);
        }
        
        public static Vector2 Scale(this Vector2 vec, float val)
        {
            return new Vector2(vec.x * val, vec.y * val);
        }
        
        public static Vector3 ChangeX(this Vector3 vec, float x)
        {
            return new Vector3(x, vec.x, vec.z);
        }

        public static Vector3 ChangeY(this Vector3 vec, float y)
        {
            return new Vector3(vec.x, y, vec.z);
        }
        
        public static Vector3 ChangeZ(this Vector3 vec, float z)
        {
            return new Vector3(vec.x, vec.y, z);
        }
    }
}