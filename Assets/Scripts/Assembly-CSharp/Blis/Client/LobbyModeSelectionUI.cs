using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyModeSelectionUI : BaseUI
	{
		public const string NormalMenu = "Normal";


		public const string RankMenu = "Rank";


		[SerializeField] private GameObject gameModeList = default;


		private readonly List<MenuGroup> menuGroups = new List<MenuGroup>();


		private MenuGroup normalMenuGroup;


		private MenuGroup rankMenuGroup;


		private MatchingMode selectedMatchingMode;


		private string selectedMenuGroupName = string.Empty;


		public MatchingMode SelectedMatchingMode => selectedMatchingMode;


		private void LateUpdate()
		{
			if (selectedMenuGroupName.Equals("Normal") && normalMenuGroup != null)
			{
				bool flag = Lobby.inst != null && Lobby.inst.HasMatchingPenalty(MatchingMode.Normal);
				normalMenuGroup.SetContents(flag);
				normalMenuGroup.SetPenalty(flag, MatchingMode.Normal);
			}

			if (selectedMenuGroupName.Equals("Rank") && rankMenuGroup != null)
			{
				bool flag2 = Lobby.inst != null && Lobby.inst.HasMatchingPenalty(MatchingMode.Rank);
				rankMenuGroup.SetContents(flag2);
				rankMenuGroup.SetPenalty(flag2, MatchingMode.Rank);
			}
		}

		
		
		public event Action<string> OnGameModeSelect;


		protected override void OnStartUI()
		{
			base.OnStartUI();
			Transform transform = gameModeList.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				menuGroups.Add(new MenuGroup(transform.GetChild(i).gameObject));
			}

			menuGroups.ForEach(delegate(MenuGroup x) { x.Close(); });
			menuGroups.ForEach(delegate(MenuGroup x) { x.gameModeSelect = OnGameModeSelect; });
			normalMenuGroup = menuGroups.Find(x => x.MenuGroupName == "Normal");
			rankMenuGroup = menuGroups.Find(x => x.MenuGroupName == "Rank");
			if (GlobalUserData.matchingMode == MatchingMode.Rank)
			{
				selectedMenuGroupName = "Rank";
				selectedMatchingMode = MatchingMode.Rank;
				rankMenuGroup.Open();
				return;
			}

			selectedMenuGroupName = "Normal";
			selectedMatchingMode = MatchingMode.Normal;
			normalMenuGroup.Open();
		}


		public void OnClickItem(GameObject item)
		{
			for (int i = 0; i < menuGroups.Count; i++)
			{
				if (menuGroups[i].Item == item)
				{
					selectedMenuGroupName = menuGroups[i].MenuGroupName;
					if (selectedMenuGroupName.Equals("Normal"))
					{
						selectedMatchingMode = MatchingMode.Normal;
					}

					if (selectedMenuGroupName.Equals("Rank"))
					{
						selectedMatchingMode = MatchingMode.Rank;
					}

					menuGroups[i].Open();
				}
				else
				{
					menuGroups[i].Close();
				}
			}
		}


		public string GetSelectedMode()
		{
			return (from x in menuGroups
				where x.IsActive
				select x.GetSelectedModeName()).FirstOrDefault<string>();
		}


		public int GetSelectedModeIndex()
		{
			return (from x in menuGroups
				where x.IsActive
				select x.GetSelectedModeIndex()).FirstOrDefault<int>();
		}


		public List<string> GetAvailableMode()
		{
			MenuGroup menuGroup = menuGroups.FirstOrDefault(x => x.IsActive);
			if (menuGroup == null)
			{
				return new List<string>();
			}

			return menuGroup.GetAvailableMode();
		}


		public void ClickCreateCustomRoom()
		{
			if (CommunityService.HasLobby())
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("팀구성 중에는 이용할 수 없습니다."), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			if (GlobalUserData.matchingRegion == MatchingRegion.None)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("접속 가능 지역 없음"), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			if (MonoBehaviourInstance<LobbyService>.inst.LobbyState == LobbyState.Matching ||
			    MonoBehaviourInstance<LobbyService>.inst.LobbyState == LobbyState.MatchCompleted)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 중에는 커스텀 모드에 입장할 수 없습니다."), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			MonoBehaviourInstance<LobbyUI>.inst.CustomModeSelectionWindow.Open();
		}


		public void ClickJoinCustomRoom()
		{
			if (CommunityService.HasLobby())
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("팀구성 중에는 이용할 수 없습니다."), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			MonoBehaviourInstance<LobbyUI>.inst.JoinCustomGameRoomWindow.Open();
		}


		public void ActiveMatchMode(List<bool> modeActives)
		{
			SetActiveMode(modeActives);
			DefaultSelectMode(modeActives);
			if (selectedMenuGroupName.Equals("Rank"))
			{
				SetRankTierChange();
			}
		}


		public void SetActiveMode(List<bool> modeActiveStates)
		{
			MenuGroup menuGroup = null;
			if (selectedMenuGroupName.Equals("Normal"))
			{
				menuGroup = normalMenuGroup;
			}
			else if (selectedMenuGroupName.Equals("Rank"))
			{
				menuGroup = rankMenuGroup;
			}

			if (menuGroup != null)
			{
				for (int i = 0; i < menuGroup.Modes.Count; i++)
				{
					menuGroup.Modes[i].SetDisableActive(!modeActiveStates[i]);
				}
			}
		}


		public void DefaultSelectMode(List<bool> modeActives)
		{
			MenuGroup menuGroup = null;
			if (selectedMenuGroupName.Equals("Normal"))
			{
				menuGroup = normalMenuGroup;
			}
			else if (selectedMenuGroupName.Equals("Rank"))
			{
				menuGroup = rankMenuGroup;
			}

			int num = GlobalUserData.matchingTeamMode - MatchingTeamMode.Solo;
			if (menuGroup != null)
			{
				if (num != -1)
				{
					if (modeActives[num])
					{
						if (!menuGroup.Modes[num].GetDisableActive())
						{
							menuGroup.Modes[num].SetIsOn(true);
						}
					}
					else
					{
						int num2 = modeActives.FindIndex(x => x);
						if (num2 != -1)
						{
							menuGroup.Modes[num2].SetIsOn(true);
						}
					}
				}
				else
				{
					int num3 = modeActives.FindIndex(x => x);
					if (num3 != -1)
					{
						menuGroup.Modes[num3].SetIsOn(true);
					}
				}
			}
		}


		public void SetRankTierChange()
		{
			if (rankMenuGroup != null)
			{
				for (int i = 0; i < rankMenuGroup.Modes.Count; i++)
				{
					MatchingTeamMode matchingTeamMode = MatchingTeamMode.None;
					string modeName = rankMenuGroup.Modes[i].ModeName;
					if (!(modeName == "Solo"))
					{
						if (!(modeName == "Duo"))
						{
							if (modeName == "Squad")
							{
								matchingTeamMode = MatchingTeamMode.Squad;
							}
						}
						else
						{
							matchingTeamMode = MatchingTeamMode.Duo;
						}
					}
					else
					{
						matchingTeamMode = MatchingTeamMode.Solo;
					}

					rankMenuGroup.Modes[i].SetTierChange(Lobby.inst.GetTierChageTypeMode(matchingTeamMode));
				}
			}
		}


		private class MenuGroup
		{
			private readonly Transform activeHeader;


			private readonly Transform contents;


			private readonly Transform inactiveHeader;


			private readonly Transform penalty;


			private readonly LnText penaltyContents;


			private readonly LnText penaltyHead;


			private readonly LnText penaltyTime;


			public Action<string> gameModeSelect;


			private bool isActive;


			public MenuGroup(GameObject item)
			{
				Item = item;
				MenuGroupName = item.name;
				inactiveHeader = GameUtil.Bind<Transform>(item, "Head/Inactive");
				activeHeader = GameUtil.Bind<Transform>(item, "Head/Active");
				contents = GameUtil.Bind<Transform>(item, "Contents");
				penalty = item.transform.FindRecursively("Penalty");
				if (penalty != null)
				{
					penaltyHead = GameUtil.Bind<LnText>(penalty.gameObject, "Bg/Head");
					penaltyContents = GameUtil.Bind<LnText>(penalty.gameObject, "Bg/Contents");
					penaltyTime = GameUtil.Bind<LnText>(penalty.gameObject, "Bg/Time");
				}

				Modes = new List<Mode>();
				for (int i = 0; i < contents.childCount; i++)
				{
					Mode mode = new Mode(contents.GetChild(i));
					Modes.Add(mode);
				}
			}


			public GameObject Item { get; }


			public string MenuGroupName { get; }


			public List<Mode> Modes { get; }


			public bool IsActive => isActive;


			public void Open()
			{
				inactiveHeader.gameObject.SetActive(false);
				activeHeader.gameObject.SetActive(true);
				bool hasPenalty = false;
				if (MenuGroupName.Equals("Normal"))
				{
					hasPenalty = Lobby.inst != null && Lobby.inst.HasMatchingPenalty(MatchingMode.Normal);
					SetPenalty(hasPenalty, MatchingMode.Normal);
				}

				if (MenuGroupName.Equals("Rank"))
				{
					hasPenalty = Lobby.inst != null && Lobby.inst.HasMatchingPenalty(MatchingMode.Rank);
					SetPenalty(hasPenalty, MatchingMode.Rank);
				}

				SetContents(hasPenalty);
				isActive = true;
				gameModeSelect(Item.name);
			}


			public void Close()
			{
				inactiveHeader.gameObject.SetActive(true);
				activeHeader.gameObject.SetActive(false);
				contents.gameObject.SetActive(false);
				Transform transform = penalty;
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}

				isActive = false;
			}


			public void SetContents(bool hasPenalty)
			{
				if (hasPenalty)
				{
					if (contents.gameObject.activeSelf)
					{
						contents.gameObject.SetActive(false);
					}

					if (penalty != null && !penalty.gameObject.activeSelf)
					{
						penalty.gameObject.SetActive(true);
					}
				}
				else
				{
					if (!contents.gameObject.activeSelf)
					{
						contents.gameObject.SetActive(true);
					}

					if (penalty != null && penalty.gameObject.activeSelf)
					{
						penalty.gameObject.SetActive(false);
					}
				}
			}


			public void SetPenalty(bool hasPenalty, MatchingMode matchingMode)
			{
				if (penalty == null || !hasPenalty)
				{
					return;
				}

				if (matchingMode == MatchingMode.Normal)
				{
					penaltyHead.gameObject.SetActive(true);
					penaltyContents.gameObject.SetActive(true);
					penaltyTime.gameObject.SetActive(true);
					penaltyHead.text = Ln.Get("매칭 이후 게임을 이탈하여 패널티가 적용중입니다.");
					penaltyTime.text = Lobby.inst.GetMatchingPenaltyTime(matchingMode);
					return;
				}

				if (matchingMode == MatchingMode.Rank)
				{
					if (Lobby.inst.HasMatchingPenaltyRankOpen())
					{
						penaltyHead.gameObject.SetActive(true);
						penaltyContents.gameObject.SetActive(false);
						penaltyTime.gameObject.SetActive(false);
						penaltyHead.text = Ln.Get("랭크 시즌 종료 설명");
						return;
					}

					if (Lobby.inst.HasMatchingPenaltyRankLevel())
					{
						penaltyHead.gameObject.SetActive(true);
						penaltyContents.gameObject.SetActive(false);
						penaltyTime.gameObject.SetActive(false);
						penaltyHead.text = Ln.Format("랭크 레벨 제한", 20);
						return;
					}

					if (Lobby.inst.HasMatchingPenaltyRankCharacter())
					{
						penaltyHead.gameObject.SetActive(true);
						penaltyContents.gameObject.SetActive(false);
						penaltyTime.gameObject.SetActive(false);
						penaltyHead.text = Ln.Format("랭크 캐릭터 제한", 3);
						return;
					}

					penaltyHead.gameObject.SetActive(true);
					penaltyContents.gameObject.SetActive(true);
					penaltyTime.gameObject.SetActive(true);
					penaltyHead.text = Ln.Get("매칭 이후 게임을 이탈하여 패널티가 적용중입니다.");
					penaltyTime.text = Lobby.inst.GetMatchingPenaltyTime(matchingMode);
				}
			}


			public string GetSelectedModeName()
			{
				return (from x in Modes
					where x.IsOn()
					select x.ModeName).FirstOrDefault<string>();
			}


			public int GetSelectedModeIndex()
			{
				return Modes.FindIndex(x => x.IsOn());
			}


			public List<string> GetAvailableMode()
			{
				return (from m in Modes
					where !m.GetDisableActive()
					select m.ModeName).ToList<string>();
			}


			public class Mode
			{
				private readonly GameObject disable;


				private readonly EventTrigger eventTrigger;


				private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


				private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


				private readonly Image tierChange;


				private readonly Toggle toggle;


				private readonly Vector2 tooltipPos;


				private string strTierChange;


				private Transform tr;


				public Mode(Transform tr)
				{
					ModeName = tr.name;
					if (ModeName.Equals("Trio"))
					{
						ModeName = "Squad";
					}

					Transform transform = tr.FindRecursively("CheckBox");
					if (transform != null)
					{
						toggle = transform.GetComponent<Toggle>();
					}

					Transform transform2 = tr.FindRecursively("Disable");
					if (transform2 != null)
					{
						disable = transform2.gameObject;
					}

					Transform transform3 = tr.FindRecursively("TierChange");
					if (transform3 != null)
					{
						tierChange = transform3.GetComponent<Image>();
						tooltipPos = transform3.gameObject.transform.position;
						tooltipPos += GameUtil.ConvertPositionOnScreenResolution(-410f, 30f);
						GameUtil.BindOrAdd<EventTrigger>(transform3.gameObject, ref eventTrigger);
						onEnterEvent.AddListener(OnPointerEnter);
						onExitEvent.AddListener(OnPointerExit);
						eventTrigger.triggers.Add(new EventTrigger.Entry
						{
							eventID = EventTriggerType.PointerEnter,
							callback = onEnterEvent
						});
						eventTrigger.triggers.Add(new EventTrigger.Entry
						{
							eventID = EventTriggerType.PointerExit,
							callback = onExitEvent
						});
					}

					if (toggle != null)
					{
						toggle.onValueChanged.AddListener(delegate(bool isOn)
						{
							if (isOn)
							{
								string a = ModeName;
								if (a == "Solo")
								{
									GlobalUserData.matchingTeamMode = MatchingTeamMode.Solo;
									return;
								}

								if (a == "Duo")
								{
									GlobalUserData.matchingTeamMode = MatchingTeamMode.Duo;
									return;
								}

								if (!(a == "Squad"))
								{
									return;
								}

								GlobalUserData.matchingTeamMode = MatchingTeamMode.Squad;
							}
						});
					}
				}


				public string ModeName { get; }


				private void OnPointerEnter(BaseEventData eventData)
				{
					MonoBehaviourInstance<Tooltip>.inst.SetLabel(strTierChange);
					MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, tooltipPos, Tooltip.Pivot.RightTop);
				}


				private void OnPointerExit(BaseEventData eventData)
				{
					MonoBehaviourInstance<Tooltip>.inst.Hide();
				}


				public void SetIsOn(bool isOn)
				{
					toggle.isOn = isOn;
				}


				public bool IsOn()
				{
					return toggle.isOn;
				}


				public void SetDisableActive(bool active)
				{
					disable.SetActive(active);
				}


				public bool GetDisableActive()
				{
					return !(disable == null) && disable.activeSelf;
				}


				public void SetTierChange(RankingTierChangeType rankingTierChangeType)
				{
					if (rankingTierChangeType == RankingTierChangeType.Promotion)
					{
						tierChange.gameObject.SetActive(true);
						tierChange.sprite =
							SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Tier_Promotion");
						strTierChange = Ln.Get("승급 안내 메시지");
						return;
					}

					if (rankingTierChangeType == RankingTierChangeType.Degrade)
					{
						tierChange.gameObject.SetActive(true);
						tierChange.sprite =
							SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Tier_Demotion");
						strTierChange = Ln.Get("강등 안내 메시지");
						return;
					}

					tierChange.gameObject.SetActive(false);
				}
			}
		}
	}
}