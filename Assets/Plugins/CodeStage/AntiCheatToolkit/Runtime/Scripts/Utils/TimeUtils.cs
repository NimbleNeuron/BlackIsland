﻿#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#if UNITY_ANDROID && !UNITY_EDITOR
#define ACTK_ANDROID_DEVICE
#endif

namespace CodeStage.AntiCheat.Utils
{
	using System;
	using UnityEngine;

#if ACTK_ANDROID_DEVICE
	using Common;
#endif

	internal class TimeUtils
	{
		public const long TicksPerSecond = TimeSpan.TicksPerMillisecond * 1000;

#if ACTK_ANDROID_DEVICE
		private const string RoutinesClassPath = "net.codestage.actk.androidnative.ACTkAndroidRoutines";

		private static AndroidJavaClass routinesClass;
		private static bool androidTimeReadAttemptWasMade;
#endif

		/// <summary>
		/// Gets speed hacks unbiased current time ticks.
		/// </summary>
		/// <returns>Reliable current time in ticks.</returns>
		public static long GetReliableTicks()
		{
			long ticks = 0;
#if ACTK_ANDROID_DEVICE
			ticks = TryReadTicksFromAndroidRoutine();
#endif
			if (ticks == 0)
			{
				ticks = DateTime.UtcNow.Ticks;
			}

			return ticks;
		}

		public static long GetEnvironmentTicks()
		{
			return Environment.TickCount * TimeSpan.TicksPerMillisecond;
		}

		public static long GetRealtimeTicks()
		{
			return (long)(Time.realtimeSinceStartup * TicksPerSecond);
		}

#if ACTK_ANDROID_DEVICE
		private static long TryReadTicksFromAndroidRoutine()
		{
			long result = 0;

			if (!androidTimeReadAttemptWasMade)
			{
				androidTimeReadAttemptWasMade = true;

				try
				{
					routinesClass = new AndroidJavaClass(RoutinesClassPath);
				}
				catch (Exception e)
				{
					Debug.LogError(ACTkConstants.LogPrefix + "Couldn't create instance of the AndroidJavaClass: " + RoutinesClassPath + " !\n" + e);
				}
			}

			if (routinesClass == null) return result;

			try
			{
				// getting time in nanoseconds from the native Android timer
				// since some random fixed and JVM initialization point
				// (it even may be a future so value could be negative)
				result = routinesClass.CallStatic<long>("GetSystemNanoTime");
				result /= 100;
			}
			catch (Exception e)
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Couldn't call static method from the Android Routines Class!\n" + e);
			}

			return result;
		}

		~TimeUtils()
		{
			if (routinesClass != null)
			{
				routinesClass.Dispose();
			}
		}
#endif
	}
}