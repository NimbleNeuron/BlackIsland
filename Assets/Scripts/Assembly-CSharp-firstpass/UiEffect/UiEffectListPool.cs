using System.Collections.Generic;

namespace UiEffect
{
	public static class UiEffectListPool<T>
	{
		private static readonly UiEffectObjectPool<List<T>> s_ListPool =
			new UiEffectObjectPool<List<T>>(null, delegate(List<T> l) { l.Clear(); });


		public static List<T> Get()
		{
			return s_ListPool.Get();
		}


		public static void Release(List<T> toRelease)
		{
			s_ListPool.Release(toRelease);
		}
	}
}