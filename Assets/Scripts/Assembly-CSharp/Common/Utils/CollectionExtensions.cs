using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Common.Utils
{
	
	public static class CollectionExtensions
	{
		
		[CanBeNull]
		public static TValue? ElementAtOrNull<TValue>(this IList<TValue> list, int index) where TValue : struct
		{
			if (index < 0 || index >= list.Count)
			{
				return null;
			}
			return new TValue?(list.ElementAt(index));
		}

		
		[CanBeNull]
		public static TValue? GetOrNull<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
			{
				return new TValue?(value);
			}
			return null;
		}

		
		public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
		{
			TValue result;
			if (!dictionary.TryGetValue(key, out result))
			{
				return defaultValue;
			}
			return result;
		}

		
		public static bool AddOnlyNotNull<TValue>(this ICollection<TValue> coll, TValue elem)
		{
			if (elem == null)
			{
				return false;
			}
			coll.Add(elem);
			return true;
		}

		
		public static bool AddOnlyNotZero(this ICollection<int> coll, int elem)
		{
			if (elem == 0)
			{
				return false;
			}
			coll.Add(elem);
			return true;
		}

		
		public static bool AddOnlyNotZero(this ICollection<long> coll, long elem)
		{
			if (elem == 0L)
			{
				return false;
			}
			coll.Add(elem);
			return true;
		}

		
		public static bool AddOnlyNotNull<TKey, TValue>(this SortedDictionary<TKey, TValue> coll, TKey key, TValue? elem) where TValue : struct
		{
			if (key == null || elem == null)
			{
				return false;
			}
			coll.Add(key, elem.Value);
			return true;
		}

		
		public static bool AddOnlyNotNull<TKey, TValue>(this SortedDictionary<TKey, TValue> coll, TKey key, TValue elem) where TValue : class
		{
			if (key == null || elem == null)
			{
				return false;
			}
			coll.Add(key, elem);
			return true;
		}

		
		public static int AddRangeOnlyNotNull<T>(this List<T> list, [CanBeNull] IEnumerable<T> elems)
		{
			if (elems == null)
			{
				return 0;
			}
			List<T> list2 = (from model in elems
			where model != null
			select model).ToList<T>();
			list.AddRange(list2);
			return list2.Count;
		}
	}
}
