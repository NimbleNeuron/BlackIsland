using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheraBytes.BetterUi
{
	public class ResolutionMonitor : SingletonScriptableObject<ResolutionMonitor>
	{
		private static readonly Dictionary<string, ScreenTypeConditions> lookUpScreens =
			new Dictionary<string, ScreenTypeConditions>();


		private static Vector2 lastScreenResolution;


		private static float lastDpi;


		private static bool isDirty;


		[FormerlySerializedAs("optimizedResolution")] [SerializeField]
		private Vector2 optimizedResolutionFallback = new Vector2(1080f, 1920f);


		[FormerlySerializedAs("optimizedDpi")] [SerializeField]
		private float optimizedDpiFallback = 96f;


		[SerializeField] private string fallbackName = "Portrait";


		[SerializeField] private StaticSizerMethod[] staticSizerMethods = new StaticSizerMethod[5];


		[SerializeField] private DpiManager dpiManager = new DpiManager();


		[SerializeField] private List<ScreenTypeConditions> optimizedScreens = new List<ScreenTypeConditions>
		{
			new ScreenTypeConditions("Landscape", true, true)
		};


		private ScreenTypeConditions currentScreenConfig;


		private static string FilePath => "Standard Assets/TheraBytes/Resources/ResolutionMonitor";


		
		[Obsolete("Use 'GetOptimizedResolution()' instead.")]
		public static Vector2 OptimizedResolution {
			get => Instance.optimizedResolutionFallback;
			set
			{
				if (Instance.optimizedResolutionFallback == value)
				{
					return;
				}

				Instance.optimizedResolutionFallback = value;
				CallResolutionChanged();
			}
		}


		
		[Obsolete("Use 'GetOptimizedDpi()' instead.")]
		public static float OptimizedDpi {
			get => Instance.optimizedDpiFallback;
			set
			{
				if (Instance.optimizedDpiFallback == value)
				{
					return;
				}

				Instance.optimizedDpiFallback = value;
				CallResolutionChanged();
			}
		}


		public static Vector2 CurrentResolution {
			get
			{
				if (lastScreenResolution == Vector2.zero)
				{
					lastScreenResolution = new Vector2(Screen.width, Screen.height);
				}

				return lastScreenResolution;
			}
		}


		public static float CurrentDpi {
			get
			{
				if (lastDpi == 0f)
				{
					lastDpi = Instance.dpiManager.GetDpi();
				}

				return lastDpi;
			}
		}


		
		public string FallbackName {
			get => fallbackName;
			set => fallbackName = value;
		}


		public static Vector2 OptimizedResolutionFallback => Instance.optimizedResolutionFallback;


		public static float OptimizedDpiFallback => Instance.optimizedDpiFallback;


		public List<ScreenTypeConditions> OptimizedScreens => optimizedScreens;


		public static ScreenTypeConditions CurrentScreenConfiguration => Instance.currentScreenConfig;


		private void OnEnable()
		{
			ResolutionChanged();
		}


		public static ScreenTypeConditions GetConfig(string name)
		{
			if (lookUpScreens.Count == 0)
			{
				foreach (ScreenTypeConditions screenTypeConditions in Instance.optimizedScreens)
				{
					lookUpScreens.Add(screenTypeConditions.Name, screenTypeConditions);
				}
			}

			if (lookUpScreens.ContainsKey(name))
			{
				return lookUpScreens[name];
			}

			ScreenTypeConditions screenTypeConditions2 = Instance.optimizedScreens.FirstOrDefault(o => o.Name == name);
			if (screenTypeConditions2 != null)
			{
				lookUpScreens.Add(name, screenTypeConditions2);
				return screenTypeConditions2;
			}

			return null;
		}


		public static ScreenInfo GetOpimizedScreenInfo(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new ScreenInfo(OptimizedResolutionFallback, OptimizedDpiFallback);
			}

			return GetConfig(name).OptimizedScreenInfo;
		}


		public static IEnumerable<ScreenTypeConditions> GetCurrentScreenConfigurations()
		{
			foreach (ScreenTypeConditions screenTypeConditions in Instance.optimizedScreens)
			{
				if (screenTypeConditions.IsActive)
				{
					yield return screenTypeConditions;
				}
			}
		}


		public static float InvokeStaticMethod(ImpactMode mode, Component caller, Vector2 optimizedResolution,
			Vector2 actualResolution, float optimizedDpi, float actualDpi)
		{
			int num;
			switch (mode)
			{
				case ImpactMode.StaticMethod1:
					num = 0;
					break;
				case ImpactMode.StaticMethod2:
					num = 1;
					break;
				case ImpactMode.StaticMethod3:
					num = 2;
					break;
				case ImpactMode.StaticMethod4:
					num = 3;
					break;
				case ImpactMode.StaticMethod5:
					num = 4;
					break;
				default:
					throw new ArgumentException();
			}

			if (!HasInstance || Instance.staticSizerMethods[num] == null)
			{
				return 1f;
			}

			return Instance.staticSizerMethods[num]
				.Invoke(caller, optimizedResolution, actualResolution, optimizedDpi, actualDpi);
		}


		public static void SetResolutionDirty()
		{
			isDirty = true;
		}


		public static float GetOptimizedDpi(string screenName)
		{
			if (string.IsNullOrEmpty(screenName) || screenName == Instance.fallbackName)
			{
				return OptimizedDpiFallback;
			}

			ScreenTypeConditions screenTypeConditions =
				Instance.optimizedScreens.FirstOrDefault(o => o.Name == screenName);
			if (screenTypeConditions == null)
			{
				Debug.LogError("Screen Config with name " + screenName + " could not be found.");
				return OptimizedDpiFallback;
			}

			return screenTypeConditions.OptimizedDpi;
		}


		public static Vector2 GetOptimizedResolution(string screenName)
		{
			if (string.IsNullOrEmpty(screenName) || screenName == Instance.fallbackName)
			{
				return OptimizedResolutionFallback;
			}

			ScreenTypeConditions config = GetConfig(screenName);
			if (config == null)
			{
				return OptimizedResolutionFallback;
			}

			return config.OptimizedResolution;
		}


		public static bool IsOptimizedResolution(int width, int height)
		{
			if ((int) OptimizedResolutionFallback.x == width && (int) OptimizedResolutionFallback.y == height)
			{
				return true;
			}

			foreach (ScreenTypeConditions screenTypeConditions in Instance.optimizedScreens)
			{
				ScreenInfo optimizedScreenInfo = screenTypeConditions.OptimizedScreenInfo;
				if (optimizedScreenInfo != null && (int) optimizedScreenInfo.Resolution.x == width &&
				    (int) optimizedScreenInfo.Resolution.y == height)
				{
					return true;
				}
			}

			return false;
		}


		public static void Update()
		{
			isDirty = isDirty || GetCurrentResolution() != lastScreenResolution;
			if (isDirty)
			{
				CallResolutionChanged();
				isDirty = false;
			}
		}


		public static void CallResolutionChanged()
		{
			Instance.ResolutionChanged();
		}


		public void ResolutionChanged()
		{
			lastScreenResolution = GetCurrentResolution();
			lastDpi = GetCurrentDpi();
			currentScreenConfig = null;
			bool flag = false;
			foreach (ScreenTypeConditions screenTypeConditions in optimizedScreens)
			{
				if (screenTypeConditions.IsScreenType() && !flag)
				{
					currentScreenConfig = screenTypeConditions;
					flag = true;
				}
			}

			if (HasInstance)
			{
				foreach (IResolutionDependency resolutionDependency in AllResolutionDependencies())
				{
					resolutionDependency.OnResolutionChanged();
				}
			}
		}


		private static IEnumerable<IResolutionDependency> AllResolutionDependencies()
		{
			GameObject[] allObjects = FindObjectsOfType<GameObject>();
			GameObject[] array = allObjects;
			for (int i = 0; i < array.Length; i++)
			{
				OverrideScreenProperties[] components = array[i].GetComponents<OverrideScreenProperties>();
				foreach (OverrideScreenProperties resolutionDependency in components)
				{
					yield return resolutionDependency;
				}
			}

			array = null;
			array = allObjects;
			for (int i = 0; i < array.Length; i++)
			{
				IEnumerable<IResolutionDependency> enumerable =
					array[i].GetComponents<Component>().OfType<IResolutionDependency>();
				foreach (IResolutionDependency resolutionDependency2 in enumerable)
				{
					if (!(resolutionDependency2 is OverrideScreenProperties))
					{
						yield return resolutionDependency2;
					}
				}
			}
		}


		private static Vector2 GetCurrentResolution()
		{
			return new Vector2(Screen.width, Screen.height);
		}


		private float GetCurrentDpi()
		{
			return dpiManager.GetDpi();
		}
	}
}