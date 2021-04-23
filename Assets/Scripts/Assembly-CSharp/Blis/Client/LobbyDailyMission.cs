using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LobbyDailyMission : BaseUI, ILnEventHander
	{
		private readonly DailyMissionSlot[] dailyMissionSlots = new DailyMissionSlot[3];
		private CanvasAlphaTweener canvasAlphaTweener;


		private GameObject missionComment;


		public void OnLnDataChange()
		{
			SetDailyMissions();
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			missionComment = transform.FindRecursively("MissionComment").gameObject;
			dailyMissionSlots[0] = GameUtil.Bind<DailyMissionSlot>(gameObject, "MissionSlot_1");
			dailyMissionSlots[1] = GameUtil.Bind<DailyMissionSlot>(gameObject, "MissionSlot_2");
			dailyMissionSlots[2] = GameUtil.Bind<DailyMissionSlot>(gameObject, "MissionSlot_3");
			GameUtil.Bind<CanvasAlphaTweener>(gameObject, ref canvasAlphaTweener);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
		}


		public void SetDailyMissions()
		{
			List<UserMission> userDailyMissions = GlobalUserData.userDailyMissions;
			bool active = userDailyMissions.Count <= 0;
			missionComment.SetActive(active);
			for (int i = 0; i < dailyMissionSlots.Length; i++)
			{
				if (i < userDailyMissions.Count)
				{
					dailyMissionSlots[i].SetDailyMission(userDailyMissions[i]);
				}
				else
				{
					dailyMissionSlots[i].Hide();
				}
			}
		}


		private void Show()
		{
			canvasAlphaTweener.from = 0f;
			canvasAlphaTweener.to = 1f;
			canvasAlphaTweener.PlayAnimation();
		}


		private void Hide()
		{
			canvasAlphaTweener.from = 1f;
			canvasAlphaTweener.to = 0f;
			canvasAlphaTweener.PlayAnimation();
		}
	}
}