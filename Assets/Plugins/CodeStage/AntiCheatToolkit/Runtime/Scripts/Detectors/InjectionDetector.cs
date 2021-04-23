#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
#if (ACTK_INJECTION_DEBUG || ACTK_INJECTION_DEBUG_VERBOSE || ACTK_INJECTION_DEBUG_PARANOID)
#define ACTK_DEBUG_NORMAL
#endif

#if (ACTK_INJECTION_DEBUG_VERBOSE || ACTK_INJECTION_DEBUG_PARANOID)
#define ACTK_DEBUG_VERBOSE
#endif

#if (ACTK_INJECTION_DEBUG_PARANOID)
#define ACTK_DEBUG_PARANOID
#endif
#endif

#if (UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#define UNITY_SUPPORTED_PLATFORM
#endif

namespace CodeStage.AntiCheat.Detectors
{
	using Common;
	using ObscuredTypes;

	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using Debug = UnityEngine.Debug;
	using UnityEngine;

#if ACTK_DEBUG_NORMAL
	using System.Diagnostics;
#endif

	/// <summary>
	/// Allows to detect foreign managed assemblies in your application.
	/// </summary>
	/// Just add it to any GameObject as usual or through the "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit"
	/// menu to get started.<br/>
	/// You can use detector completely from inspector without writing any code except the actual reaction on cheating.
	/// 
	/// Avoid using detectors from code at the Awake phase.
	/// 
	/// <strong>\htmlonly<font color="7030A0">NOTE #1:</font>\endhtmlonly Make sure you've checked the 
	/// "Enable Injection Detector" option at the "Window > Code Stage > Anti-Cheat Toolkit > Settings" window
	/// before using detector at runtime.<br/>
	/// \htmlonly<font color="7030A0">NOTE #2:</font>\endhtmlonly Always test detector on the
	/// target platform before releasing your application to the public.<br/>
	/// It may detect some external assemblies as foreign,
	/// thus make sure you've added all external assemblies your application uses to the Whitelist (see section 
	/// "How to fill user-defined Whitelist" of the read me for details).<br/>
	/// \htmlonly<font color="7030A0">NOTE #3:</font>\endhtmlonly Disabled in Editor because of specific assemblies causing false positives. Use ACTK_INJECTION_DEBUG symbol to force it in Editor.
	/// 
	/// \htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Only Standalone and Android platforms are supported.</strong>
	[AddComponentMenu(MenuPath + ComponentName)]
	[DisallowMultipleComponent]
	[HelpURL(ACTkConstants.DocsRootUrl + "class_code_stage_1_1_anti_cheat_1_1_detectors_1_1_injection_detector.html")]
	public class InjectionDetector : ACTkDetectorBase<InjectionDetector>
	{
		public delegate void InjectionDetectedEventHandler(string reason);

		public const string ComponentName = "Injection Detector";
		internal const string FinalLogPrefix = ACTkConstants.LogPrefix + ComponentName + ": ";

#if UNITY_SUPPORTED_PLATFORM
		/// <summary>
		/// Subscribe to this event to get notified when cheat will be detected.
		/// The detection cause will be passed as an argument if possible.
		/// </summary>
		public new event InjectionDetectedEventHandler CheatDetected;

		#region private variables
		private bool signaturesAreMissing;
		private bool signaturesAreNotGenuine;
		private AllowedAssembly[] allowedAssemblies;
		private string[] hexTable;
		#endregion

		#region public static methods

		/// <summary>
		/// Creates new instance of the detector at scene if it doesn't exists. Make sure to call NOT from Awake phase.
		/// </summary>
		/// <returns>New or existing instance of the detector.</returns>
		public static InjectionDetector AddToSceneOrGetExisting()
		{
			return GetOrCreateInstance;
		}

		/// <summary>
		/// Starts foreign assemblies injection detection for detector you have in scene.
		/// </summary>
		/// Make sure you have properly configured detector in scene with #autoStart disabled before using this method.
		public static InjectionDetector StartDetection()
		{
			if (Instance != null)
			{
				return Instance.StartDetectionInternal(null);
			}

			Debug.LogError(FinalLogPrefix + "can't be started since it doesn't exists in scene or not yet initialized!");
			return null;
		}

