public class BSERVersion
{
	public static string VERSION {
		get
		{
#if LOCAL_SERVER
			return "0.25.4";
#else
			return "0.29.1";
#endif
		}
	}

	public static bool isDebugBuild {
		get
		{
#if UNITY_EDITOR
			return false;
#else 
			return UnityEngine.Debug.isDebugBuild;
#endif
		}
	}
}
