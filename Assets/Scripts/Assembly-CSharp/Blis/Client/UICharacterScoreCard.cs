using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UICharacterScoreCard : BaseUI
	{
		private static readonly Color32 DEAD_COLOR = new Color32(195, 139, 141, byte.MaxValue);


		[SerializeField] private Image mySlot = default;


		[SerializeField] private Transform contents = default;


		[SerializeField] private Image teamColor = default;


		[SerializeField] private Image deadMark = default;


		[SerializeField] private ColorTweener deadMarkTweener = default;


		[SerializeField] private Image status = default;


		[SerializeField] private Image noDataBg = default;


		[SerializeField] private Transform itemsContainer = default;


		[SerializeField] private Image characterImg = default;


		[SerializeField] private ItemSlot weapon = default;


		[SerializeField] private ItemSlot chest = default;


		[SerializeField] private ItemSlot head = default;


		[SerializeField] private ItemSlot arm = default;


		[SerializeField] private ItemSlot leg = default;


		[SerializeField] private ItemSlot trinket = default;


		[SerializeField] private Button ignoreChatButton = default;


		[SerializeField] private Button ignorePingButton = default;


		[SerializeField] private Button ignoreEmotionButton = default;


		private LnText characterLevel = default;


		private int characterObjectId = default;


		protected LnText combatMasteryLevel = default;


		protected LnText growthMasteryLevel = default;


		private CanvasGroup ignoreCanvasGroup = default;


		private Image ignoreChatImage = default;


		private Image ignoreChatState = default;


		private Image ignoreEmotionImage = default;


		private Image ignoreEmotionState = default;


		private Image ignorePingImage = default;


		private Image ignorePingState = default;


		private float lastEncounteredTime = default;


		private LnText levelPrefix = default;


		protected LnText monsterKillCount = default;


		protected LnText nickname = default;


		protected LnText playerKillAssistCount = default;


		protected LnText playerKillCount = default;


		private ScoreCardState scoreCardState = default;


		private List<ItemSlot> scoreItemSlots = default;


		protected ScrollRect scrollRect = default;


		protected LnText searchMasteryLevel = default;


		public Image MySlot => mySlot;


		public Image DeadMark => deadMark;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			scoreItemSlots = new List<ItemSlot>
			{
				weapon,
				chest,
				head,
				arm,
				leg,
				trinket
			};
			nickname = GameUtil.Bind<LnText>(gameObject, "Name/UserNickname");
			characterLevel = GameUtil.Bind<LnText>(gameObject, "Contents/Lv/Value");
			levelPrefix = GameUtil.Bind<LnText>(gameObject, "Contents/Lv");
			combatMasteryLevel = GameUtil.Bind<LnText>(gameObject, "Contents/Mastery_Combat");
			searchMasteryLevel = GameUtil.Bind<LnText>(gameObject, "Contents/Mastery_Search");
			growthMasteryLevel = GameUtil.Bind<LnText>(gameObject, "Contents/Mastery_Growth");
			playerKillCount = GameUtil.Bind<LnText>(gameObject, "Contents/Mastery_PK");
			playerKillAssistCount = GameUtil.Bind<LnText>(gameObject, "Contents/Mastery_PKA");
			monsterKillCount = GameUtil.Bind<LnText>(gameObject, "Contents/Mastery_MK");
			if (teamColor != null)
			{
				teamColor.enabled = false;
			}

			if (status != null)
			{
				status.enabled = false;
			}

			if (deadMarkTweener != null)
			{
				deadMarkTweener.StopAnimation();
				deadMark.color = deadMarkTweener.from;
			}

			weapon.SetSlotType(SlotType.ScoreBoard);
			chest.SetSlotType(SlotType.ScoreBoard);
			head.SetSlotType(SlotType.ScoreBoard);
			arm.SetSlotType(SlotType.ScoreBoard);
			leg.SetSlotType(SlotType.ScoreBoard);
			trinket.SetSlotType(SlotType.ScoreBoard);
			foreach (ItemSlot itemSlot in scoreItemSlots)
			{
				itemSlot.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					ScrollRect scrollRect = this.scrollRect;
					if (scrollRect == null)
					{
						return;
					}

					scrollRect.OnBeginDrag(eventData);
				};
				itemSlot.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					ScrollRect scrollRect = this.scrollRect;
					if (scrollRect == null)
					{
						return;
					}

					scrollRect.OnEndDrag(eventData);
				};
				itemSlot.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					ScrollRect scrollRect = this.scrollRect;
					if (scrollRect == null)
					{
						return;
					}

					scrollRect.OnDrag(eventData);
				};
				itemSlot.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					ScrollRect scrollRect = this.scrollRect;
					if (scrollRect == null)
					{
						return;
					}

					scrollRect.OnScroll(eventData);
				};
			}

			InitIgnoreToggleEvent();
		}


		public void SetScrollRect(ScrollRect scrollRect)
		{
			this.scrollRect = scrollRect;
		}


		public void SetObjectId(int objectId)
		{
			characterObjectId = objectId;
			InitIgnoreToggle();
		}


		public void SetName(string name)
		{
			nickname.text = name;
		}


		public void SetMySlot(bool enable)
		{
			mySlot.enabled = enable;
		}


		public void SetCharacterImage(Sprite characterSprite)
		{
			characterImg.sprite = characterSprite;
		}


		public void SetTeamColor(bool isMyTeam, int teamSlot)
		{
			if (teamColor == null)
			{
				return;
			}

			teamColor.color = GameConstants.TeamMode.GetTeamColor(teamSlot);
			teamColor.enabled = isMyTeam;
		}


		public void Alive()
		{
			deadMark.enabled = false;
			if (deadMarkTweener != null)
			{
				deadMarkTweener.StopAnimation();
			}
		}


		public void DyingCondition()
		{
			deadMark.enabled = true;
			if (deadMarkTweener != null)
			{
				deadMarkTweener.PlayAnimation();
			}
		}


		public void Dead()
		{
			deadMark.enabled = true;
			if (deadMarkTweener != null)
			{
				deadMarkTweener.StopAnimation();
				deadMark.color = deadMarkTweener.from;
			}
		}


		public void Observing()
		{
			if (status == null)
			{
				return;
			}

			status.enabled = true;
			status.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Observe");
		}


		public void Disconnected()
		{
			if (status == null)
			{
				return;
			}

			status.enabled = true;
			status.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Unlink");
		}


		public void Connected()
		{
			if (status == null)
			{
				return;
			}

			status.enabled = false;
		}


		public void SetCharacterLevel(int level)
		{
			characterLevel.text = level.ToString();
		}


		public void SetEquips(List<Item> items)
		{
			ResetSlots();
			for (int i = 0; i < items.Count; i++)
			{
				ItemData itemData = items[i].ItemData;
				if (itemData.itemType == ItemType.Weapon)
				{
					weapon.SetItem(items[i]);
					weapon.SetSprite(items[i].ItemData.GetSprite());
					weapon.EnableBaseImage(false);
					weapon.SetBackground(items[i].ItemData.GetGradeSprite());
				}
				else if (itemData.itemType == ItemType.Armor)
				{
					switch (itemData.GetSubTypeData<ItemArmorData>().armorType)
					{
						case ArmorType.Head:
							head.SetItem(items[i]);
							head.SetSprite(items[i].ItemData.GetSprite());
							head.EnableBaseImage(false);
							head.SetBackground(items[i].ItemData.GetGradeSprite());
							break;
						case ArmorType.Chest:
							chest.SetItem(items[i]);
							chest.SetSprite(items[i].ItemData.GetSprite());
							chest.EnableBaseImage(false);
							chest.SetBackground(items[i].ItemData.GetGradeSprite());
							break;
						case ArmorType.Arm:
							arm.SetItem(items[i]);
							arm.SetSprite(items[i].ItemData.GetSprite());
							arm.EnableBaseImage(false);
							arm.SetBackground(items[i].ItemData.GetGradeSprite());
							break;
						case ArmorType.Leg:
							leg.SetItem(items[i]);
							leg.SetSprite(items[i].ItemData.GetSprite());
							leg.EnableBaseImage(false);
							leg.SetBackground(items[i].ItemData.GetGradeSprite());
							break;
						case ArmorType.Trinket:
							trinket.SetItem(items[i]);
							trinket.SetSprite(items[i].ItemData.GetSprite());
							trinket.EnableBaseImage(false);
							trinket.SetBackground(items[i].ItemData.GetGradeSprite());
							break;
					}
				}
			}
		}


		private void ResetSlots()
		{
			scoreItemSlots.ForEach(delegate(ItemSlot x)
			{
				x.ResetSlot();
				x.EnableBaseImage(true);
				x.SetBackground(null);
				x.SetDraggable(false);
			});
		}


		public void SetCombatMasteryLevel(int level)
		{
			combatMasteryLevel.text = level.ToString();
		}


		public void SetSearchMasteryLevel(int level)
		{
			searchMasteryLevel.text = level.ToString();
		}


		public void SetGrowthMasteryLevel(int level)
		{
			growthMasteryLevel.text = level.ToString();
		}


		public void SetPlayerKillCount(int killCount)
		{
			playerKillCount.text = killCount.ToString();
		}


		public void SetPlayerKillAssistCount(int assistCount)
		{
			playerKillAssistCount.text = assistCount.ToString();
		}


		public void SetMonsterKillCount(int killCount)
		{
			monsterKillCount.text = killCount.ToString();
		}


		public void SetScoreCardState(ScoreCardState state)
		{
			if (scoreCardState != state)
			{
				scoreCardState = state;
			}

			UpdateLayout();
		}


		private void UpdateLayout()
		{
			switch (scoreCardState)
			{
				case ScoreCardState.Known:
					contents.transform.localScale = Vector3.one;
					itemsContainer.transform.localScale = Vector3.one;
					noDataBg.transform.localScale = Vector3.zero;
					deadMark.enabled = false;
					SetFontColor(Color.white);
					return;
				case ScoreCardState.Unknown:
					contents.transform.localScale = Vector3.zero;
					noDataBg.transform.localScale = Vector3.one;
					deadMark.enabled = false;
					SetFontColor(Color.white);
					return;
				case ScoreCardState.Dead:
					contents.transform.localScale = Vector3.one;
					itemsContainer.transform.localScale = Vector3.zero;
					noDataBg.transform.localScale = Vector3.zero;
					deadMark.enabled = true;
					SetFontColor(DEAD_COLOR);
					return;
				default:
					return;
			}
		}


		private void SetFontColor(Color color)
		{
			nickname.color = color;
			characterLevel.color = color;
			levelPrefix.color = color;
			combatMasteryLevel.color = color;
			searchMasteryLevel.color = color;
			growthMasteryLevel.color = color;
			monsterKillCount.color = color;
		}


		public void SetLastEncounteredTime(float time)
		{
			lastEncounteredTime = time;
		}


		public void Show()
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
		}


		public void Hide()
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
		}


		private bool IsEnableIgnoreUI()
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return false;
			}

			bool flag = characterObjectId != inst.MyObjectId;
			bool flag2 = false;
			List<PlayerContext> teamMember = inst.GetTeamMember(inst.MyTeamNumber);
			if (teamMember != null)
			{
				flag2 = teamMember.Any(x => x.Character.ObjectId == characterObjectId);
			}

			return flag && flag2;
		}


		private void InitIgnoreToggleEvent()
		{
			if (ignoreChatButton == null || ignorePingButton == null || ignoreEmotionButton == null)
			{
				return;
			}

			ignoreCanvasGroup = GameUtil.Bind<CanvasGroup>(gameObject, "IgnoreList/Dimmed/Group");
			ignoreChatImage = ignoreChatButton.GetComponent<Image>();
			ignorePingImage = ignorePingButton.GetComponent<Image>();
			ignoreEmotionImage = ignoreEmotionButton.GetComponent<Image>();
			ignoreChatState = GameUtil.Bind<Image>(gameObject, "IgnoreState/Chat");
			ignorePingState = GameUtil.Bind<Image>(gameObject, "IgnoreState/Ping");
			ignoreEmotionState = GameUtil.Bind<Image>(gameObject, "IgnoreState/Emotion");
			ignoreChatButton.onClick.AddListener(delegate
			{
				MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Chat,
					!MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Chat,
						characterObjectId), new List<int>
					{
						characterObjectId
					}, delegate
					{
						InitIgnoreToggle();
						SetToolTip(IgnoreType.Chat);
					});
			});
			ignorePingButton.onClick.AddListener(delegate
			{
				MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Ping,
					!MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Ping,
						characterObjectId), new List<int>
					{
						characterObjectId
					}, delegate
					{
						InitIgnoreToggle();
						SetToolTip(IgnoreType.Ping);
					});
			});
			ignoreEmotionButton.onClick.AddListener(delegate
			{
				MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Emotion,
					!MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Emotion,
						characterObjectId), new List<int>
					{
						characterObjectId
					}, delegate
					{
						InitIgnoreToggle();
						SetToolTip(IgnoreType.Emotion);
					});
			});
		}


		public void OnPointerEnterChat(BaseEventData eventData)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			SetToolTip(IgnoreType.Chat);
			OnPointerEnter(ignoreChatButton.transform.position);
		}


		public void OnPointerEnterPing(BaseEventData eventData)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			SetToolTip(IgnoreType.Ping);
			OnPointerEnter(ignorePingButton.transform.position);
		}


		public void OnPointerEnterEmotion(BaseEventData eventData)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			SetToolTip(IgnoreType.Emotion);
			OnPointerEnter(ignoreEmotionButton.transform.position);
		}


		public void OnPointerExit(BaseEventData eventData)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}


		private void SetToolTip(IgnoreType type)
		{
			switch (type)
			{
				case IgnoreType.Ping:
				{
					bool flag =
						MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Ping,
							characterObjectId);
					MonoBehaviourInstance<Tooltip>.inst.SetLabel(
						Ln.Get(flag ? "Ignore/AllowPing" : "Ignore/IgnorePing"));
					return;
				}
				case IgnoreType.Chat:
				{
					bool flag2 =
						MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Chat,
							characterObjectId);
					MonoBehaviourInstance<Tooltip>.inst.SetLabel(
						Ln.Get(flag2 ? "Ignore/AllowChat" : "Ignore/IgnoreChat"));
					return;
				}
				case IgnoreType.Emotion:
				{
					bool flag3 =
						MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Emotion,
							characterObjectId);
					MonoBehaviourInstance<Tooltip>.inst.SetLabel(
						Ln.Get(flag3 ? "Ignore/AllowEmotion" : "Ignore/IgnoreEmotion"));
					return;
				}
				default:
					return;
			}
		}


		private void OnPointerEnter(Vector2 position)
		{
			Vector2 position2 = position + GameUtil.ConvertPositionOnScreenResolution(15f, 50f);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, position2, Tooltip.Pivot.LeftTop);
		}


		private void InitIgnoreToggle()
		{
			if (ignoreChatButton == null || ignorePingButton == null || ignoreEmotionButton == null)
			{
				return;
			}

			bool active = IsEnableIgnoreUI();
			ignoreChatButton.gameObject.SetActive(active);
			ignorePingButton.gameObject.SetActive(active);
			ignoreEmotionButton.gameObject.SetActive(active);
			bool flag = MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Chat,
				characterObjectId);
			bool flag2 =
				MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Ping,
					characterObjectId);
			bool flag3 =
				MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Emotion,
					characterObjectId);
			ignoreChatImage.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(flag
					? "Ico_Ignore_Chatting"
					: "Ico_See_Chatting");
			ignorePingImage.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(flag2
					? "Ico_Ignore_Ping"
					: "Ico_See_Ping");
			ignoreEmotionImage.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(flag3
					? "Ico_Ignore_Emotion"
					: "Ico_See_Emotion");
			ignoreChatState.gameObject.SetActive(flag);
			ignorePingState.gameObject.SetActive(flag2);
			ignoreEmotionState.gameObject.SetActive(flag3);
		}


		public void OnTriggerIgnoreUI(bool isActive)
		{
			if (ignoreCanvasGroup == null || !IsEnableIgnoreUI() || !MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			ignoreCanvasGroup.DOFade(isActive ? 0.9f : 0f, 0.25f).SetEase(Ease.OutCirc);
		}
	}
}