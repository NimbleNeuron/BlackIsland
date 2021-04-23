using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.CrashReportHandler;
using UnityEngine.SceneManagement;

namespace Blis.Client
{
	public class GameAnalytics : SingletonMonoBehaviour<GameAnalytics>
	{
		protected override void OnAwakeSingleton()
		{
			base.OnAwakeSingleton();
			Analytics.enabled = true;
			Analytics.deviceStatsEnabled = true;
			AnalyticsEvent.debugMode = false;
			DontDestroyOnLoad(this);
		}


		public void Init(string version)
		{
			try
			{
				CrashReportHandler.SetUserMetadata("version", version);
				CrashReportHandler.SetUserMetadata("phase", "release");
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail SetUserMetadata : " + ex.Message);
			}
		}


		public void SetUserMetaData()
		{
			try
			{
				CrashReportHandler.SetUserMetadata("userId", Lobby.inst.User.UserNum.ToString());
				CrashReportHandler.SetUserMetadata("nickName", Lobby.inst.User.Nickname);
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail SetUserMetadata : " + ex.Message);
			}
		}


		public void UpdateGraphicsMetaData()
		{
			UpdateQualityLevel();
			UpdateVSyncCount();
			UpdateMaxQueuedFrames();
			UpdateScreen();
			UpdateTargetFrameRate();
		}


		public void UpdateQualityLevel()
		{
			try
			{
				CrashReportHandler.SetUserMetadata("qualityLevel", QualitySettings.GetQualityLevel().ToString());
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail UpdateQualityLevel : " + ex.Message);
			}
		}


		public void UpdateVSyncCount()
		{
			try
			{
				CrashReportHandler.SetUserMetadata("vSyncCount", QualitySettings.vSyncCount.ToString());
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail UpdateVSyncCount : " + ex.Message);
			}
		}


		public void UpdateMaxQueuedFrames()
		{
			try
			{
				CrashReportHandler.SetUserMetadata("maxQueuedFrames", QualitySettings.maxQueuedFrames.ToString());
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail UpdateMaxQueuedFrames : " + ex.Message);
			}
		}


		public void UpdateScreen()
		{
			try
			{
				CrashReportHandler.SetUserMetadata("screenWidth", Screen.width.ToString());
				CrashReportHandler.SetUserMetadata("screenHeight", Screen.height.ToString());
				CrashReportHandler.SetUserMetadata("fullScreenMode", Screen.fullScreenMode.ToString());
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail UpdateScreen : " + ex.Message);
			}
		}


		public void UpdateTargetFrameRate()
		{
			try
			{
				CrashReportHandler.SetUserMetadata("targetFrameRate", Application.targetFrameRate.ToString());
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail UpdateTargetFrameRate : " + ex.Message);
			}
		}


		public void UpdateActiveScene()
		{
			try
			{
				CrashReportHandler.SetUserMetadata("activeScene", SceneManager.GetActiveScene().name);
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail UpdateActiveScene : " + ex.Message);
			}
		}


		public void SetLoadScene(string sceneName)
		{
			try
			{
				CrashReportHandler.SetUserMetadata("loadScene", sceneName);
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail SetLoadScene : " + ex.Message);
			}
		}


		public void SetGameStatus(string gameStatus)
		{
			try
			{
				CrashReportHandler.SetUserMetadata("gameStatus", gameStatus);
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail SetGameStatus : " + ex.Message);
			}
		}


		public void SetSelectCharacter(int characterCode)
		{
			try
			{
				CrashReportHandler.SetUserMetadata("selectCharacter", characterCode.ToString());
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail SetSelectCharacter : " + ex.Message);
			}
		}


		public void SetSelectWeapon(int weaponCode)
		{
			try
			{
				CrashReportHandler.SetUserMetadata("selectWeapon", weaponCode.ToString());
			}
			catch (Exception ex)
			{
				Log.W("[GameAnalytics] Fail SetSelectWeapon : " + ex.Message);
			}
		}


		public void CustomEvent(string eventName, IDictionary<string, object> eventData = null)
		{
			try
			{
				AnalyticsResult analyticsResult = AnalyticsEvent.Custom(eventName, eventData);
				if (analyticsResult != AnalyticsResult.Ok)
				{
					Log.W("[GameAnalytics] AnalyticsResult : " + analyticsResult);
				}
			}
			catch { }
		}
	}
}