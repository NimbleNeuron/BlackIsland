using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class InfoLegauePage : BasePage
	{
		[SerializeField] private GameObject leagueUserListSlotPrefab = default;


		private BattleUser battleUser = default;


		private Button btnTierInfo = default;


		private MatchingTeamMode currentTeamMode = MatchingTeamMode.Solo;


		private RankingTierGrade currentTierGrade = default;


		private Image imgTierGrade = default;


		private Image imgTierType = default;


		private GameObject noUser = default;


		private ScrollRect scrollRect = default;


		private Dropdown teamModeList = default;


		private ToggleGroup tierGradeGroup = default;


		private Toggle[] tierGrades = default;


		private Text[] txtTierGrades = default;


		private Text txtTierLP = default;


		private Text txtTierName = default;


		public MatchingTeamMode CurrentTeamMode => currentTeamMode;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			imgTierType = GameUtil.Bind<Image>(gameObject, "Tier/RankTierSlot");
			imgTierGrade = GameUtil.Bind<Image>(gameObject, "Tier/RankTierSlot/Tier_Number");
			txtTierName = GameUtil.Bind<Text>(gameObject, "Tier/Txt_Tier/Txt_TierName");
			txtTierLP = GameUtil.Bind<Text>(gameObject, "Tier/Txt_Tier/Txt_Lp");
			btnTierInfo = GameUtil.Bind<Button>(gameObject, "Tier/Btn_TierInfo");
			teamModeList = GameUtil.Bind<Dropdown>(gameObject, "UserList/Title/Dropdown");
			tierGradeGroup = GameUtil.Bind<ToggleGroup>(gameObject, "UserList/Tab");
			tierGrades = tierGradeGroup.GetComponentsInChildren<Toggle>(true);
			txtTierGrades = tierGradeGroup.GetComponentsInChildren<Text>(true);
			scrollRect = GameUtil.Bind<ScrollRect>(gameObject, "UserList/Cotent/Scroll Rect");
			noUser = transform.Find("UserList/Cotent/NoUser").gameObject;
			teamModeList.onValueChanged.AddListener(OnChangeTeamMode);
			btnTierInfo.onClick.AddListener(OnClickTierInfo);
			for (int i = 0; i < tierGrades.Length; i++)
			{
				RankingTierGrade tierGrade = i + RankingTierGrade.One;
				tierGrades[i].isOn = false;
				tierGrades[i].onValueChanged.AddListener(delegate(bool isOn) { OnChangeTierGrade(isOn, tierGrade); });
			}
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			SetBattleUser();
			SetModeList();
			SetTierInfo();
			SetUserList();
		}


		private void SetBattleUser()
		{
			battleUser = InfoService.GetLeagueBattleUser();
		}


		private void SetModeList()
		{
			if (teamModeList.options.Count == 0)
			{
				List<string> options = new List<string>
				{
					Ln.Get("솔로"),
					Ln.Get("듀오"),
					Ln.Get("스쿼드")
				};
				teamModeList.ClearOptions();
				teamModeList.AddOptions(options);
			}
		}


		private void SetTierInfo()
		{
			if (battleUser == null)
			{
				imgTierGrade.gameObject.SetActive(false);
				imgTierType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Unrank);
				txtTierName.text = string.Format(RankingTierType.Unrank.GetName() ?? "", Array.Empty<object>());
				txtTierLP.text = "-";
				tierGradeGroup.gameObject.SetActive(false);
				return;
			}

			if (battleUser.batchMode)
			{
				imgTierGrade.gameObject.SetActive(false);
				imgTierType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Unrank);
				txtTierName.text = string.Format(RankingTierType.Unrank.GetName() ?? "", Array.Empty<object>());
				txtTierLP.text = "-";
				tierGradeGroup.gameObject.SetActive(false);
				return;
			}

			currentTierGrade = battleUser.tierGrade;
			imgTierGrade.gameObject.SetActive(true);
			imgTierType.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(battleUser.tierType);
			imgTierGrade.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierGradeSprite(battleUser.tierGrade);
			imgTierGrade.color = battleUser.tierType.GetColor();
			txtTierName.text = string.Format(battleUser.tierType.GetName() + " " + currentTierGrade.GetName(),
				Array.Empty<object>());
			txtTierLP.text =
				string.Format(
					string.Format("LP {0}",
						battleUser.mmr - InfoService.GetRankingMinMMR(battleUser.tierType, battleUser.tierGrade)),
					Array.Empty<object>());
			tierGradeGroup.gameObject.SetActive(true);
			tierGrades[currentTierGrade - RankingTierGrade.One].isOn = true;
			List<RankingTierGrade> leaguePlayerListKeys = InfoService.GetLeaguePlayerListKeys();
			for (int i = 0; i < txtTierGrades.Length; i++)
			{
				if (i < leaguePlayerListKeys.Count)
				{
					tierGrades[i].gameObject.SetActive(true);
					txtTierGrades[i].text =
						string.Format(battleUser.tierType.GetName() + " " + leaguePlayerListKeys[i].GetName(),
							Array.Empty<object>());
				}
				else
				{
					tierGrades[i].gameObject.SetActive(false);
					txtTierGrades[i].text = "";
				}
			}
		}


		private void SetUserList()
		{
			if (battleUser == null)
			{
				scrollRect.gameObject.SetActive(false);
				noUser.SetActive(true);
				return;
			}

			if (battleUser.batchMode)
			{
				scrollRect.gameObject.SetActive(false);
				noUser.SetActive(true);
				return;
			}

			if (InfoService.IsLeaguePlayerList())
			{
				int leaguePlayerListCount = InfoService.GetLeaguePlayerListCount(currentTierGrade);
				scrollRect.gameObject.SetActive(true);
				noUser.SetActive(leaguePlayerListCount == 0);
				if (scrollRect.content.childCount < leaguePlayerListCount)
				{
					int num = leaguePlayerListCount - scrollRect.content.childCount;
					for (int i = 0; i < num; i++)
					{
						Instantiate<GameObject>(leagueUserListSlotPrefab, scrollRect.content);
					}
				}

				int index = 0;
				LeagueUserListSlot[] componentsInChildren =
					scrollRect.content.GetComponentsInChildren<LeagueUserListSlot>(true);
				for (int j = 0; j < scrollRect.content.childCount; j++)
				{
					if (j < leaguePlayerListCount)
					{
						componentsInChildren[j].gameObject.SetActive(true);
						RankingUser rankingUser = InfoService.GetRankingUser(currentTierGrade, j);
						if (rankingUser.nickname == Lobby.inst.User.Nickname)
						{
							componentsInChildren[j].SetMyInfo(rankingUser.nickname,
								rankingUser.mmr - InfoService.GetRankingMinMMR(battleUser.tierType, currentTierGrade));
							index = j;
						}
						else
						{
							componentsInChildren[j].SetUserInfo(rankingUser.nickname,
								rankingUser.mmr - InfoService.GetRankingMinMMR(battleUser.tierType, currentTierGrade));
						}
					}
					else
					{
						componentsInChildren[j].gameObject.SetActive(false);
					}
				}

				scrollRect.content.localPosition = GetSnapToPositionToBringChildIntoView(scrollRect,
					scrollRect.content.GetChild(index).GetComponent<RectTransform>());
				return;
			}

			scrollRect.gameObject.SetActive(false);
			noUser.SetActive(true);
		}


		private void OnClickTierInfo()
		{
			MonoBehaviourInstance<LobbyUI>.inst.TierInfoWindow.Open();
		}


		private void OnChangeTeamMode(int value)
		{
			if (currentTeamMode != value + MatchingTeamMode.Solo)
			{
				currentTeamMode = value + MatchingTeamMode.Solo;
				InfoService.GetUserRankings(delegate
				{
					SetBattleUser();
					SetTierInfo();
					SetUserList();
				}, currentTeamMode);
			}
		}


		private void OnChangeTierGrade(bool isOn, RankingTierGrade tierGrade)
		{
			if (isOn && currentTierGrade != tierGrade)
			{
				currentTierGrade = tierGrade;
				SetUserList();
			}
		}


		private Vector2 GetSnapToPositionToBringChildIntoView(ScrollRect instance, RectTransform child)
		{
			Canvas.ForceUpdateCanvases();
			Vector2 vector = instance.viewport.localPosition;
			Vector2 vector2 = child.localPosition;
			return new Vector2(0f - (vector.x + vector2.x), 0f - (vector.y + vector2.y));
		}
	}
}