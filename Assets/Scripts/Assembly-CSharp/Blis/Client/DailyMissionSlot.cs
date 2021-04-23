using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class DailyMissionSlot : BaseControl
	{
		public enum MissionState
		{
			Refresh,

			GetReward,

			Complete
		}


		private Button btnGetReward;


		private Button btnRefresh;


		private GameObject fxComplete;


		private Image imgGaugeValue;


		private MissionData missionData;


		private MissionState missionState;


		private PositionTweener positionTweener;


		private Text txtACoin;


		private Text txtEXP;


		private Text txtGaugeValue;


		private LnText txtTitle;


		private UserMission userMission;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			positionTweener = GameUtil.Bind<PositionTweener>(gameObject, "TweenPivot");
			txtTitle = GameUtil.Bind<LnText>(gameObject, "TweenPivot/TXT_Title");
			imgGaugeValue = GameUtil.Bind<Image>(gameObject, "TweenPivot/Gauge/IMG_GaugeValue");
			txtGaugeValue = GameUtil.Bind<Text>(gameObject, "TweenPivot/Gauge/TXT_GaugeValue");
			txtACoin = GameUtil.Bind<Text>(gameObject, "TweenPivot/TXT_ACoin");
			txtEXP = GameUtil.Bind<Text>(gameObject, "TweenPivot/TXT_EXP");
			btnRefresh = GameUtil.Bind<Button>(gameObject, "TweenPivot/BTN_Refresh");
			btnGetReward = GameUtil.Bind<Button>(gameObject, "TweenPivot/BTN_GetReward");
			fxComplete = transform.FindRecursively("FX_Complete").gameObject;
			btnRefresh.onClick.AddListener(delegate { Refresh(); });
			btnGetReward.onClick.AddListener(delegate { GetReward(); });
		}


		private int GetRefreshIndex()
		{
			List<UserMission> userDailyMissions = GlobalUserData.userDailyMissions;
			for (int i = 0; i < userDailyMissions.Count; i++)
			{
				if (userDailyMissions[i].missionCode == missionData.code)
				{
					return i;
				}
			}

			return -1;
		}


		public void SetDailyMission(UserMission userMission)
		{
			this.userMission = userMission;
			missionData = GameDB.mission.GetMissionData(userMission.missionCode, userMission.missionSeq);
			if (missionData == null)
			{
				Hide();
				return;
			}

			imgGaugeValue.fillAmount = userMission.progressCount / (float) missionData.count;
			txtTitle.text = Ln.Get(string.Format("Mission/{0}/{1}", missionData.code, missionData.seq));
			txtGaugeValue.text = string.Format("{0}/{1}", userMission.progressCount, missionData.count);
			txtACoin.text = missionData.rewardAcoin.ToString();
			txtEXP.text = missionData.rewardExp.ToString();
			if (imgGaugeValue.fillAmount >= 1f)
			{
				fxComplete.SetActive(true);
				btnRefresh.gameObject.SetActive(false);
				btnGetReward.gameObject.SetActive(true);
				missionState = MissionState.GetReward;
				positionTweener.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
			else
			{
				fxComplete.SetActive(false);
				btnRefresh.gameObject.SetActive(true);
				btnGetReward.gameObject.SetActive(false);
				missionState = MissionState.Refresh;
				positionTweener.transform.localPosition = new Vector3(-100f, 0f, 0f);
			}

			gameObject.SetActive(true);
		}


		public void Hide()
		{
			missionData = null;
			gameObject.SetActive(false);
		}


		private void Refresh()
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.Matching ||
			    Lobby.inst.LobbyContext.lobbyState == LobbyState.MatchCompleted)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭중에는 변경할 수 없습니다."), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			if (!Lobby.inst.DailyMissionRefreshFlag)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("도전과제 변경 초과"), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("도전과제 변경 가능"), new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("변경"),
				callback = delegate
				{
					RequestDelegate.request<MissionApi.RefreshMission>(
						MissionApi.GetRefreshMission(new MissionRefreshParam(missionData.type, missionData.code)),
						false, delegate(RequestDelegateError err, MissionApi.RefreshMission res)
						{
							if (err != null)
							{
								MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
								return;
							}

							if (res.userMission == null)
							{
								MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("미션 갱신 에러"));
								return;
							}

							int refreshIndex = GetRefreshIndex();
							GlobalUserData.userDailyMissions[refreshIndex] = res.userMission;
							Lobby.inst.SetDailyMissionRefreshFlag(false);
							SetDailyMission(res.userMission);
						});
				}
			}, new Popup.Button
			{
				type = Popup.ButtonType.Cancel,
				text = Ln.Get("취소")
			});
		}


		private void GetReward()
		{
			RequestDelegate.request<MissionApi.RewardMission>(
				MissionApi.GetMissionReward(new MissionRewardParam(missionData.code, missionData.seq)), false,
				delegate(RequestDelegateError err, MissionApi.RewardMission res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					int rewardAcoin = missionData.rewardAcoin;
					int rewardExp = missionData.rewardExp;
					if (res.userMission != null)
					{
						int refreshIndex = GetRefreshIndex();
						GlobalUserData.userDailyMissions[refreshIndex] = res.userMission;
						Lobby.inst.SetDailyMissionRefreshFlag(false);
					}
					else
					{
						GlobalUserData.userDailyMissions.Remove(userMission);
						Hide();
					}

					int prevLevel = Lobby.inst.User.Level;
					int currentLevel = res.userLevel;
					Lobby.inst.User.SetUserLevel(res.userLevel);
					Lobby.inst.User.SetUserNeedExp(res.userNeedExp);
					MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.Open();
					MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.AddReward(new RewardInfo(RewardType.DailyMission,
						new List<RewardItemInfo>
						{
							new RewardItemInfo(RewardItemType.ACoin, rewardAcoin),
							new RewardItemInfo(RewardItemType.XP, rewardExp)
						}));
					MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.ShowReward();
					if (currentLevel > prevLevel)
					{
						MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.levelUpEvent = delegate
						{
							MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.Open();
							MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.SetLevelUpReward(prevLevel, currentLevel,
								RewardType.LevelUp, rewardExp);
							GlobalUserData.myLevel = currentLevel;
						};
					}

					MonoBehaviourInstance<LobbyUI>.inst.UIEvent.SetAccountInfo(Lobby.inst.User.Nickname,
						Lobby.inst.User.Level, Lobby.inst.User.NeedXP);
					MonoBehaviourInstance<LobbyUI>.inst.UIEvent.SetDailyMissions();
				});
		}


		public void TweenPositionEnter()
		{
			Vector3 from = new Vector3(-100f, 0f, 0f);
			Vector3 to = new Vector3(0f, 0f, 0f);
			positionTweener.enabled = false;
			positionTweener.from = from;
			positionTweener.to = to;
			positionTweener.speed = 0.3f;
			positionTweener.PlayAnimation();
			positionTweener.enabled = true;
			positionTweener.OnAnimationFinish += delegate { };
		}


		public void TweenPositionExit()
		{
			positionTweener.enabled = false;
			positionTweener.transform.localPosition = new Vector3(-100f, 0f, 0f);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (missionState != MissionState.Refresh)
			{
				return;
			}

			TweenPositionEnter();
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			if (missionState != MissionState.Refresh)
			{
				return;
			}

			TweenPositionExit();
		}
	}
}