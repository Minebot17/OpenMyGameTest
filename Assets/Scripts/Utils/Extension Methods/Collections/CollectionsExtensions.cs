using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;

namespace Utils.Extension_Methods.Collections
{
    public static class CollectionsExtensions
    {
        private static Random rnd = new ();
    
        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
        
        public static void AddRange<T>(this ISet<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    
        public static List<T> ToList<T>(this HashSet<T> set)
        {
            var result = new List<T>();

            foreach (T element in set)
            {
                result.Add(element);
            }
        
            return result;
        }

        public static List<T> ToList<T>(this Queue<T> queue)
        {
            var result = new List<T>();

            for (int i = 0; i < queue.Count; i++)
            {
                result.Add(queue.Dequeue());
            }

            return result;
        }
    
        public static List<T> ToList<T>(this Stack<T> stack)
        {
            var result = new List<T>();

            for (int i = 0; i < stack.Count; i++)
            {
                result.Add(stack.Pop());
            }

            return result;
        }
    
        public static HashSet<T> ToHashSet<T>(this List<T> list)
        {
            var result = new HashSet<T>();

            foreach (T element in list)
            {
                result.Add(element);
            }
        
            return result;
        }

        public static Queue<T> ToQueue<T>(this List<T> list)
        {
            var result = new Queue<T>();

            foreach (T element in list)
            {
                result.Enqueue(element);
            }

            return result;
        }
    
        public static Stack<T> ToStack<T>(this List<T> list)
        {
            var result = new Stack<T>();

            for (int i = list.Count; i >= 0 ; i--)
            {
                result.Push(list[i]);
            }

            return result;
        }
    
        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rnd.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
    
        public static void Shuffle<T>(this T[] array)  
        {  
            int n = array.Length;  
            while (n > 1) {  
                n--;  
                int k = rnd.Next(n + 1);  
                (array[k], array[n]) = (array[n], array[k]);
            }  
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            return list[rnd.Next(0, list.Count)];
        }

        public static IList<T> GetRandomElements<T>(this IList<T> list, int count)
        {
            var newList = new List<T>(list);
            newList.Shuffle();
            return newList.GetRange(0, count);
        }
        
        public static T GetRandomNotNull<T>(this IList<T> list)
        {
            var filtered = list.Where(e => e != null).ToArray();
            return filtered[rnd.Next(0, filtered.Length)];
        }
        
        public static void RemoveAllBy<T>(this ObservableHashSet<T> set, Func<T, bool> predicate)
        {
            var toRemove = set.Where(predicate).ToArray();
            
            foreach (var item in toRemove)
            {
                set.Remove(item);
            }
        }
        
        public static void RemoveAllBy<T, N>(this ObservableDictionary<T, N> set, Func<KeyValuePair<T, N>, bool> predicate)
        {
            var toRemove = set.Where(predicate).ToArray();
            
            foreach (var item in toRemove)
            {
                set.Remove(item);
            }
        }
        
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return DistinctByIterator(source, keySelector, null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return DistinctByIterator(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            var seenKeys = comparer == null
                ? new HashSet<TKey>()
                : new HashSet<TKey>(comparer);

            foreach (var item in source)
            {
                if (seenKeys.Add(keySelector(item)))
                    yield return item;
            }
        }

        public static int IndexOf<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            var keys = dict.Keys.ToArray();
            
            for (var i = 0; i < keys.Length; i++)
            {
                if (key.Equals(keys[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}