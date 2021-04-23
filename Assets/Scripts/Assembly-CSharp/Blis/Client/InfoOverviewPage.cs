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
	public class InfoOverviewPage : BasePage
	{
		private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


		private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();
		private BattleUser battleUser;


		private int currentSeasonIndex;


		private MatchingTeamMode currentTeamMode = MatchingTeamMode.Solo;


		private EventTrigger eventTrigger;


		private Image imgTierGrade;


		private Image imgTierType;


		private Dropdown infoSeasonList;


		private Transform mostCharactersGroup;


		private MostCharacterSlot[] mostCharacterSlots;


		private Toggle toggleDuo;


		private Toggle toggleSolo;


		private Toggle toggleSquad;


		private GameObject tooltipTime;


		private GameObject top1;


		private GameObject top2;


		private GameObject top3;


		private GameObject top5;


		private GameObject top7;


		private Text txtAssist;


		private Text txtAvgRank;


		private Text txtGames;


		private Text txtHunt;


		private Text txtKill;


		private Text txtRemainTime;


		private Text txtTierLP;


		private Text txtTierName;


		private Text txtTop1;


		private Text txtTop2;


		private Text txtTop3;


		private Text txtTop5;


		private Text txtTop7;


		private Text txtWin;


		public int CurrentSeasonIndex => currentSeasonIndex;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			currentSeasonIndex = 0;
			infoSeasonList = GameUtil.Bind<Dropdown>(gameObject, "Dropdown");
			imgTierType = GameUtil.Bind<Image>(gameObject, "Tier/RankTierSlot");
			imgTierGrade = GameUtil.Bind<Image>(gameObject, "Tier/RankTierSlot/Tier_Number");
			txtTierName = GameUtil.Bind<Text>(gameObject, "Tier/Txt_Tier/Txt_TierName");
			txtTierLP = GameUtil.Bind<Text>(gameObject, "Tier/Txt_Tier/Txt_Lp");
			toggleSolo = GameUtil.Bind<Toggle>(gameObject, "Statistics/Tab/Tab01");
			toggleDuo = GameUtil.Bind<Toggle>(gameObject, "Statistics/Tab/Tab02");
			toggleSquad = GameUtil.Bind<Toggle>(gameObject, "Statistics/Tab/Tab03");
			txtRemainTime = GameUtil.Bind<Text>(gameObject, "Statistics/Title/Time/Txt");
			tooltipTime = txtRemainTime.transform.FindRecursively("TooltipTime").gameObject;
			txtAvgRank = GameUtil.Bind<Text>(gameObject, "Statistics/Cotent/Top/AvgRank/Count");
			txtGames = GameUtil.Bind<Text>(gameObject, "Statistics/Cotent/Top/Play/Games/Count");
			txtWin = GameUtil.Bind<Text>(gameObject, "Statistics/Cotent/Top/Play/Win/Count");
			txtKill = GameUtil.Bind<Text>(gameObject, "Statistics/Cotent/Middle/Average/List/Kill/Count");
			txtAssist = GameUtil.Bind<Text>(gameObject, "Statistics/Cotent/Middle/Average/List/Assist/Count");
			txtHunt = GameUtil.Bind<Text>(gameObject, "Statistics/Cotent/Middle/Average/List/Hunt/Count");
			top1 = transform.Find("Statistics/Cotent/Bottom/TopFinishes/List/Top1").gameObject;
			top2 = transform.Find("Statistics/Cotent/Bottom/TopFinishes/List/Top2").gameObject;
			top3 = transform.Find("Statistics/Cotent/Bottom/TopFinishes/List/Top3").gameObject;
			top5 = transform.Find("Statistics/Cotent/Bottom/TopFinishes/List/Top5").gameObject;
			top7 = transform.Find("Statistics/Cotent/Bottom/TopFinishes/List/Top7").gameObject;
			txtTop1 = GameUtil.Bind<Text>(top1, "Count");
			txtTop2 = GameUtil.Bind<Text>(top2, "Count");
			txtTop3 = GameUtil.Bind<Text>(top3, "Count");
			txtTop5 = GameUtil.Bind<Text>(top5, "Count");
			txtTop7 = GameUtil.Bind<Text>(top7, "Count");
			mostCharactersGroup = GameUtil.Bind<Transform>(gameObject, "MostChr/Cotent");
			mostCharacterSlots = mostCharactersGroup.GetComponentsInChildren<MostCharacterSlot>();
			infoSeasonList.onValueChanged.AddListener(OnChangeSeason);
			toggleSolo.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnChangeTeamMode(isOn, MatchingTeamMode.Solo);
			});
			toggleDuo.onValueChanged.AddListener(delegate(bool isOn) { OnChangeTeamMode(isOn, MatchingTeamMode.Duo); });
			toggleSquad.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnChangeTeamMode(isOn, MatchingTeamMode.Squad);
			});
			GameUtil.BindOrAdd<EventTrigger>(tooltipTime, ref eventTrigger);
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


		private void OnPointerEnter(BaseEventData eventData)
		{
			Vector2 vector = tooltipTime.transform.position;
			vector += GameUtil.ConvertPositionOnScreenResolution(0f, 60f);
			MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get("시즌 종료까지 남은 시간"));
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
		}


		private void OnPointerExit(BaseEventData eventData)
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			SetBattleUser();
			SetSeasonList();
			SetTierInfo();
			SetStatisticsInfo();
			SetUseTopCharacters();
		}


		private void SetBattleUser()
		{
			battleUser = InfoService.GetOverviewBattleUser(currentTeamMode);
		}


		private void SetSeasonList()
		{
			if (infoSeasonList.options.Count == 0)
			{
				infoSeasonList.ClearOptions();
				infoSeasonList.AddOptions(InfoService.GetRankingSeasonList());
			}
		}


		private void SetTierInfo()
		{
			if (battleUser != null)
			{
				if (battleUser.seasonId == 0)
				{
					imgTierGrade.gameObject.SetActive(false);
					imgTierType.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Normal);
					txtTierName.text = string.Format(RankingTierType.Normal.GetName() ?? "", Array.Empty<object>());
					txtTierLP.text = "";
					return;
				}

				if (battleUser.batchMode)
				{
					imgTierGrade.gameObject.SetActive(false);
					imgTierType.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Unrank);
					txtTierName.text = string.Format(RankingTierType.Unrank.GetName() ?? "", Array.Empty<object>());
					txtTierLP.text = "-";
					return;
				}

				imgTierGrade.gameObject.SetActive(true);
				imgTierType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(battleUser.tierType);
				imgTierGrade.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierGradeSprite(battleUser.tierGrade);
				imgTierGrade.color = battleUser.tierType.GetColor();
				txtTierName.text = string.Format(battleUser.tierType.GetName() + " " + battleUser.tierGrade.GetName(),
					Array.Empty<object>());
				txtTierLP.text =
					string.Format(
						string.Format("LP {0}",
							battleUser.mmr - InfoService.GetRankingMinMMR(battleUser.tierType, battleUser.tierGrade)),
						Array.Empty<object>());
			}
			else
			{
				if (currentSeasonIndex > 0)
				{
					imgTierGrade.gameObject.SetActive(false);
					imgTierType.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Unrank);
					txtTierName.text = string.Format(RankingTierType.Unrank.GetName() ?? "", Array.Empty<object>());
					txtTierLP.text = "-";
					return;
				}

				imgTierGrade.gameObject.SetActive(false);
				imgTierType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Normal);
				txtTierName.text = string.Format(RankingTierType.Normal.GetName() ?? "", Array.Empty<object>());
				txtTierLP.text = "";
			}
		}


		private void SetStatisticsInfo()
		{
			if (currentTeamMode == MatchingTeamMode.Solo)
			{
				top1.SetActive(false);
				top2.SetActive(false);
				top3.SetActive(true);
				top5.SetActive(true);
				top7.SetActive(true);
			}
			else
			{
				top1.SetActive(true);
				top2.SetActive(true);
				top3.SetActive(true);
				top5.SetActive(false);
				top7.SetActive(false);
			}

			txtRemainTime.gameObject.SetActive(false);
			if (battleUser != null && battleUser.battleUserStat != null)
			{
				if (battleUser.matchingMode == MatchingMode.Rank)
				{
					string remainRankingSeasonTime =
						InfoService.GetRemainRankingSeasonTime(battleUser.battleUserStat.seasonId);
					if (!string.IsNullOrEmpty(remainRankingSeasonTime))
					{
						txtRemainTime.gameObject.SetActive(true);
						txtRemainTime.text = remainRankingSeasonTime;
					}
				}

				txtAvgRank.text = battleUser.battleUserStat.averageRank.ToString();
				txtGames.text = battleUser.battleUserStat.totalGames.ToString();
				txtWin.text = battleUser.battleUserStat.totalWins.ToString();
				txtKill.text = battleUser.battleUserStat.averageKills.ToString();
				txtAssist.text = battleUser.battleUserStat.averageAssistants.ToString();
				txtHunt.text = battleUser.battleUserStat.averageHunts.ToString();
				txtTop1.text =
					string.Format(
						string.Format("{0} ({1})%", battleUser.battleUserStat.totalTop1s,
							battleUser.battleUserStat.top1 * 100f), Array.Empty<object>());
				txtTop2.text =
					string.Format(
						string.Format("{0} ({1})%", battleUser.battleUserStat.totalTop2s,
							battleUser.battleUserStat.top2 * 100f), Array.Empty<object>());
				txtTop3.text =
					string.Format(
						string.Format("{0} ({1})%", battleUser.battleUserStat.totalTop3s,
							battleUser.battleUserStat.top3 * 100f), Array.Empty<object>());
				txtTop5.text =
					string.Format(
						string.Format("{0} ({1})%", battleUser.battleUserStat.totalTop5s,
							battleUser.battleUserStat.top5 * 100f), Array.Empty<object>());
				txtTop7.text =
					string.Format(
						string.Format("{0} ({1})%", battleUser.battleUserStat.totalTop7s,
							battleUser.battleUserStat.top7 * 100f), Array.Empty<object>());
				return;
			}

			txtAvgRank.text = "-";
			txtGames.text = "-";
			txtWin.text = "-";
			txtKill.text = "-";
			txtAssist.text = "-";
			txtHunt.text = "-";
			txtTop1.text = "-";
			txtTop2.text = "-";
			txtTop3.text = "-";
			txtTop5.text = "-";
			txtTop7.text = "-";
		}


		private void SetUseTopCharacters()
		{
			if (battleUser != null && battleUser.battleUserStat != null)
			{
				List<BattleCharacterStat> list = (from x in battleUser.battleUserStat.characterStats
					orderby x.usages descending, x.characterCode
					select x).ToList<BattleCharacterStat>();
				for (int i = 0; i < mostCharacterSlots.Length; i++)
				{
					if (i < battleUser.battleUserStat.characterStats.Count)
					{
						mostCharacterSlots[i].SetCharacter(list[i].characterCode);
						mostCharacterSlots[i].SetUseCount((int) list[i].usages);
						mostCharacterSlots[i].SetKillCount(list[i].maxKillings);
						mostCharacterSlots[i].SetTopCount(list[i].top3, list[i].top3Rate * 100f);
					}
					else
					{
						mostCharacterSlots[i].SetNoData();
					}
				}

				return;
			}

			for (int j = 0; j < mostCharacterSlots.Length; j++)
			{
				mostCharacterSlots[j].SetNoData();
			}
		}


		private void OnChangeTeamMode(bool isOn, MatchingTeamMode matchingTeamMode)
		{
			if (isOn)
			{
				currentTeamMode = matchingTeamMode;
				SetBattleUser();
				SetTierInfo();
				SetStatisticsInfo();
				SetUseTopCharacters();
			}
		}


		private void OnChangeSeason(int value)
		{
			if (currentSeasonIndex != value)
			{
				currentSeasonIndex = value;
				InfoService.GetBattleOverview(delegate
				{
					if (currentTeamMode == MatchingTeamMode.Solo)
					{
						SetBattleUser();
						SetTierInfo();
						SetStatisticsInfo();
						SetUseTopCharacters();
						return;
					}

					toggleSolo.isOn = true;
				}, Lobby.inst.User.UserNum, InfoService.GetRankingSeasonId(currentSeasonIndex));
			}
		}


		public void ResetDropBox()
		{
			infoSeasonList.Hide();
		}
	}
}