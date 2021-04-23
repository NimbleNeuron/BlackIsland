using System;
using System.Collections.Generic;
using System.Text;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class BattleRecordSlot : BaseUI
	{
		public delegate void OnPointerEnterMastery(MasteryType masterys, Vector3 position);


		public delegate void OnPointerExitMastery();


		[SerializeField] private GameObject rank1 = default;


		[SerializeField] private GameObject rank2 = default;


		[SerializeField] private GameObject rank3 = default;


		[SerializeField] private Text txtRank = default;


		[SerializeField] private Image characterImg = default;


		[SerializeField] private ItemSlot weapon = default;


		[SerializeField] private ItemSlot chest = default;


		[SerializeField] private ItemSlot head = default;


		[SerializeField] private ItemSlot arm = default;


		[SerializeField] private ItemSlot leg = default;


		[SerializeField] private ItemSlot trinket = default;


		private Text characterLevel;


		private Image combatMasteryIcon;


		private LnText combatMasteryLevel;


		public OnPointerEnterMastery EnterMasterys;


		public OnPointerExitMastery ExitMasterys;


		private Text gameDate;


		private BaseControl masteryCombat;


		private MasteryType masteryType;


		private LnText monsterKillCount;


		private LnText playerKillAssistCount;


		private LnText playerKillCount;


		private Text playTime;


		private List<ItemSlot> scoreItemSlots;


		private ScrollRect scrollRect;


		private LnText txtGameMode;

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
			txtGameMode = GameUtil.Bind<LnText>(gameObject, "BattleInfo/Name");
			characterLevel = GameUtil.Bind<Text>(gameObject, "BattleInfo/Txt_Level");
			playerKillCount = GameUtil.Bind<LnText>(gameObject, "KDH/Mastery_PK");
			playerKillAssistCount = GameUtil.Bind<LnText>(gameObject, "KDH/Mastery_PKA");
			monsterKillCount = GameUtil.Bind<LnText>(gameObject, "KDH/Mastery_MK");
			combatMasteryIcon = GameUtil.Bind<Image>(gameObject, "Mastery_Combat/Icon_Combat");
			combatMasteryLevel = GameUtil.Bind<LnText>(gameObject, "Mastery_Combat/Mastery_Combat");
			playTime = GameUtil.Bind<Text>(gameObject, "Time/Txt_PlayTime");
			gameDate = GameUtil.Bind<Text>(gameObject, "Time/Txt_Date");
			masteryCombat = GameUtil.Bind<BaseControl>(gameObject, "Mastery_Combat/Icon_Combat");
			weapon.SetSlotType(SlotType.ScoreBoard);
			chest.SetSlotType(SlotType.ScoreBoard);
			head.SetSlotType(SlotType.ScoreBoard);
			arm.SetSlotType(SlotType.ScoreBoard);
			leg.SetSlotType(SlotType.ScoreBoard);
			trinket.SetSlotType(SlotType.ScoreBoard);
			masteryCombat.OnPointerEnterEvent += delegate
			{
				OnPointerEnterMasterys(masteryType, masteryCombat.transform.position);
			};
			masteryCombat.OnPointerExitEvent += delegate { OnPointerExit(); };
			masteryCombat.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				OnPointerExit();
				ScrollRect scrollRect = this.scrollRect;
				if (scrollRect == null)
				{
					return;
				}

				scrollRect.OnBeginDrag(eventData);
			};
			masteryCombat.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				OnPointerExit();
				ScrollRect scrollRect = this.scrollRect;
				if (scrollRect == null)
				{
					return;
				}

				scrollRect.OnEndDrag(eventData);
			};
			masteryCombat.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				OnPointerExit();
				ScrollRect scrollRect = this.scrollRect;
				if (scrollRect == null)
				{
					return;
				}

				scrollRect.OnDrag(eventData);
			};
			masteryCombat.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				OnPointerExit();
				ScrollRect scrollRect = this.scrollRect;
				if (scrollRect == null)
				{
					return;
				}

				scrollRect.OnScroll(eventData);
			};
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
		}


		public void SetBattleRecord(BattleUserGame battleUserGame)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (battleUserGame.matchingMode == 2)
			{
				stringBuilder.Append(Ln.Get("일반대전"));
			}
			else if (battleUserGame.matchingMode == 3)
			{
				stringBuilder.Append(Ln.Get("랭크대전"));
			}

			if (battleUserGame.matchingTeamMode == 1)
			{
				stringBuilder.Append("(" + Ln.Get("솔로") + ")");
			}
			else if (battleUserGame.matchingTeamMode == 2)
			{
				stringBuilder.Append("(" + Ln.Get("듀오") + ")");
			}
			else if (battleUserGame.matchingTeamMode == 3)
			{
				stringBuilder.Append("(" + Ln.Get("스쿼드") + ")");
			}

			masteryType = (MasteryType) battleUserGame.bestWeapon;
			txtGameMode.text = stringBuilder.ToString();
			characterLevel.text = string.Format("Lv. {0}", battleUserGame.characterLevel);
			characterImg.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterScoreSprite(battleUserGame.characterNum);
			playerKillCount.text = battleUserGame.playerKill.ToString();
			playerKillAssistCount.text = battleUserGame.playerAssistant.ToString();
			monsterKillCount.text = battleUserGame.monsterKill.ToString();
			combatMasteryIcon.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetWeaponMasterySprite(
					(WeaponType) battleUserGame.bestWeapon);
			combatMasteryLevel.text = battleUserGame.bestWeaponLevel.ToString();
			playTime.text = LnUtil.GetRankRemainTimeText(battleUserGame.duration);
			gameDate.text = string.Format("{0:yyyy-MM-dd\nHH:mm:ss}",
				GameUtil.ConvertTimeFromUtc(battleUserGame.startDtm / 1000L, TimeZoneInfo.Local));
			SetRank((int) battleUserGame.gameRank);
			List<Item> list = new List<Item>();
			foreach (int code in battleUserGame.equipment.Values)
			{
				ItemData itemData = GameDB.item.FindItemByCode(code);
				Item item = new Item(0, itemData.code, 1, 0, itemData);
				list.Add(item);
			}

			SetEquips(list);
		}


		public void SetScrollRect(ScrollRect scrollRect)
		{
			this.scrollRect = scrollRect;
		}


		private void SetRank(int rank)
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


		private void OnPointerEnterMasterys(MasteryType masteryType, Vector3 pos)
		{
			pos.y += GameUtil.ConvertPositionOnScreenResolution(0f, 60f).y;
			EnterMasterys(masteryType, pos);
		}


		public void OnPointerExit()
		{
			ExitMasterys();
		}
	}
}