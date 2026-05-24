using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Utils.Extension_Methods
{
    public static class OtherExtensions
    {
        public static TObject Nullable<TObject>(this TObject obj) where TObject : UnityEngine.Object
        {
            if (obj is null) return null;
            return !obj ? null : obj;
        }

        public static bool IsUnityNull<T>(this T obj) where T : class
        {
            if (obj is Object uObj)
            {
                return uObj.Nullable() is null;
            }

            return obj is null;
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }

        public static string FromCamelToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
                .ToLower();
        }

        private static readonly HashSet<string> UpperCaseExceptionWords = new() { "of", "a" };
    
        public static string FromSnakeToCamelCase(this string str)
        {
            return str.Split(new [] {"_"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => UpperCaseExceptionWords.Contains(s) ? s 
                    : char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + " " + s2);
        }
    
        public static List<Transform> GetChildren(this Transform parent, Func<Transform, bool> filter = null)
        {
            var result = new List<Transform>();

            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);

                if (filter == null || filter.Invoke(child))
                {
                    result.Add(child);
                }
            }

            return result;
        }
    
        public static void ClearDontDestroyOnLoadScene()
        {
            // Get all root GameObjects in the scene
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            // Loop through each root GameObject
            foreach (var rootObject in rootObjects)
            {
                // Check if the GameObject is marked as DontDestroyOnLoad
                if (rootObject.scene.name == null)
                {
                    // Destroy the GameObject
                    Object.Destroy(rootObject);
                }
            }
        }
        
        public static bool IsAssignableFromDefinition(this Type extendType, Type baseType, out Type[] genericTypes)
        {
            Type[] lastGenericType = null;
            while (!baseType.IsAssignableFrom(extendType))
            {
                if (extendType == typeof(object))
                {
                    genericTypes = Array.Empty<Type>();
                    return false;
                }
                if (extendType.IsGenericType && !extendType.IsGenericTypeDefinition)
                {
                    lastGenericType = extendType.GetGenericArguments();
                    extendType = extendType.GetGenericTypeDefinition();
                }
                else
                {
                    extendType = extendType.BaseType;
                }
            }

            genericTypes = lastGenericType;
            return true;
        }
        
        public static string RemoveTrailingNumberInParentheses(this string input)
        {
            var index = input.LastIndexOf(" (");
            if (index != -1 && index < input.Length - 2)
            {
                var numberPart = input.Substring(index + 2, input.Length - index - 3);
                var closingParenthesis = input.Substring(input.Length - 1);
        
                if (closingParenthesis == ")" && int.TryParse(numberPart, out _))
                {
                    return input.Substring(0, index);
                }
            }
            return input;
        }

        public static int ToPercent(this float value)
        {
            return (int) Math.Round(value * 100);
        }
        
        // AI Generated
        public static Rect GetCanvasRect(this RectTransform rectTransform, Canvas canvas)
        {
            var canvasRect = canvas.transform as RectTransform;
        
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for (int i = 0; i < 4; i++)
            {
                Vector2 localPoint = canvasRect.InverseTransformPoint(worldCorners[i]);
                min = Vector2.Min(min, localPoint);
                max = Vector2.Max(max, localPoint);
            }
            
            var rect = new Rect(min, max - min);
            return rect;
        }

        public static Rect GetScreenRect(this RectTransform rt, Camera camera)
        {
            Vector3[] worldCorners = new Vector3[4];
            rt.GetWorldCorners(worldCorners);

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for (int i = 0; i < 4; i++)
            {
                Vector2 localPoint = camera.WorldToScreenPoint(worldCorners[i]);
                min = Vector2.Min(min, localPoint);
                max = Vector2.Max(max, localPoint);
            }
            
            var rect = new Rect(min, max - min);
            return rect;
        }
        
        public static float GetXAtY(Vector2 a, Vector2 b, float y)
        {
            float dy = b.y - a.y;
            if (Mathf.Approximately(dy, 0f))
                throw new InvalidOperationException("Нельзя вычислить X: прямая горизонтальна.");
            
            if ((y < Mathf.Min(a.y, b.y)) || (y > Mathf.Max(a.y, b.y)))
                throw new ArgumentOutOfRangeException(nameof(y), "y должно быть между a.y и b.y");

            float t = (y - a.y) / dy;
            return a.x + t * (b.x - a.x);
        }
        
        public static float GetYAtX(Vector2 a, Vector2 b, float x)
        {
            float dx = b.x - a.x;
            if (Mathf.Approximately(dx, 0f))
                throw new InvalidOperationException("Нельзя вычислить Y: прямая вертикальна.");
            
            if ((x < Mathf.Min(a.x, b.x)) || (x > Mathf.Max(a.x, b.x)))
                throw new ArgumentOutOfRangeException(nameof(x), "x должно быть между a.x и b.x");

            float t = (x - a.x) / dx;
            return a.y + t * (b.y - a.y);
        }
        
        // AI Generated
        public static Dictionary<TKey, List<T>> ToBuckets<T, TKey>(
            this IEnumerable<T> source,
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            var dict = new Dictionary<TKey, List<T>>(comparer);

            foreach (var item in source)
            {
                var key = keySelector(item);

                if (!dict.TryGetValue(key, out var list))
                {
                    dict[key] = list = new List<T>();
                }

                list.Add(item);
            }

            return dict;
        }
    }
}