using System;
using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UICharacterScoreCardResult : UICharacterScoreCard
	{
		public delegate void OnPointerEnterMastery(Dictionary<MasteryType, int> masterys, Vector3 position);


		public delegate void OnPointerExitMastery();


		private static readonly Color32 TEXT_COLOR_ME = new Color32(222, 138, 46, byte.MaxValue);


		private static readonly Color32 TEXT_COLOR_ALIVE = new Color32(152, 173, 170, byte.MaxValue);


		private static readonly Color32 TEXT_COLOR_DEAD = new Color32(127, 70, 50, byte.MaxValue);


		private static readonly Color32 ICON_DEFAULT = new Color32(byte.MaxValue, 36, 88, byte.MaxValue);


		private static readonly Color32 ICON_REPORTED = new Color32(88, 88, 88, byte.MaxValue);


		private static readonly Color32 TEXT_DEFAULT = Color.white;


		private static readonly Color32 TEXT_REPORTED = new Color32(88, 88, 88, byte.MaxValue);


		[SerializeField] private Image upperSlot = default;


		[SerializeField] private GameObject rank1 = default;


		[SerializeField] private GameObject rank2 = default;


		[SerializeField] private GameObject rank3 = default;


		[SerializeField] private Text txtRank = default;


		private readonly Dictionary<MasteryType, int> combatLevel =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private readonly Dictionary<MasteryType, int> growthLevel =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private readonly Dictionary<MasteryType, int> searchLevel =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		public OnPointerEnterMastery EnterMasterys;


		public OnPointerExitMastery ExitMasterys;


		private bool isReported;


		private BaseControl masteryCombat;


		private BaseControl masteryGrowth;


		private BaseControl masterySearch;


		public Action<UICharacterScoreCardResult, int, int> OnClickReportBtn;


		public Action<UICharacterScoreCardResult> OnClickReportEventBox;


		public Action<UICharacterScoreCardResult> OnEnterReportEventBox;


		public Action<UICharacterScoreCardResult> OnExitReportBtn;


		public Action OnFinishReport;


		private PlayerInfo playerInfo;


		private BaseControl reportButton;


		private BaseControl reportEventBox;


		private Image reportIcon;


		private Text reportText;


		private int slotIndex;


		private LnText tempNickname;


		public bool IsReported => isReported;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			scrollRect = GameUtil.Bind<ScrollRect>(MonoBehaviourInstance<LobbyUI>.inst.gameObject,
				"LobbyWindow/ResultScoreboardWindow/Contents/ItemScrollView");
			tempNickname = GameUtil.Bind<LnText>(gameObject, "Name/TempNickname");
			masteryCombat = GameUtil.Bind<BaseControl>(gameObject, "Contents/Mastery_Combat");
			masteryCombat.OnPointerEnterEvent += delegate
			{
				OnPointerEnterMasterys(combatLevel, combatMasteryLevel.transform.position);
			};
			masteryCombat.OnPointerExitEvent += delegate { OnPointerExit(); };
			masteryCombat.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnBeginDrag(eventData);
			};
			masteryCombat.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnEndDrag(eventData);
			};
			masteryCombat.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnDrag(eventData);
			};
			masteryCombat.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnScroll(eventData);
			};
			masterySearch = GameUtil.Bind<BaseControl>(gameObject, "Contents/Mastery_Search");
			masterySearch.OnPointerEnterEvent += delegate
			{
				OnPointerEnterMasterys(searchLevel, searchMasteryLevel.transform.position);
			};
			masterySearch.OnPointerExitEvent += delegate { OnPointerExit(); };
			masterySearch.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnBeginDrag(eventData);
			};
			masterySearch.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnEndDrag(eventData);
			};
			masterySearch.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnDrag(eventData);
			};
			masterySearch.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnScroll(eventData);
			};
			masteryGrowth = GameUtil.Bind<BaseControl>(gameObject, "Contents/Mastery_Growth");
			masteryGrowth.OnPointerEnterEvent += delegate
			{
				OnPointerEnterMasterys(growthLevel, growthMasteryLevel.transform.position);
			};
			masteryGrowth.OnPointerExitEvent += delegate { OnPointerExit(); };
			masteryGrowth.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnBeginDrag(eventData);
			};
			masteryGrowth.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnEndDrag(eventData);
			};
			masteryGrowth.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnDrag(eventData);
			};
			masteryGrowth.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnScroll(eventData);
			};
			reportButton = GameUtil.Bind<BaseControl>(gameObject, "ReportButton");
			reportButton.OnPointerClickEvent += delegate
			{
				Action<UICharacterScoreCardResult, int, int> onClickReportBtn = OnClickReportBtn;
				if (onClickReportBtn == null)
				{
					return;
				}

				onClickReportBtn(this, slotIndex, playerInfo.teamNumber);
			};
			reportButton.OnPointerExitEvent += delegate
			{
				Action<UICharacterScoreCardResult> onExitReportBtn = OnExitReportBtn;
				if (onExitReportBtn == null)
				{
					return;
				}

				onExitReportBtn(this);
			};
			reportButton.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnBeginDrag(eventData);
			};
			reportButton.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnEndDrag(eventData);
			};
			reportButton.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnDrag(eventData);
			};
			reportButton.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnScroll(eventData);
			};
			reportEventBox = GameUtil.Bind<BaseControl>(gameObject, "ReportEventBox");
			reportEventBox.OnPointerEnterEvent += delegate
			{
				Action<UICharacterScoreCardResult> onEnterReportEventBox = OnEnterReportEventBox;
				if (onEnterReportEventBox == null)
				{
					return;
				}

				onEnterReportEventBox(this);
			};
			reportEventBox.OnPointerClickEvent += delegate
			{
				Action<UICharacterScoreCardResult> onClickReportEventBox = OnClickReportEventBox;
				if (onClickReportEventBox == null)
				{
					return;
				}

				onClickReportEventBox(this);
			};
			reportEventBox.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnBeginDrag(eventData);
			};
			reportEventBox.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnEndDrag(eventData);
			};
			reportEventBox.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnDrag(eventData);
			};
			reportEventBox.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnScroll(eventData);
			};
			reportIcon = GameUtil.Bind<Image>(reportButton.gameObject, "MenuList/Btn_Report/Ico_Report");
			reportText = GameUtil.Bind<Text>(reportButton.gameObject, "MenuList/Btn_Report/Txt_Report");
		}


		public new void Show()
		{
			gameObject.SetActive(true);
		}


		public new void Hide()
		{
			gameObject.SetActive(false);
		}


		public void SetSlotIndex(int index)
		{
			slotIndex = index;
		}


		public void SetPlayerInfo(PlayerInfo playerInfo)
		{
			this.playerInfo = playerInfo;
		}


		public void SetTempNickname(string tempName)
		{
			if (string.IsNullOrEmpty(tempName))
			{
				tempNickname.gameObject.SetActive(false);
				return;
			}

			tempNickname.text = "[" + tempName + "]";
			tempNickname.gameObject.SetActive(true);
		}


		public void SetMasteryLevels(PlayerInfo playerInfo)
		{
			if (playerInfo.masterysLevel == null)
			{
				return;
			}

			foreach (KeyValuePair<MasteryType, int> keyValuePair in playerInfo.masterysLevel)
			{
				if (keyValuePair.Key.GetCategory() == MasteryCategory.Combat)
				{
					combatLevel.Add(keyValuePair.Key, keyValuePair.Value);
				}
				else if (keyValuePair.Key.GetCategory() == MasteryCategory.Growth)
				{
					growthLevel.Add(keyValuePair.Key, keyValuePair.Value);
				}
				else if (keyValuePair.Key.GetCategory() == MasteryCategory.Search)
				{
					searchLevel.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}


		public void SetSlotBG(bool isMe, bool isAlive)
		{
			if (isMe)
			{
				MySlot.enabled = true;
				upperSlot.enabled = false;
				DeadMark.enabled = false;
				return;
			}

			MySlot.enabled = false;
			upperSlot.enabled = true;
			DeadMark.enabled = !isAlive;
			if (isAlive)
			{
				upperSlot.color = new Color(0.5888f, 0.941f, 1f, 0.666f);
				return;
			}

			upperSlot.color = new Color(1f, 0.682f, 0.682f, 0.392f);
		}


		public void SetTextColor(bool isMe, bool isAlive)
		{
			if (isMe)
			{
				nickname.color = TEXT_COLOR_ME;
				combatMasteryLevel.color = TEXT_COLOR_ME;
				searchMasteryLevel.color = TEXT_COLOR_ME;
				growthMasteryLevel.color = TEXT_COLOR_ME;
				playerKillCount.color = TEXT_COLOR_ME;
				playerKillAssistCount.color = TEXT_COLOR_ME;
				monsterKillCount.color = TEXT_COLOR_ME;
				return;
			}

			nickname.color = isAlive ? TEXT_COLOR_ALIVE : TEXT_COLOR_DEAD;
		}


		public void SetRank(int rank)
		{
			switch (rank)
			{
				case 1:
					rank1.SetActive(true);
					rank2.SetActive(false);
					rank3.SetActive(false);
					txtRank.gameObject.SetActive(false);
					return;
				case 2:
					rank1.SetActive(false);
					rank2.SetActive(true);
					rank3.SetActive(false);
					txtRank.gameObject.SetActive(false);
					return;
				case 3:
					rank1.SetActive(false);
					rank2.SetActive(false);
					rank3.SetActive(true);
					txtRank.gameObject.SetActive(false);
					return;
				default:
				{
					rank1.SetActive(false);
					rank2.SetActive(false);
					rank3.SetActive(false);
					txtRank.gameObject.SetActive(true);
					string text = rank == -1 ? "-" : rank.ToString();
					txtRank.text = text;
					return;
				}
			}
		}


		private void OnPointerEnterMasterys(Dictionary<MasteryType, int> masteryLevel, Vector3 pos)
		{
			pos.x += GameUtil.ConvertPositionOnScreenResolution(25f, 0f).x;
			EnterMasterys(masteryLevel, pos);
		}


		public void OnPointerExit()
		{
			ExitMasterys();
		}


		public void SetReportUI(bool active)
		{
			if (MySlot.enabled)
			{
				return;
			}

			if (!isReported)
			{
				reportIcon.color = ICON_DEFAULT;
				reportText.color = TEXT_DEFAULT;
				reportButton.enabled = true;
			}
			else
			{
				reportIcon.color = ICON_REPORTED;
				reportText.color = TEXT_REPORTED;
				reportButton.enabled = false;
			}

			reportEventBox.enabled = !active;
			reportButton.gameObject.SetActive(active);
		}


		public void ClickReasonButton(ReportType reportType)
		{
			if (playerInfo.userId <= 0L || playerInfo.nicknamePair == null)
			{
				this.StartThrowingCoroutine(CoroutineUtil.DelayedAction(0.1f, ShowCompletePopup),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][ReportUser] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
				return;
			}

			RequestDelegate.request<NullResponse>(
				LobbyApi.PlayerReport(new ReportParam(reportType, playerInfo.userId, playerInfo.nicknamePair.original,
					GlobalUserData.lastGameId)), delegate { ShowCompletePopup(); });
		}


		private void ShowCompletePopup()
		{
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("접수되었습니다."), new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("확인"),
				callback = delegate
				{
					reportIcon.color = ICON_REPORTED;
					reportText.color = TEXT_REPORTED;
					reportButton.enabled = false;
					isReported = true;
					Action onFinishReport = OnFinishReport;
					if (onFinishReport == null)
					{
						return;
					}

					onFinishReport();
				}
			});
		}


		public void EnterReportEventBox()
		{
			SetReportUI(false);
		}


		public void ReportCountIsMax()
		{
			isReported = true;
		}
	}
}