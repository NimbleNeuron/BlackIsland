using System;
using System.Collections.Generic;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class RestrictedAreaHud : BaseUI
	{
		[SerializeField] private Color normalColor = default;


		[SerializeField] private Color reservedColor = default;


		[SerializeField] private Color restrictedColor = default;


		private Image areaIcon;


		private Text areaName;


		private int currentAreaCode = -1;


		private Image dayNight;


		private Text days;


		private int displayRemainRestrictedTime;


		private int passDays;


		private float remainRestrictedTime;


		private Text survivableTimer;


		private GameObject survivableTimerObj;


		private Text timer;


		public int CurrentAreaCode => currentAreaCode;


		private void Update()
		{
			if (remainRestrictedTime > 0f && !MonoBehaviourInstance<ClientService>.inst.IsStopAreaRestriction)
			{
				remainRestrictedTime = Mathf.Max(0f, remainRestrictedTime - Time.deltaTime);
				int remainTime = Mathf.FloorToInt(remainRestrictedTime);
				UpdateTimer(remainTime);
			}
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			areaName = GameUtil.Bind<Text>(gameObject, "Area/Name");
			areaIcon = GameUtil.Bind<Image>(gameObject, "Area/Name/Icon");
			timer = GameUtil.Bind<Text>(gameObject, "RemainTime");
			survivableTimerObj = GameUtil.Bind<Transform>(gameObject, "SurvivableTimer").gameObject;
			survivableTimer = GameUtil.Bind<Text>(survivableTimerObj, "Time");
			dayNight = GameUtil.Bind<Image>(gameObject, "DayNight");
			days = GameUtil.Bind<Text>(gameObject, "Days");
			passDays = 0;
		}


		public void EnableSurvivableTimer(bool enable)
		{
			survivableTimerObj.SetActive(enable);
		}


		private void UpdateTimer(int remainTime)
		{
			if (displayRemainRestrictedTime == remainTime)
			{
				return;
			}

			displayRemainRestrictedTime = remainTime;
			int value = remainTime / 60;
			int value2 = remainTime % 60;
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.Append(GameUtil.IntToString(value, GameUtil.NumberOfDigits.One));
			stringBuilder.Append(" : ");
			stringBuilder.Append(GameUtil.IntToString(value2, GameUtil.NumberOfDigits.Two));
			timer.text = stringBuilder.ToString();
		}


		public void OnUpdateRestrictArea(LevelData currentLevel, Dictionary<int, AreaRestrictionState> areaStateMap,
			float remainTime, DayNight dayNight, int day)
		{
			remainRestrictedTime = remainTime;
			UpdateTimer(Mathf.FloorToInt(remainRestrictedTime));
			this.dayNight.sprite = dayNight.GetSprite();
			if (day > 0)
			{
				passDays = (day - 1) * 2 + (dayNight == DayNight.Night ? 1 : 0);
			}

			days.text = Ln.Format("DAYS", passDays / 2 + 1);
			passDays++;
			AreaData areaData;
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				areaData = SingletonMonoBehaviour<ObserverController>.inst.GetCurrentTargetAreaData(currentLevel);
			}
			else
			{
				MyPlayerContext myPlayer = MonoBehaviourInstance<ClientService>.inst.MyPlayer;
				areaData = myPlayer != null ? myPlayer.Character.GetCurrentAreaData(currentLevel) : null;
			}

			AreaData areaData2 = areaData;
			if (areaData2 != null && areaStateMap.ContainsKey(areaData2.code))
			{
				OnUpdateCurrentArea(areaData2.code, areaStateMap[areaData2.code]);
			}
		}


		public void OnUpdateCurrentArea(int areaCode, AreaRestrictionState state)
		{
			if (areaCode != currentAreaCode && MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.haveAllOwnItemFlag = false;
			}

			currentAreaCode = areaCode;
			areaName.text = areaCode == -1 ? Ln.Get("알 수 없음") : LnUtil.GetAreaName(areaCode);
			switch (state)
			{
				default:
					areaName.color = normalColor;
					areaIcon.color = normalColor;
					return;
				case AreaRestrictionState.Reserved:
					areaName.color = reservedColor;
					areaIcon.color = reservedColor;
					return;
				case AreaRestrictionState.Restricted:
					areaName.color = restrictedColor;
					areaIcon.color = restrictedColor;
					return;
			}
		}


		public void UpdateSurvivableTime(float remainTime)
		{
			int value = Mathf.FloorToInt(remainTime);
			survivableTimer.text = GameUtil.IntToString(value, GameUtil.NumberOfDigits.Two);
			if (Math.Abs(remainTime - 30f) > 0.001f)
			{
				Singleton<SoundControl>.inst.PlayUISound("RestrictedCount",
					ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
			}
		}
	}
}