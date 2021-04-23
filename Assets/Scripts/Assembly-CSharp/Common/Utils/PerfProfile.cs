using UnityEngine;

namespace Common.Utils
{
	
	public static class PerfProfile
	{
		
		public static void Begin()
		{
			PerfProfile.beginTime = Time.realtimeSinceStartup;
		}

		
		public static float End(bool showLog)
		{
			float num = Time.realtimeSinceStartup - PerfProfile.beginTime;
			if (showLog)
			{
				Debug.Log(string.Format("Time: {0}ms ({1})", Mathf.Floor(num * 1000f) / 1000f, num));
			}
			return num;
		}

		
		private static float beginTime;
	}
}
