using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterSelectObserverView : CharacterSelectView
	{
		private GameObject duoList;


		private List<ObserverViewTeamSlot> duoTeamSlots;


		private Button exitBtn;


		private MatchingTeamMode matchingTeamMode;


		private Action onClickExit;


		private Action<int, int> onClickTeam;


		private Text playerCount;


		private GameObject soloList;


		private List<ObserverViewTeamSlot> soloTeamSlots;


		private GameObject squardList;


		private List<ObserverViewTeamSlot> squardTeamSlots;


		private Text teamModeType;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			matchingTeamMode = MatchingTeamMode.None;
			Transform transform = GameUtil.Bind<Transform>(this.gameObject, "Content/PlayerListWindow/PlayerList");
			soloList = transform.Find("Solo").gameObject;
			soloTeamSlots = soloList.GetComponentsInChildren<ObserverViewTeamSlot>().ToList<ObserverViewTeamSlot>();
			duoList = transform.Find("Duo").gameObject;
			duoTeamSlots = duoList.GetComponentsInChildren<ObserverViewTeamSlot>().ToList<ObserverViewTeamSlot>();
			squardList = transform.Find("Squard").gameObject;
			squardTeamSlots = squardList.GetComponentsInChildren<ObserverViewTeamSlot>().ToList<ObserverViewTeamSlot>();
			GameObject gameObject = GameUtil
				.Bind<Transform>(this.gameObject, "Content/PlayerListWindow/CustomGameOption/OptionList").gameObject;
			teamModeType = GameUtil.Bind<Text>(gameObject, "List_1/Frame/Text");
			playerCount = GameUtil.Bind<Text>(gameObject, "List_2/Frame/Text");
			exitBtn = GameUtil.Bind<Button>(this.gameObject, "ButtonExit");
			exitBtn.onClick.AddListener(OnClickExit);
			exitBtn.gameObject.SetActive(false);
		}


		public override void SetOnClickExit(Action action)
		{
			onClickExit = action;
		}


		public void SetOnClickTeam(Action<int, int> action)
		{
			onClickTeam = action;
		}


		public override void Open()
		{
			base.Open();
			matchingTeamMode = MonoBehaviourInstance<MatchingService>.inst.MatchingTeamMode;
			Log.V("[OBSERVER] SelectView Open");
			SortedDictionary<int, Dictionary<long, MatchingService.MatchingUser>> allTeamInfo =
				MonoBehaviourInstance<MatchingService>.inst.GetAllTeamInfo();
			foreach (KeyValuePair<int, Dictionary<long, MatchingService.MatchingUser>> keyValuePair in allTeamInfo)
			{
				string text = string.Format("[OBSERVER] Team({0}) :", keyValuePair.Key);
				foreach (KeyValuePair<long, MatchingService.MatchingUser> keyValuePair2 in keyValuePair.Value)
				{
					text += string.Format(" {0}({1})", keyValuePair2.Value.NickName, keyValuePair2.Value.CharacterCode);
				}

				Log.V(text);
			}

			playerCount.text = allTeamInfo.Sum(x => x.Value.Count).ToString();
			switch (matchingTeamMode)
			{
				case MatchingTeamMode.Solo:
				{
					soloList.SetActive(true);
					duoList.SetActive(false);
					squardList.SetActive(false);
					teamModeType.text = Ln.Get("솔로");
					int num = 0;
					foreach (KeyValuePair<int, Dictionary<long, MatchingService.MatchingUser>> keyValuePair3 in
						allTeamInfo)
					{
						if (soloTeamSlots.Count > num)
						{
							soloTeamSlots[num].SetTeam(keyValuePair3.Key, keyValuePair3.Value);
							num++;
						}
					}

					for (int i = num; i < soloTeamSlots.Count; i++)
					{
						soloTeamSlots[i].Blank();
					}

					return;
				}
				case MatchingTeamMode.Duo:
				{
					soloList.SetActive(false);
					duoList.SetActive(true);
					squardList.SetActive(false);
					teamModeType.text = Ln.Get("듀오");
					int num2 = 0;
					foreach (KeyValuePair<int, Dictionary<long, MatchingService.MatchingUser>> keyValuePair4 in
						allTeamInfo)
					{
						if (duoTeamSlots.Count > num2)
						{
							duoTeamSlots[num2].SetTeam(keyValuePair4.Key, keyValuePair4.Value);
							num2++;
						}
					}

					for (int j = num2; j < duoTeamSlots.Count; j++)
					{
						duoTeamSlots[j].Blank();
					}

					return;
				}
				case MatchingTeamMode.Squad:
				{
					soloList.SetActive(false);
					duoList.SetActive(false);
					squardList.SetActive(true);
					teamModeType.text = Ln.Get("스쿼드");
					int num3 = 0;
					foreach (KeyValuePair<int, Dictionary<long, MatchingService.MatchingUser>> keyValuePair5 in
						allTeamInfo)
					{
						if (squardTeamSlots.Count > num3)
						{
							squardTeamSlots[num3].SetTeam(keyValuePair5.Key, keyValuePair5.Value);
							num3++;
						}
					}

					for (int k = num3; k < squardTeamSlots.Count; k++)
					{
						squardTeamSlots[k].Blank();
					}

					return;
				}
				default:
					return;
			}
		}


		public void OpenStartingView(MatchingTeamMode matchingTeamMode)
		{
			this.matchingTeamMode = matchingTeamMode;
			Log.V("[OBSERVER] StartingView Open");
			SortedDictionary<int, List<PlayerContext>> teams = MonoBehaviourInstance<ClientService>.inst.GetTeams();
			foreach (KeyValuePair<int, List<PlayerContext>> keyValuePair in teams)
			{
				string text = string.Format("[OBSERVER] Team({0}) :", keyValuePair.Key);
				foreach (PlayerContext playerContext in keyValuePair.Value)
				{
					text += string.Format(" {0}({1})", playerContext.nickname, playerContext.Character.CharacterCode);
				}

				Log.V(text);
			}

			playerCount.text = teams.Sum(x => x.Value.Count).ToString();
			switch (this.matchingTeamMode)
			{
				case MatchingTeamMode.Solo:
					soloList.SetActive(true);
					duoList.SetActive(false);
					squardList.SetActive(false);
					teamModeType.text = Ln.Get("솔로");
					SetTeamSlot(soloTeamSlots, teams);
					return;
				case MatchingTeamMode.Duo:
					soloList.SetActive(false);
					duoList.SetActive(true);
					squardList.SetActive(false);
					teamModeType.text = Ln.Get("듀오");
					SetTeamSlot(duoTeamSlots, teams);
					return;
				case MatchingTeamMode.Squad:
					soloList.SetActive(false);
					duoList.SetActive(false);
					squardList.SetActive(true);
					teamModeType.text = Ln.Get("스쿼드");
					SetTeamSlot(squardTeamSlots, teams);
					return;
				default:
					return;
			}
		}


		private void SetTeamSlot(List<ObserverViewTeamSlot> teamSlots,
			SortedDictionary<int, List<PlayerContext>> allTeamInfo)
		{
			Dictionary<long, MatchingService.MatchingUser> dictionary =
				new Dictionary<long, MatchingService.MatchingUser>();
			int num = 0;
			foreach (KeyValuePair<int, List<PlayerContext>> keyValuePair in allTeamInfo)
			{
				if (teamSlots.Count > num)
				{
					dictionary.Clear();
					foreach (PlayerContext playerContext in keyValuePair.Value)
					{
						dictionary.Add(playerContext.userId,
							new MatchingService.MatchingUser(playerContext.GetTeamSlot(), false, playerContext.userId,
								playerContext.nickname, playerContext.Character.CharacterCode,
								playerContext.startingWeaponCode, true));
					}

					teamSlots[num].SetTeam(keyValuePair.Key, dictionary);
					teamSlots[num].SetClickEvent(OnClickTeam);
					num++;
				}
			}

			for (int i = num; i < teamSlots.Count; i++)
			{
				teamSlots[i].Blank();
			}
		}


		private void OnClickTeam(int teamNumber, int teamSlot)
		{
			Action<int, int> action = onClickTeam;
			if (action == null)
			{
				return;
			}

			action(teamNumber, teamSlot);
		}


		public void SelectTeam(int teamNumber)
		{
			switch (matchingTeamMode)
			{
				case MatchingTeamMode.Solo:
					SelectTeam(soloTeamSlots, teamNumber);
					return;
				case MatchingTeamMode.Duo:
					SelectTeam(duoTeamSlots, teamNumber);
					return;
				case MatchingTeamMode.Squad:
					SelectTeam(squardTeamSlots, teamNumber);
					return;
				default:
					return;
			}
		}


		private void SelectTeam(List<ObserverViewTeamSlot> teamSlots, int teamNumber)
		{
			for (int i = 0; i < teamSlots.Count; i++)
			{
				if (teamSlots[i].TeamNumber == teamNumber)
				{
					teamSlots[i].Select();
				}
				else
				{
					teamSlots[i].Deselect();
				}
			}
		}


		public override void CharacterSelect(int teamNumber, long userNum, int characterCode, int startingDataCode)
		{
			ObserverViewCharacterSlot playerSlot = GetPlayerSlot(teamNumber, userNum);
			if (playerSlot == null)
			{
				return;
			}

			playerSlot.SetCharacter(characterCode);
			playerSlot.SetWeaponType(startingDataCode);
		}


		public override void SkinSelect(int teamNumber, MatchingService.MatchingUser userInfo)
		{
			ObserverViewCharacterSlot playerSlot = GetPlayerSlot(teamNumber, userInfo.UserNum);
			if (playerSlot == null)
			{
				return;
			}

			playerSlot.SetSkin(userInfo.CharacterCode, userInfo.SkinCode);
		}


		public override void WeaponSelect(int teamNumber, long userNum, int startingDataCode)
		{
			ObserverViewCharacterSlot playerSlot = GetPlayerSlot(teamNumber, userNum);
			if (playerSlot == null)
			{
				return;
			}

			playerSlot.SetWeaponType(startingDataCode);
		}


		public override void PickCharacter(int teamNumber, MatchingService.MatchingUser userInfo)
		{
			ObserverViewCharacterSlot playerSlot = GetPlayerSlot(teamNumber, userInfo.UserNum);
			if (playerSlot == null)
			{
				return;
			}

			playerSlot.SetPick(true);
			playerSlot.PlayPickEffect();
		}


		public override void CharacterCancelPick(int teamNumber, MatchingService.MatchingUser userInfo)
		{
			ObserverViewCharacterSlot playerSlot = GetPlayerSlot(teamNumber, userInfo.UserNum);
			if (playerSlot == null)
			{
				return;
			}

			playerSlot.SetPick(false);
		}


		private void OnClickExit()
		{
			Action action = onClickExit;
			if (action == null)
			{
				return;
			}

			action();
		}


		private ObserverViewCharacterSlot GetPlayerSlot(int teamNumber, long userNum)
		{
			switch (MonoBehaviourInstance<MatchingService>.inst.MatchingTeamMode)
			{
				case MatchingTeamMode.Solo:
					for (int i = 0; i < soloTeamSlots.Count; i++)
					{
						if (soloTeamSlots[i].TeamNumber == teamNumber)
						{
							return soloTeamSlots[i].GetSlot(userNum);
						}
					}

					break;
				case MatchingTeamMode.Duo:
					for (int j = 0; j < duoTeamSlots.Count; j++)
					{
						if (duoTeamSlots[j].TeamNumber == teamNumber)
						{
							return duoTeamSlots[j].GetSlot(userNum);
						}
					}

					break;
				case MatchingTeamMode.Squad:
					for (int k = 0; k < squardTeamSlots.Count; k++)
					{
						if (squardTeamSlots[k].TeamNumber == teamNumber)
						{
							return squardTeamSlots[k].GetSlot(userNum);
						}
					}

					break;
			}

			return null;
		}
	}
}