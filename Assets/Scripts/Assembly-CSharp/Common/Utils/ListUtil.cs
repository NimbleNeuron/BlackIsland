using System.Collections.Generic;

namespace Common.Utils
{
	
	public class ListUtil
	{
		
		public static List<T> Random<T>(List<T> list)
		{
			List<T> list2 = new List<T>(list);
			List<T> list3 = new List<T>();
			while (list2.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list2.Count);
				list3.Add(list2[index]);
				list2.RemoveAt(index);
			}
			return list3;
		}
	}
}
