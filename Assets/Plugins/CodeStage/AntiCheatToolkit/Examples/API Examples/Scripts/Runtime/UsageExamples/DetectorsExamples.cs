#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#define ACTK_TIME_CHEATING_DETECTOR_ENABLED

#if ACTK_PREVENT_INTERNET_PERMISSION
#undef ACTK_TIME_CHEATING_DETECTOR_ENABLED
#endif

#if (UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#define ACTK_INJECTION_DETECTOR_ENABLED
#endif

// add this line in order to use Detectors from code:
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	internal class DetectorsExamples : MonoBehaviour
	{
		internal bool injectionDetected;
		internal bool speedHackDetected;
#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
		internal bool wrongTimeDetected;
		internal bool timeCheatingDetected;
#endif
		internal bool obscuredTypeCheatDetected;
		internal bool wallHackCheatDetected;

		#region detection callbacks

		/* --------------------------------------------------------------------------------
		   these methods are get called by the Detection Events of detectors placed in the
		   ACTk Detectors game object, i.e. they are referenced from Inspector
		   -------------------------------------------------------------------------------- */

		public void OnSpeedHackDetected()
		{
			speedHackDetected = true;
			Debug.Log("Speed hack Detected!");
		}

#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
		public void OnTimeCheatingDetected()
		{
			timeCheatingDetected = true;
			Debug.Log("Time cheating Detected!");
		}
#endif

		public void OnInjectionDetected()
		{
			injectionDetected = true;
			Debug.Log("Injection Detected!");
		}

		// cause will have detection cause or full assembly name
		public void OnInjectionDetectedWithCause(string cause)
		{
			injectionDetected = true;
			Debug.Log("Injection Detected! Cause: " + cause);
		}

		public void OnObscuredTypeCheatingDetected()
		{
			obscuredTypeCheatDetected = true;
			Debug.Log("Obscured Vars Cheating Detected!");
		}

		public void OnWallHackDetected()
		{
			wallHackCheatDetected = true;
			Debug.Log("Wall hack Detected!");
		}

		/* --------------------------------------------------------------------------------
		   these methods are passed as arguments StartDetection in examples below
		   -------------------------------------------------------------------------------- */

		public void OnTimeCheatChecked(TimeCheatingDetector.CheckResult checkResult,
			TimeCheatingDetector.ErrorKind errorKind)
		{
#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
			switch (checkResult)
			{
				case TimeCheatingDetector.CheckResult.CheckPassed:
					Debug.Log("TimeCheatingDetector: Check passed, time seems to be fine.");
					break;
				case TimeCheatingDetector.CheckResult.WrongTimeDetected:
					wrongTimeDetected = true;
					Debug.LogWarning("TimeCheatingDetector: Wrong time Detected!");
					break;
				case TimeCheatingDetector.CheckResult.CheatDetected:
					timeCheatingDetected = true;
					Debug.LogWarning("TimeCheatingDetector: Time cheating Detected!");
					break;
				case TimeCheatingDetector.CheckResult.Unknown:
				case TimeCheatingDetector.CheckResult.Error:
					Debug.LogError("TimeCheatingDetector: some error occured: " + errorKind);
					break;
				default:
					Debug.LogError("TimeCheatingDetector: incorrect CheckResult value!");
					break;
			}
#endif
		}

		#endregion

		private void Start()
		{
			// All detectors have auto start option enabled by default, so you don't need to
			// write any code to start them generally.
			//
			// However, you still can start and control detectors from code.
			// You have 3 options for using detectors from code in general now:
			//
			// - configure detector in inspector, disable Auto Start,
			//   fill Detection Event and start it via StartDetection();
			//
			// - configure detector in inspector, disable Auto Start,
			//   do not fill Detection Event and start it via StartDetection(), passing detection callback;
			//
			// - do not add detector to your scene at all and create it completely from code using StartDetection();
			//
			// Also, generally, you have 3 options for the detection event listening:
			//
			// - configure Unity Event through the detector inspector
			// - subscribe to the [DetectorName].Instance.CheatDetected event
			// - pass reference to the delegate when starting detection from code

			SpeedHackDetectorExample();
			InjectionDetectorExample();
			ObscuredCheatingDetectorExample();
			TimeCheatingDetectorExample();

			// We can change all options of any detectors from code like this:
			// ObscuredCheatingDetector.StartDetection(OnObscuredTypeCheatingDetected);
			// ObscuredCheatingDetector.Instance.autoDispose = true;
			// ObscuredCheatingDetector.Instance.keepAlive = true;
		}

		private void SpeedHackDetectorExample()
		{
			// ---------------------------------------------
			/* SpeedHackDetector pure code usage example. */
			// ---------------------------------------------

			// SpeedHackDetector allows to detect speed hack applied
			// in tools like Cheat Engine, Game Guardian, etc.

			// In this case we subscribe to the speed hack detection event,
			// set detector update interval to 1 second, allowing 5 false positives and
			// allowing Cool Down after 60 seconds (read more about Cool Down in the readme.pdf).
			// Thus OnSpeedHackDetected normally will execute after 5 seconds since
			// speed hack was applied to the application.
			// Please, note, if we have detector added to scene, all settings
			// we made there in inspector will be overridden by settings we pass
			// to the StartDetection(); e.g.:
			// SpeedHackDetector.StartDetection(OnSpeedHackDetected, 1f, 5, 60);
		}

		private void InjectionDetectorExample()
		{
			// ---------------------------------------------
			/* InjectionDetector pure code usage example. */
			// ---------------------------------------------

			// InjectionDetector can detect foreign managed
			// assemblies loaded into your AppDomain
			//
			// build with IL2CPP to completely prevent managed injections
			// and make it harder to reverse-engineer your game

#if ACTK_INJECTION_DETECTOR_ENABLED
			// may be started with 1 string parameter callback to get the detection cause
			// InjectionDetector.StartDetection(OnInjectionDetectedWithCause);
#endif
		}

		private void ObscuredCheatingDetectorExample()
		{
			// ----------------------------------------------------
			/* ObscuredCheatingDetector pure code usage example. */
			// ----------------------------------------------------

			// When you start ObscuredCheatingDetector, it starts to
			// create fake unprotected versions of all obscured
			// variables in memory, making 'honey pot' for the cheaters
			// allowing them to find desired value and cheat it just to
			// be caught, original obscured value will stay untouched

			// that's it =D
			// ObscuredCheatingDetector.StartDetection(OnObscuredTypeCheatingDetected);
		}


		private void TimeCheatingDetectorExample()
		{
#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
			// --------------------------------------------------------------------
			/* TimeCheatingDetector pure code usage example and additional notes */
			// --------------------------------------------------------------------

			// TimeCheatingDetector can compare local time against
			// online time to detect wrong local time
			// and compare difference of their differences to detect local time cheating

			// TimeCheatingDetector can be disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional
			// which can be enabled in Settings window if you need to avoid
			// android.permission.INTERNET and you do not use TimeCheatingDetector in your project.

			// TimeCheatingDetector has this additional event: CheatChecked
			// it's get called after each cheat check and lets you know result of the check
			//
			// this is a preferred way over the inspector event as this CheatChecked event allows
			// you to see if check was successful or if user has incorrect time for example
			// (which may signalize for the suspicious activity)
			TimeCheatingDetector.Instance.CheatChecked += OnTimeCheatChecked;

			// also you may execute checks manually using one of these 3 methods:
			//
			// TimeCheatingDetector.Instance.ForceCheck()
			// and listen to callbacks passed to StartDetection
			//
			// yield return StartCoroutine(TimeCheatingDetector.Instance.ForceCheckEnumerator());
			// and check TimeCheatingDetector.Instance.LastResult property
			//
			// await TimeCheatingDetector.Instance.ForceCheckTask(); in async method (.NET 4.6 +)
			// and check TimeCheatingDetector.Instance.LastResult property or result of this call
#endif
		}

		// you can manually force TimeCheatingDetector cheating check
		// in case you don't wish to wait for the interval to elapse,
		// or if you wish to call it manually only, when interval is set to 0
		internal void ForceTimeCheatingDetectorCheck()
		{
#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
			if (TimeCheatingDetector.Instance != null && !TimeCheatingDetector.Instance.IsCheckingForCheat)
			{
				TimeCheatingDetector.Instance.ForceCheck();
			}
#endif
		}

	}
}