		/// <summary>
		/// Starts foreign assemblies injection detection with specified callback containing string argument.<br/>
		/// Assembly name will be passed to the argument if possible. Otherwise another cause of the detection will be passed.
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="callback">Method to call after detection.</param>
		public static InjectionDetector StartDetection(InjectionDetectedEventHandler callback)
		{
			return GetOrCreateInstance.StartDetectionInternal(callback);
		}

		/// <summary>
		/// Stops detector. Detector's component remains in the scene. Use Dispose() to completely remove detector.
		/// </summary>
		public static void StopDetection()
		{
			if (Instance != null)
				Instance.StopDetectionInternal();
		}

		/// <summary>
		/// Stops and completely disposes detector component.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
		/// destroyed if no other Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		public static void Dispose()
		{
			if (Instance != null)
				Instance.DisposeInternal();
		}
		#endregion

		private InjectionDetector() { } // prevents direct instantiation

		private InjectionDetector StartDetectionInternal(InjectionDetectedEventHandler callback)
		{
			if (isRunning)
			{
				Debug.LogWarning(FinalLogPrefix + "already running!", this);
				return this;
			}

			if (!enabled)
			{
				Debug.LogWarning(FinalLogPrefix + "disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return this;
			}

			if ((CheatDetected != null || callback != null) && detectionEventHasListener)
			{
				Debug.LogWarning(FinalLogPrefix + "has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}

			if (CheatDetected == null && callback == null && !detectionEventHasListener)
			{
				Debug.LogWarning(FinalLogPrefix + "was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				enabled = false;
				return this;
			}

			CheatDetected += callback;
			started = true;
			isRunning = true;

			if (Application.isEditor)
			{
				Debug.Log(FinalLogPrefix + "does not work in Unity Editor.", this);
				return this;
			}

			if (allowedAssemblies == null)
			{
				LoadAndParseAllowedAssemblies();
			}

			if (signaturesAreMissing)
			{
				OnCheatingDetected("signatures are missing (did you enable " + ComponentName + "?)");
			}
			else if (signaturesAreNotGenuine)
			{
				OnCheatingDetected("signatures are not genuine or damaged");
				return this;
			}

			string cause;

			if (!FindInjectionInCurrentAssemblies(out cause))
			{
				// listening for new assemblies
				AppDomain.CurrentDomain.AssemblyLoad += OnNewAssemblyLoaded;
			}
			else
			{
				OnCheatingDetected(cause);
			}

			return this;
		}

		protected override void StartDetectionAutomatically()
		{
			StartDetectionInternal(null);
		}

		protected override void PauseDetector()
		{
			AppDomain.CurrentDomain.AssemblyLoad -= OnNewAssemblyLoaded;
			base.PauseDetector();
		}

		protected override bool ResumeDetector()
		{
			if (!base.ResumeDetector()) return false;

			AppDomain.CurrentDomain.AssemblyLoad += OnNewAssemblyLoaded;
			return true;
		}

		protected override bool DetectorHasCallbacks()
		{
			return base.DetectorHasCallbacks() || CheatDetected != null;
		}

		protected override void StopDetectionInternal()
		{
			if (started)
			{
				AppDomain.CurrentDomain.AssemblyLoad -= OnNewAssemblyLoaded;
			}
			base.StopDetectionInternal();
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (Instance == this) Instance = null;
		}

		protected override string GetComponentName()
		{
			return ComponentName;
		}

		private void OnCheatingDetected(string cause)
		{
			IsCheatDetected = true;

			if (CheatDetected != null)
				CheatDetected.Invoke(cause);

			if (detectionEventHasListener)
				detectionEvent.Invoke();

			if (autoDispose)
			{
				DisposeInternal();
			}
			else
			{
				StopDetectionInternal();
			}
		}

		private void OnNewAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
#if ACTK_DEBUG_NORMAL
			Debug.Log(ACTkConstants.LogPrefix + "New assembly loaded: " + args.LoadedAssembly.FullName, this);
#endif
			if (!AssemblyAllowed(args.LoadedAssembly))
			{
#if ACTK_DEBUG_NORMAL
				Debug.Log(ACTkConstants.LogPrefix + "Injected Assembly found:\n" + args.LoadedAssembly.FullName, this);
#endif
				OnCheatingDetected(args.LoadedAssembly.FullName);
			}
		}

		private bool FindInjectionInCurrentAssemblies(out string cause)
		{
			cause = null;
			var result = false;
#if ACTK_DEBUG_NORMAL
			var stopwatch = Stopwatch.StartNew();
#endif
			var assembliesInCurrentDomain = AppDomain.CurrentDomain.GetAssemblies();
			if (assembliesInCurrentDomain.Length == 0)
			{
#if ACTK_DEBUG_NORMAL
				stopwatch.Stop();
				Debug.Log(ACTkConstants.LogPrefix + "0 assemblies in current domain! Not genuine behavior.", this);
				stopwatch.Start();
#endif
				cause = "no assemblies";
				result = true;
			}
			else
			{
				foreach (var ass in assembliesInCurrentDomain)
				{
#if ACTK_DEBUG_VERBOSE
				stopwatch.Stop();
				Debug.Log(ACTkConstants.LogPrefix + "Currently loaded assembly:\n" + ass.FullName, this);
				stopwatch.Start();
#endif
					if (!AssemblyAllowed(ass))
					{
#if ACTK_DEBUG_NORMAL
						stopwatch.Stop();
						Debug.Log(ACTkConstants.LogPrefix + "Injected Assembly found:\n" + ass.FullName + "\n" + GetAssemblyHash(ass), this);
						stopwatch.Start();
#endif
						cause = ass.FullName;
						result = true;
						break;
					}
				}
			}

#if ACTK_DEBUG_NORMAL
			stopwatch.Stop();
			Debug.Log(ACTkConstants.LogPrefix + "Loaded assemblies scan duration: " + stopwatch.ElapsedMilliseconds + " ms.", this);
#endif
			return result;
		}

		private bool AssemblyAllowed(Assembly ass)
		{
			var assemblyName = ass.GetName().Name;
			var hash = GetAssemblyHash(ass);
			
			var result = false;
			for (var i = 0; i < allowedAssemblies.Length; i++)
			{
				var allowedAssembly = allowedAssemblies[i];

				if (allowedAssembly.name == assemblyName)
				{
					if (Array.IndexOf(allowedAssembly.hashes, hash) != -1)
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		private void LoadAndParseAllowedAssemblies()
		{
#if ACTK_DEBUG_NORMAL
			Debug.Log(ACTkConstants.LogPrefix + "Starting LoadAndParseAllowedAssemblies()", this);
			var sw = Stopwatch.StartNew();
#endif
			var assembliesSignatures = (TextAsset)Resources.Load("fndid", typeof(TextAsset));
			if (assembliesSignatures == null)
			{
				signaturesAreMissing = true;
				signaturesAreNotGenuine = true;
				return;
			}

#if ACTK_DEBUG_NORMAL
			sw.Stop();
			Debug.Log(ACTkConstants.LogPrefix + "Creating separator array and opening MemoryStream", this);
			sw.Start();
#endif

			string[] separator = {":"};

			var ms = new MemoryStream(assembliesSignatures.bytes);
			var br = new BinaryReader(ms, Encoding.Unicode);
			
			var count = br.ReadInt32();

#if ACTK_DEBUG_NORMAL
			sw.Stop();
			Debug.Log(ACTkConstants.LogPrefix + "Allowed assemblies count from MS: " + count, this);
			sw.Start();
#endif

			allowedAssemblies = new AllowedAssembly[count];

			for (var i = 0; i < count; i++)
			{
				var lineLength = br.ReadInt32();
				var line = br.ReadChars(lineLength);
#if ACTK_DEBUG_PARANOID
				sw.Stop();
				Debug.Log(ACTkConstants.LogPrefix + "Line: " + new string(line) + ", length: " + lineLength, this);
				sw.Start();
#endif
				var lineString = ObscuredString.Decrypt(line, ACTkConstants.StringKey);
#if ACTK_DEBUG_PARANOID
				sw.Stop();
				Debug.Log(ACTkConstants.LogPrefix + "Line decrypted : " + lineString, this);
				sw.Start();
#endif
				var strArr = lineString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				var stringsCount = strArr.Length;
#if ACTK_DEBUG_PARANOID
				sw.Stop();
				Debug.Log(ACTkConstants.LogPrefix + "stringsCount : " + stringsCount, this);
				sw.Start();
#endif
				if (stringsCount > 1)
				{
					var assemblyName = strArr[0];

					var hashes = new int[stringsCount - 1];
					for (var j = 1; j < stringsCount; j++)
					{
						var parseResult = 0;
						var success = int.TryParse(strArr[j], out parseResult);
						if (success)
						{
							hashes[j - 1] = parseResult;
						}
						else
						{
							Debug.LogError(FinalLogPrefix + "Could not parse value: " + strArr[j] + ", line:\n" + lineString);
						}
					}

					allowedAssemblies[i] = new AllowedAssembly(assemblyName, hashes);
				}
				else
				{
					signaturesAreNotGenuine = true;
					br.Close();
					ms.Close();
#if ACTK_DEBUG_NORMAL
					sw.Stop();
#endif
					return;
				}
			}
			br.Close();
			ms.Close();
			Resources.UnloadAsset(assembliesSignatures);

#if ACTK_DEBUG_NORMAL
			sw.Stop();
			Debug.Log(ACTkConstants.LogPrefix + "Allowed Assemblies parsing duration: " + sw.ElapsedMilliseconds + " ms.", this);
#endif

			hexTable = new string[256];
			for (var i = 0; i < 256; i++)
			{
				hexTable[i] = i.ToString("x2");
			}
		}

		private int GetAssemblyHash(Assembly ass)
		{
			string hashInfo;

			var assName = ass.GetName();
			var bytes = assName.GetPublicKeyToken();
			if (bytes.Length >= 8)
			{
				hashInfo = assName.Name + PublicKeyTokenToString(bytes);
			}
			else
			{
				hashInfo = assName.Name;
			}

			// based on Jenkins hash function (http://en.wikipedia.org/wiki/Jenkins_hash_function)
			var result = 0;
			var len = hashInfo.Length;

			for (var i = 0; i < len; ++i)
			{
				result += hashInfo[i];
				result += (result << 10);
				result ^= (result >> 6);
			}
			result += (result << 3);
			result ^= (result >> 11);
			result += (result << 15);

			return result;
		}

		private string PublicKeyTokenToString(byte[] bytes)
		{
			var result = "";

			// AssemblyName.GetPublicKeyToken() returns 8 bytes
			for (var i = 0; i < 8; i++)
			{
				result += hexTable[bytes[i]];
			}

			return result;
		}

		private class AllowedAssembly
		{
			public readonly string name;
			public readonly int[] hashes;

			public AllowedAssembly(string name, int[] hashes)
			{
				this.name = name;
				this.hashes = hashes;
			}
		}
#else
		//! @cond
		public static InjectionDetector AddToSceneOrGetExisting()
		{
			Debug.Log(FinalLogPrefix + "is not supported on current platform! This message is harmless.");
			return null;
		}

		public static void StartDetection()
		{
			Debug.Log(FinalLogPrefix + "is not supported on current platform! This message is harmless.");
		}

		public static void StartDetection(Action<string> callback)
		{
			Debug.Log(FinalLogPrefix + "is not supported on current platform! This message is harmless.");
		}

		public static void StopDetection()
		{
			Debug.Log(FinalLogPrefix + "is not supported on current platform! This message is harmless.");
		}

		public static void Dispose()
		{
			Debug.Log(FinalLogPrefix + "is not supported on current platform! This message is harmless.");
		}

		protected override void StartDetectionAutomatically()
		{
			Debug.Log(FinalLogPrefix + "is not supported on current platform! This message is harmless.");
		}

		protected override string GetComponentName()
		{
			return ComponentName;
		}
		//! @endcond
#endif
	}
}