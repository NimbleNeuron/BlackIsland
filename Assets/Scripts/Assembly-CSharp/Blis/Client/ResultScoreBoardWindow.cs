using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ResultScoreBoardWindow : BaseWindow
	{
		public delegate void EventEnterReportEventBox();


		public delegate void EventReportCountIsMax();


		private const int REPORT_LIMIT = 2;


		[SerializeField] private UICharacterScoreCardResult playerScoreResultPrefab = default;


		[SerializeField] private TooltipMastery tooltipMastery = default;


		private readonly Image Mmr = default;


		private readonly Image Reward = default;


		private readonly List<TabIcon> tabList = new List<TabIcon>();


		private Image AP = default;


		private Button btnClose = default;


		private string customGameRoomKey;


		private EventEnterReportEventBox EnterReportEventBox;


		private Transform exportObject;


		private GameObject IconBattle = default;


		private GameObject IconItems = default;


		private GameObject IconMonsterKill = default;


		private GameObject IconPlayer = default;


		private GameObject IconPlayerKill = default;


		private GameObject IconPlayerKillAssist = default;


		private GameObject IconPowerUp = default;


		private GameObject IconRank = default;


		private GameObject IconSearch = default;


		private Image imgGaugeValue = default;


		private Transform imgKakaoPcBonus;


		private Image imgTierGrade = default;


		private Image imgTierType = default;


		private bool isReportNotApply;


		private Transform list = default;


		private Image LP = default;


		private List<PlayerInfo> playerInfos = new List<PlayerInfo>();


		private int reportCount;


		private EventReportCountIsMax ReportCountIsMax;


		private ReportReasonList reportReasonList;


		private string reportTargetNickname;


		private long reportTargetUserid;


		private UICharacterScoreCardResult[] scoreList = default;


		private ScrollRect scrollRect = default;


		private LnText txtDemotion = default;


		private LnText txtGainAP = default;


		private LnText txtGainLP = default;


		private LnText txtGainXP = default;


		private LnText txtGameMode = default;


		private LnText txtGameModeDetail = default;


		private LnText txtLvValue = default;


		private LnText txtMyLP = default;


		private LnText txtMyRanking = default;


		private LnText txtPlayTime = default;


		private LnText txtPromotion = default;


		private Image XP = default;

		protected override void OnAwakeUI()
		{
			FadeSpeed = 1000f;
			base.OnAwakeUI();
			LP = GameUtil.Bind<Image>(gameObject, "LeftInfo/List/LP");
			AP = GameUtil.Bind<Image>(gameObject, "LeftInfo/List/AP");
			XP = GameUtil.Bind<Image>(gameObject, "LeftInfo/List/XP");
			txtMyRanking = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/Rank/IMG_MyRanking/TXT_MyRanking");
			txtGameModeDetail = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/Rank/TXT_GameMode");
			txtPlayTime = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/Rank/TXT_PlayTime");
			txtMyLP = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/LP/TXT_LP/TXT_MyLP");
			txtGameMode = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/LP/TXT_LP/TXT_GameMode");
			imgTierType = GameUtil.Bind<Image>(gameObject, "LeftInfo/List/LP/RankTierSlot");
			imgTierGrade = GameUtil.Bind<Image>(gameObject, "LeftInfo/List/LP/RankTierSlot/Tier_Number");
			txtPromotion = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/LP/TXT_Promotion");
			txtDemotion = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/LP/TXT_Demotion");
			txtGainLP = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/LP/TXT_LP/TXT_GainLP");
			txtGainAP = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/AP/TXT_GainAP");
			txtLvValue = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/XP/TXT_Lv/TXT_LvValue");
			imgGaugeValue = GameUtil.Bind<Image>(gameObject, "LeftInfo/List/XP/IMG_GaugeValue");
			txtGainXP = GameUtil.Bind<LnText>(gameObject, "LeftInfo/List/XP/TXT_GainXP");
			IconRank = transform.FindRecursively("IconRank").gameObject;
			IconPlayer = transform.FindRecursively("IconPlayer").gameObject;
			IconItems = transform.FindRecursively("IconItems").gameObject;
			IconBattle = transform.FindRecursively("IconBattle").gameObject;
			IconSearch = transform.FindRecursively("IconSearch").gameObject;
			IconPowerUp = transform.FindRecursively("IconPowerUp").gameObject;
			IconPlayerKill = transform.FindRecursively("IconPlayerKill").gameObject;
			IconPlayerKillAssist = transform.FindRecursively("IconPlayerKillAssist").gameObject;
			IconMonsterKill = transform.FindRecursively("IconMonsterKill").gameObject;
			scrollRect = GameUtil.Bind<ScrollRect>(gameObject, "Contents/ItemScrollView");
			list = GameUtil.Bind<Transform>(gameObject, "Contents/ItemScrollView/Viewport/PlayerList");
			btnClose = GameUtil.Bind<Button>(gameObject, "BTN_Close");
			btnClose.onClick.AddListener(Close);
			scoreList = list.GetComponentsInChildren<UICharacterScoreCardResult>(true);
			reportReasonList = GameUtil.Bind<ReportReasonList>(gameObject, "ReportReasonList");
			exportObject = GameUtil.Bind<Transform>(gameObject, "LeftInfo/List/Btn_Download");
			imgKakaoPcBonus = GameUtil.Bind<Transform>(gameObject, "LeftInfo/List/AP/KakaoPcBonus");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			tabList.Add(new TabIcon(IconRank, Ln.Get("순위")));
			tabList.Add(new TabIcon(IconPlayer, Ln.Get("플레이어")));
			tabList.Add(new TabIcon(IconItems, Ln.Get("아이템")));
			tabList.Add(new TabIcon(IconBattle, Ln.Get("전투")));
			tabList.Add(new TabIcon(IconSearch, Ln.Get("탐색")));
			tabList.Add(new TabIcon(IconPowerUp, Ln.Get("성장")));
			tabList.Add(new TabIcon(IconPlayerKill, Ln.Get("플레이어 처치 수")));
			tabList.Add(new TabIcon(IconPlayerKillAssist, Ln.Get("어시스트 수")));
			tabList.Add(new TabIcon(IconMonsterKill, Ln.Get("야생동물 처치 수")));
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			playerInfos.Clear();
			playerInfos.AddRange(GlobalUserData.dicPlayerResults.Values.ToList<PlayerInfo>());
			customGameRoomKey = GlobalUserData.customGameRoomKey;
			if (GlobalUserData.matchingMode == MatchingMode.Rank)
			{
				InfoService.GetRankingSeasonTiers(OpenResult);
				return;
			}

			OpenResult();
		}


		protected override void OnClose()
		{
			FadeSpeed = 10f;
			base.OnClose();
		}


		private void OpenResult()
		{
			CalResultDatas();
			SetLeftInfo();
			SetResultScoreBoard();
			isReportNotApply = !GlobalUserData.IsPlayer || GlobalUserData.IsStandaloneMode() ||
			                   GlobalUserData.IsCustomMatching();
			if (string.IsNullOrEmpty(customGameRoomKey))
			{
				exportObject.gameObject.SetActive(false);
				return;
			}

			exportObject.gameObject.SetActive(true);
		}


		public void SetFocus(int index)
		{
			float num = 69f;
			float x = list.transform.localPosition.x;
			float num2 = list.transform.localPosition.y;
			if (index <= 5)
			{
				// num2 = num2;
			}
			else if (index == 6)
			{
				num2 += 85f;
			}
			else
			{
				num2 = num2 + 85f + (index - 6) * num;
			}

			list.transform.localPosition = new Vector3(x, num2, 0f);
		}


		private void CalResultDatas()
		{
			int myObjectId = GlobalUserData.myObjectId;
			if (GlobalUserData.IsTeamMode())
			{
				PlayerInfo me = playerInfos.Find(x => x.objectId == myObjectId);
				List<PlayerInfo> list = playerInfos.FindAll(x => x.teamNumber == me.teamNumber);
				List<PlayerInfo> list2 = playerInfos.FindAll(x => x.isAlive);
				bool flag = true;
				using (List<PlayerInfo>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.teamNumber != me.teamNumber)
						{
							flag = false;
							break;
						}
					}
				}

				if (flag)
				{
					foreach (PlayerInfo playerInfo in list)
					{
						PlayerInfo item = CopyPlayerInfo(playerInfo, 1);
						playerInfos.Add(item);
						playerInfos.Remove(playerInfo);
					}

					return;
				}

				if (me.rank != 2)
				{
					return;
				}

				using (List<PlayerInfo>.Enumerator enumerator = playerInfos.FindAll(x => x.rank == -1).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayerInfo playerInfo2 = enumerator.Current;
						PlayerInfo item2 = CopyPlayerInfo(playerInfo2, 1);
						playerInfos.Add(item2);
						playerInfos.Remove(playerInfo2);
					}

					return;
				}
			}

			PlayerInfo playerInfo3 = playerInfos.Find(x => x.objectId == myObjectId);
			if (playerInfo3.rank == -1)
			{
				PlayerInfo item3 = CopyPlayerInfo(playerInfo3, 1);
				playerInfos.Add(item3);
				playerInfos.Remove(playerInfo3);
				return;
			}

			if (playerInfo3.rank == 2)
			{
				if (playerInfos.Exists(x => x.rank == -1))
				{
					playerInfo3 = playerInfos.Find(x => x.rank == -1);
					PlayerInfo item4 = CopyPlayerInfo(playerInfo3, 1);
					playerInfos.Add(item4);
					playerInfos.Remove(playerInfo3);
				}
			}
		}


		private PlayerInfo CopyPlayerInfo(PlayerInfo playerInfo, int rank)
		{
			PlayerInfo result = default;
			result.objectId = playerInfo.objectId;
			result.rank = rank;
			result.isInSight = playerInfo.isInSight;
			result.name = playerInfo.name;
			result.characterCode = playerInfo.characterCode;
			List<Item> equipment = playerInfo.equipment;
			result.equipment = equipment != null ? equipment.ToList<Item>() : null;
			result.isAlive = playerInfo.isAlive;
			result.teamNumber = playerInfo.teamNumber;
			result.teamSlot = playerInfo.teamSlot;
			result.level = playerInfo.level;
			result.combatLevel = playerInfo.combatLevel;
			result.searchLevel = playerInfo.searchLevel;
			result.growthLevel = playerInfo.growthLevel;
			result.masterysLevel = playerInfo.masterysLevel;
			result.playerKill = playerInfo.playerKill;
			result.playerKillAssist = playerInfo.playerKillAssist;
			result.monsterKill = playerInfo.monsterKill;
			result.updateDtm = playerInfo.updateDtm;
			result.nicknamePair = playerInfo.nicknamePair;
			return result;
		}


		private void SetLeftInfo()
		{
			if (GlobalUserData.IsPlayer)
			{
				LP.transform.localScale = Vector3.one;
				AP.transform.localScale = Vector3.one;
				XP.transform.localScale = Vector3.one;
			}
			else
			{
				LP.transform.localScale = Vector3.zero;
				AP.transform.localScale = Vector3.zero;
				XP.transform.localScale = Vector3.zero;
			}

			int money = Lobby.inst.User.MMR;
			int gainMMR = Lobby.inst.User.GainMMR;
			int gainAP = Lobby.inst.User.GainAP;
			int gainXP = Lobby.inst.User.GainXP;
			txtMyLP.text = "";
			txtGainLP.text = "";
			txtGameMode.text = "";
			int rank = playerInfos.Find(x => x.objectId == GlobalUserData.myObjectId).rank;
			txtMyRanking.text = rank == -1 || !GlobalUserData.IsPlayer ? "-" : Ln.Format("{0}위", rank);
			if (GlobalUserData.matchingMode == MatchingMode.Rank)
			{
				money = Lobby.inst.User.MMR -
				        InfoService.GetRankingMinMMR(Lobby.inst.User.AfterTierType, Lobby.inst.User.AfterTierGrade);
				if (Lobby.inst.User.LastBatchMode)
				{
					txtMyLP.text = StringUtil.AssetToString(money);
					imgTierType.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(
							Lobby.inst.User.AfterTierType);
					imgTierGrade.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierGradeSprite(Lobby.inst.User
							.AfterTierGrade);
					imgTierGrade.color = Lobby.inst.User.AfterTierType.GetColor();
					imgTierGrade.gameObject.SetActive(true);
					txtPromotion.gameObject.SetActive(false);
					txtDemotion.gameObject.SetActive(false);
				}
				else if (Lobby.inst.User.BatchMode)
				{
					txtMyLP.text = Ln.Get("배치고사 중");
					imgTierType.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Unrank);
					imgTierGrade.gameObject.SetActive(false);
					txtPromotion.gameObject.SetActive(false);
					txtDemotion.gameObject.SetActive(false);
				}
				else
				{
					if (Lobby.inst.User.AfterTierChangeType == RankingTierChangeType.Promotion)
					{
						txtMyLP.text = "<color=#2C9EFF>" + Ln.Get("승급 안내") + "</color>";
					}
					else if (Lobby.inst.User.AfterTierChangeType == RankingTierChangeType.Degrade)
					{
						txtMyLP.text = "<color=#ED3434>" + Ln.Get("강등 안내") + "</color>";
					}
					else
					{
						money = Lobby.inst.User.MMR - InfoService.GetRankingMinMMR(Lobby.inst.User.AfterTierType,
							Lobby.inst.User.AfterTierGrade);
						txtMyLP.text = StringUtil.AssetToString(money);
					}

					if (gainMMR == 0)
					{
						txtGainLP.text = "<color=#FDD9A5>-</color>";
					}
					else if (gainMMR > 0)
					{
						txtGainLP.text = "<color=#4DE02F>+ " + StringUtil.AssetToString(gainMMR) + "</color>";
					}
					else
					{
						txtGainLP.text = "<color=#E5371F>" + StringUtil.AssetToString(gainMMR) + "</color>";
					}

					imgTierType.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(
							Lobby.inst.User.AfterTierType);
					imgTierGrade.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierGradeSprite(Lobby.inst.User
							.AfterTierGrade);
					imgTierGrade.color = Lobby.inst.User.AfterTierType.GetColor();
					imgTierGrade.gameObject.SetActive(true);
					txtPromotion.gameObject.SetActive(Lobby.inst.User.BeforeTierChangeType ==
					                                  RankingTierChangeType.Promotion);
					txtDemotion.gameObject.SetActive(Lobby.inst.User.BeforeTierChangeType ==
					                                 RankingTierChangeType.Degrade);
				}
			}
			else
			{
				txtGameMode.text = GlobalUserData.GetStringMatchingMode();
				imgTierType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Normal);
				imgTierGrade.gameObject.SetActive(false);
				txtPromotion.gameObject.SetActive(false);
				txtDemotion.gameObject.SetActive(false);
			}

			txtGainAP.text = "+ " + StringUtil.AssetToString(gainAP);
			txtGainXP.text = "+ " + StringUtil.AssetToString(gainXP);
			txtLvValue.text = Lobby.inst.User.Level.ToString();
			float num = GameDB.user.GetNeedXP(Lobby.inst.User.Level);
			float num2 = num - Lobby.inst.User.NeedXP;
			imgGaugeValue.fillAmount = num2 / num;
			if (imgKakaoPcBonus != null)
			{
				imgKakaoPcBonus.gameObject.SetActive(SingletonMonoBehaviour<KakaoPcService>.inst.BenefitByKakaoPcCafe &&
				                                     gainAP != 0);
			}

			SetTextMatchingMode();
			SetTextPlayTime();
		}


		private void SetResultScoreBoard()
		{
			UICharacterScoreCardResult[] array = scoreList;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Hide();
			}

			SortingPlayerResults();
			int focus = 0;
			int num = 0;
			while (num < scoreList.Length && playerInfos.Count != num)
			{
				PlayerInfo playerInfo = playerInfos[num];
				bool flag = GlobalUserData.myObjectId == playerInfo.objectId;
				bool isMyTeam = GlobalUserData.myTeamNumber == playerInfo.teamNumber;
				scoreList[num].SetPlayerInfo(playerInfo);
				scoreList[num].SetMasteryLevels(playerInfo);
				scoreList[num].SetRank(playerInfo.rank);
				scoreList[num]
					.SetCharacterImage(
						SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterScoreSprite(playerInfo.characterCode));
				scoreList[num].SetTeamColor(isMyTeam, playerInfo.teamSlot);
				scoreList[num].SetCharacterLevel(playerInfo.level);
				scoreList[num].SetEquips(playerInfo.equipment);
				scoreList[num].SetCombatMasteryLevel(playerInfo.combatLevel);
				scoreList[num].SetSearchMasteryLevel(playerInfo.searchLevel);
				scoreList[num].SetGrowthMasteryLevel(playerInfo.growthLevel);
				scoreList[num].SetPlayerKillCount(playerInfo.playerKill);
				scoreList[num].SetPlayerKillAssistCount(playerInfo.playerKillAssist);
				scoreList[num].SetMonsterKillCount(playerInfo.monsterKill);
				scoreList[num].SetSlotIndex(num);
				scoreList[num].SetScrollRect(scrollRect);
				scoreList[num].EnterMasterys = OnPointerEnterMasterys;
				scoreList[num].ExitMasterys = OnPointerExitMasterys;
				scoreList[num].OnEnterReportEventBox = OnEnterReportEventBox;
				scoreList[num].OnExitReportBtn = OnExitReportBtn;
				scoreList[num].OnClickReportEventBox = OnClickReportEventBox;
				scoreList[num].OnClickReportBtn = OnClickReportBtn;
				scoreList[num].OnFinishReport = IncreaseReportCount;
				EnterReportEventBox = (EventEnterReportEventBox) Delegate.Combine(EnterReportEventBox,
					new EventEnterReportEventBox(scoreList[num].EnterReportEventBox));
				ReportCountIsMax = (EventReportCountIsMax) Delegate.Combine(ReportCountIsMax,
					new EventReportCountIsMax(scoreList[num].ReportCountIsMax));
				scoreList[num].SetObjectId(playerInfo.objectId);
				scoreList[num].SetSlotBG(flag, playerInfo.isAlive);
				scoreList[num].SetName(playerInfo.nicknamePair != null
					? playerInfo.nicknamePair.original
					: playerInfo.name);
				scoreList[num].SetTempNickname(playerInfo.nicknamePair != null && playerInfo.isUseTempNickname
					? playerInfo.name
					: null);
				scoreList[num].SetTextColor(flag, playerInfo.isAlive);
				if (flag)
				{
					focus = num;
				}

				scoreList[num].Show();
				num++;
			}

			SetFocus(focus);
		}


		private void SortingPlayerResults()
		{
			if (GlobalUserData.matchingTeamMode == MatchingTeamMode.Duo ||
			    GlobalUserData.matchingTeamMode == MatchingTeamMode.Squad)
			{
				playerInfos = (from x in playerInfos
					orderby x.isAlive descending, x.rank, x.objectId == GlobalUserData.myObjectId descending
					select x).ToList<PlayerInfo>();
				return;
			}

			playerInfos.Sort((x, y) => x.rank.CompareTo(y.rank));
		}


		private void SetTextMatchingMode()
		{
			txtGameModeDetail.text = GlobalUserData.GetStringMatchingModeDetail();
		}


		private void SetTextPlayTime()
		{
			int myPlayTime = GlobalUserData.myPlayTime;
			int num = myPlayTime / 60;
			int num2 = myPlayTime % 60;
			txtPlayTime.text = string.Format("{0:0} : {1:00}", num, num2);
		}


		private void OnPointerEnterMasterys(Dictionary<MasteryType, int> masterys, Vector3 position)
		{
			tooltipMastery.ShowTooltip(masterys, position);
		}


		private void OnPointerExitMasterys()
		{
			tooltipMastery.HideTooltip();
		}


		private void OnEnterReportEventBox(UICharacterScoreCardResult enteredCard)
		{
			if (isReportNotApply || reportReasonList.IsShow())
			{
				return;
			}

			EnterReportEventBox();
			enteredCard.SetReportUI(true);
			reportReasonList.ClickReasonButton = enteredCard.ClickReasonButton;
		}


		private void OnExitReportBtn(UICharacterScoreCardResult enteredCard)
		{
			if (reportReasonList.IsShow())
			{
				return;
			}

			enteredCard.SetReportUI(false);
		}


		private void OnClickReportEventBox(UICharacterScoreCardResult enteredCard)
		{
			if (isReportNotApply)
			{
				return;
			}

			EnterReportEventBox();
			enteredCard.SetReportUI(true);
			reportReasonList.ClickReasonButton = enteredCard.ClickReasonButton;
		}


		private void OnClickReportBtn(UICharacterScoreCardResult enteredCard, int slotIndex, int teamNumber)
		{
			if (isReportNotApply)
			{
				reportReasonList.Hide();
			}

			if (reportCount < 2 && !reportReasonList.IsShow())
			{
				reportReasonList.Show(slotIndex, GlobalUserData.myTeamNumber == teamNumber);
			}
		}


		private void IncreaseReportCount()
		{
			reportCount++;
			if (reportCount >= 2)
			{
				ReportCountIsMax();
			}
		}


		public void ExportResult()
		{
			using (StreamWriter streamWriter = new StreamWriter("GameResult_" + customGameRoomKey + ".csv", false,
				Encoding.GetEncoding("utf-8")))
			{
				streamWriter.WriteLine(
					"rank, level, character, nickname, weapon, best weapon mastery, best weapon mastery level, kill, assist, hunting");
				foreach (PlayerInfo playerInfo in playerInfos)
				{
					string text = string.Empty;
					foreach (Item item in playerInfo.equipment)
					{
						if (item.ItemData.itemType == ItemType.Weapon)
						{
							text = item.ItemData.name;
						}
					}

					int num = int.MinValue;
					string text2 = string.Empty;
					foreach (KeyValuePair<MasteryType, int> keyValuePair in playerInfo.masterysLevel)
					{
						if (keyValuePair.Key.GetCategory() == MasteryCategory.Combat && num < keyValuePair.Value)
						{
							text2 = keyValuePair.Key.ToString();
							num = keyValuePair.Value;
						}
					}

					CharacterData characterData = GameDB.character.GetCharacterData(playerInfo.characterCode);
					streamWriter.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", playerInfo.rank,
						playerInfo.level, characterData.name, playerInfo.nicknamePair.original, text, text2,
						playerInfo.combatLevel, playerInfo.playerKill, playerInfo.playerKillAssist,
						playerInfo.monsterKill);
				}
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("CSV파일다운로드완료팝업"), new Popup.Button
			{
				text = Ln.Get("확인")
			});
		}

		private void Ref()
		{
			Reference.Use(playerScoreResultPrefab);
			Reference.Use(Mmr);
			Reference.Use(Reward);
		}


		private class TabIcon
		{
			private readonly EventTrigger eventTrigger;


			private readonly GameObject gameObject;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			private readonly string str;


			private readonly Vector2 tooltipPos;

			public TabIcon(GameObject gameObject, string str)
			{
				this.gameObject = gameObject;
				this.str = str;
				tooltipPos = gameObject.transform.position;
				tooltipPos += GameUtil.ConvertPositionOnScreenResolution(0f, 60f);
				GameUtil.BindOrAdd<EventTrigger>(this.gameObject, ref eventTrigger);
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
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(str);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, tooltipPos, Tooltip.Pivot.LeftTop);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}
	}
}