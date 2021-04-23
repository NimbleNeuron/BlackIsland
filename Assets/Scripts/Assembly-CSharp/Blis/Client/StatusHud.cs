using System.Collections.Generic;
using System.Linq;
using Blis.Client.UIModel;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class StatusHud : BaseUI, ISlotEventListener
	{
		[SerializeField] private ButtonImageSwap statSwapButton = default;


		[SerializeField] private StatExtension statExtension = default;


		[SerializeField] private GameObject tutorialSquareWeaponType = default;


		[SerializeField] private GameObject tutorialSquareEquip = default;


		[SerializeField] private GameObject tutorialArrowEquip = default;


		private EquipItemSlot armSlot;


		private LnText attack;


		private LnText attackSpeed;


		private UIStateTable buffTable;


		private CanvasGroup canvasGroupInBattle;


		private EquipItemSlot clothSlot;


		private Text cooldown;


		private UIStateTable debuffTable;


		private LnText defence;


		private int drawEp;


		private int drawEpMax;


		private UIProgress epBar;


		private Color? epGaugeColor;


		private List<EquipItemSlot> equipSlots;


		private Image expGage;


		private EquipItemSlot headSlot;


		private UIProgress hpBar;


		private LnText hpRegen;


		private LnText increaseAttack;


		private LnText increaseSkillRatio;


		private EquipItemSlot legSlot;


		private LnText level;


		private InfoMaker masteryInfoMaker;


		private Image masteryLock;


		private Image portrait;


		private LnText preventAttack;


		private LnText preventSkillRatio;


		private Image reload;


		private LnText reloadKeyCode;


		private UIProgress spBar;


		private LnText speed;


		private LnText spRegen;


		private ExtraPointDisplayData targetEpDisplayData;


		private Image teamColor;


		private EquipItemSlot trinketSlot;


		private EquipItemSlot weaponSlot;


		private Image weaponType;


		public List<EquipItemSlot> EquipSlots => equipSlots;


		public void OnSlotLeftClick(Slot slot)
		{
			ItemSlot itemSlot = slot as ItemSlot;
			if (itemSlot != null && itemSlot.GetItem() != null)
			{
				SingletonMonoBehaviour<PlayerController>.inst.UnequipItem(itemSlot.GetItem());
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}


		public void OnSlotRightClick(Slot slot)
		{
			ItemSlot itemSlot = slot as ItemSlot;
			if (itemSlot != null && itemSlot.GetItem() != null)
			{
				ItemData itemData = itemSlot.GetItem().ItemData;
				MonoBehaviourInstance<GameUI>.inst.CombineWindow.SelectItem(itemData);
				MonoBehaviourInstance<GameUI>.inst.OpenWindow(MonoBehaviourInstance<GameUI>.inst.CombineWindow);
			}
		}


		public void OnDropItem(Slot slot, BaseUI draggedUI)
		{
			ItemSlot itemSlot = draggedUI as ItemSlot;
			ItemSlot x = slot as ItemSlot;
			if (itemSlot == null || x == null)
			{
				Log.E("OnDropItem error.");
				return;
			}

			if (itemSlot.GetParentUI() is InventoryHud)
			{
				Item item = itemSlot.GetItem();
				if (item != null && (item.ItemData.itemType == ItemType.Weapon ||
				                     item.ItemData.itemType == ItemType.Armor))
				{
					SingletonMonoBehaviour<PlayerController>.inst.EquipItem(item);
				}
			}
		}


		public void OnThrowItem(Slot slot)
		{
			ItemSlot itemSlot = slot as ItemSlot;
			if (itemSlot != null && itemSlot.GetItem() != null)
			{
				if (MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
				    MonoBehaviourInstance<TutorialController>.inst.DontThrowItems(itemSlot.GetItem().itemCode))
				{
					MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("아이템을 버릴 수 없습니다."));
					return;
				}

				SingletonMonoBehaviour<PlayerController>.inst.DropItem(itemSlot.GetSlotType(), itemSlot.GetItem());
			}
		}


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot) { }


		public void OnPointerExit(Slot slot) { }


		public void OnSlotDoubleClick(Slot slot) { }


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			portrait = GameUtil.Bind<Image>(gameObject, "Portrait/Character");
			weaponType = GameUtil.Bind<Image>(gameObject, "WeaponType/Img");
			masteryLock = GameUtil.Bind<Image>(gameObject, "WeaponType/Lock");
			masteryInfoMaker = GameUtil.Bind<InfoMaker>(gameObject, "MasteryBtn");
			reload = GameUtil.Bind<Image>(gameObject, "WeaponType/Img/Reload");
			reloadKeyCode = GameUtil.Bind<LnText>(gameObject, "WeaponType/Img/Reload/KeyCodeText");
			teamColor = GameUtil.Bind<Image>(gameObject, "Level/Bg");
			level = GameUtil.Bind<LnText>(gameObject, "Level/Label");
			expGage = GameUtil.Bind<Image>(gameObject, "Level/Gage");
			canvasGroupInBattle = GameUtil.Bind<CanvasGroup>(gameObject, "Level/InBattle");
			hpBar = GameUtil.Bind<UIProgress>(gameObject, "PointBar/HP");
			hpRegen = GameUtil.Bind<LnText>(gameObject, "PointBar/HP/RegenLabel");
			hpBar.OnPointerEnterEvent += delegate { hpRegen.gameObject.SetActive(true); };
			hpBar.OnPointerExitEvent += delegate { hpRegen.gameObject.SetActive(false); };
			spBar = GameUtil.Bind<UIProgress>(gameObject, "PointBar/SP");
			spRegen = GameUtil.Bind<LnText>(gameObject, "PointBar/SP/RegenLabel");
			spBar.OnPointerEnterEvent += delegate { spRegen.gameObject.SetActive(true); };
			spBar.OnPointerExitEvent += delegate { spRegen.gameObject.SetActive(false); };
			epBar = GameUtil.Bind<UIProgress>(gameObject, "PointBar/Extra");
			attack = GameUtil.Bind<LnText>(gameObject, "Detail/Attack/Text");
			defence = GameUtil.Bind<LnText>(gameObject, "Detail/Defense/Text");
			increaseAttack = GameUtil.Bind<LnText>(gameObject, "Detail/IncreaseAttack/Text");
			preventAttack = GameUtil.Bind<LnText>(gameObject, "Detail/PreventAttack/Text");
			increaseSkillRatio = GameUtil.Bind<LnText>(gameObject, "Detail/IncreaseSkillRatio/Text");
			preventSkillRatio = GameUtil.Bind<LnText>(gameObject, "Detail/PreventSkillRatio/Text");
			attackSpeed = GameUtil.Bind<LnText>(gameObject, "Detail/AttackSpeed/Text");
			speed = GameUtil.Bind<LnText>(gameObject, "Detail/MoveSpeed/Text");
			buffTable = GameUtil.Bind<UIStateTable>(gameObject, "StateTable");
			buffTable.SetSlotType(SlotType.None);
			debuffTable = GameUtil.Bind<UIStateTable>(gameObject, "StateTable_DeBuff");
			debuffTable.SetSlotType(SlotType.None);
			weaponSlot = GameUtil.Bind<EquipItemSlot>(gameObject, "Equip/Weapon");
			clothSlot = GameUtil.Bind<EquipItemSlot>(gameObject, "Equip/Cloth");
			headSlot = GameUtil.Bind<EquipItemSlot>(gameObject, "Equip/Head");
			armSlot = GameUtil.Bind<EquipItemSlot>(gameObject, "Equip/Arm");
			legSlot = GameUtil.Bind<EquipItemSlot>(gameObject, "Equip/Leg");
			trinketSlot = GameUtil.Bind<EquipItemSlot>(gameObject, "Equip/Trinket");
			weaponSlot.SetSlotType(SlotType.Equipment);
			clothSlot.SetSlotType(SlotType.Equipment);
			headSlot.SetSlotType(SlotType.Equipment);
			armSlot.SetSlotType(SlotType.Equipment);
			legSlot.SetSlotType(SlotType.Equipment);
			trinketSlot.SetSlotType(SlotType.Equipment);
			equipSlots = new List<EquipItemSlot>();
			equipSlots.Add(weaponSlot);
			equipSlots.Add(clothSlot);
			equipSlots.Add(headSlot);
			equipSlots.Add(armSlot);
			equipSlots.Add(legSlot);
			equipSlots.Add(trinketSlot);
			equipSlots.ForEach(delegate(EquipItemSlot x) { x.SetEventListener(this); });
			statSwapButton.onSwap.AddListener(delegate
			{
				statExtension.gameObject.SetActive(!statExtension.gameObject.activeSelf);
			});
		}


		protected override void OnStartUI()
		{
			SetMasteryKeyCode(Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.OpenCharacterMastery),
				Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.OpenCharacterMastery));
			SetReloadKeyCode(Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.Reload),
				Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.Reload));
		}


		public void ShowInBattleUI()
		{
			canvasGroupInBattle.alpha = 1f;
			canvasGroupInBattle.gameObject.SetActive(true);
		}


		public void HideInBattleUI()
		{
			canvasGroupInBattle.gameObject.SetActive(false);
		}


		public void SetMasteryKeyCode(KeyCode keyCode, KeyCode[] combinations)
		{
			List<KeyCode> list = combinations.ToList<KeyCode>();
			list.Add(keyCode);
			string text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
			masteryInfoMaker.keyCode = text;
			masteryInfoMaker.SetTextKeyCode(text);
		}


		public void SetReloadKeyCode(KeyCode keyCode, KeyCode[] combinations)
		{
			List<KeyCode> list = combinations.ToList<KeyCode>();
			list.Add(keyCode);
			string text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
			reloadKeyCode.text = text;
		}


		public void Init(int characterCode, int skinIndex, int level, int exp, UICharacterStat stat)
		{
			epGaugeColor = null;
			drawEp = 0;
			drawEpMax = 0;
			targetEpDisplayData = ExtraPointDisplayManager.instance.GetData(characterCode);
			if (targetEpDisplayData == null)
			{
				epBar.gameObject.SetActive(false);
			}
			else if (targetEpDisplayData.ExtraPointDisplayType != ExtraPointDisplayType.Gauge)
			{
				epBar.gameObject.SetActive(false);
				targetEpDisplayData = null;
			}
			else
			{
				epBar.gameObject.SetActive(true);
				RectTransform rectTransform = GameUtil.Bind<RectTransform>(gameObject, "PointBar/Extra/Guidline");
				if (targetEpDisplayData.Dots.Count == 0)
				{
					rectTransform.gameObject.SetActive(false);
				}
				else
				{
					rectTransform.gameObject.SetActive(true);
					RectTransform rectTransform2 = epBar.foreground.rectTransform;
					float width = rectTransform2.rect.width;
					Vector2 anchoredPosition = rectTransform2.anchoredPosition;
					rectTransform.anchoredPosition =
						anchoredPosition + new Vector2(width * targetEpDisplayData.Dots[0], 0f);
					for (int i = 1; i < targetEpDisplayData.Dots.Count; i++)
					{
						RectTransform component = Instantiate<GameObject>(rectTransform.gameObject)
							.GetComponent<RectTransform>();
						component.SetParent(rectTransform2.parent);
						component.localScale = Vector3.one;
						component.anchoredPosition3D =
							anchoredPosition + new Vector2(width * targetEpDisplayData.Dots[i], 0f);
					}
				}

				Image image = GameUtil.Bind<Image>(gameObject, "PointBar/Extra/Label/Icon");
				if (targetEpDisplayData.GaugeIcon == null)
				{
					image.enabled = false;
				}
				else
				{
					image.enabled = true;
					image.sprite = targetEpDisplayData.GaugeIcon;
				}
			}

			SetCharacterLevel(level, exp);
			OnUpdateStat(stat);
			portrait.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(characterCode, skinIndex);
		}


		public void SetCharacterLevel(int characterLevel, int characterLevelExp)
		{
			level.text = characterLevel.ToString();
			int levelUpExp = GameDB.character.GetExpData(characterLevel).levelUpExp;
			expGage.fillAmount = characterLevelExp / (float) levelUpExp;
		}


		public void SetTeamColor(int teamSlot)
		{
			teamColor.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(1 <= teamSlot
				? string.Format("Ico_Map_PointPin_{0:D2}", teamSlot)
				: "Ico_Map_PointPin_01");
		}


		public void OnUpdateStat(UICharacterStat stat, bool isInCombat = false)
		{
			UpdateStatUI(stat, isInCombat);
		}


		public void PlayEffectItemSlot(Item srcItem)
		{
			int itemCode = srcItem.itemCode;
			bool flag = true;
			if (srcItem.ItemData.itemType == ItemType.Weapon)
			{
				flag = MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsEquipableWeapon(srcItem.ItemData);
			}

			EquipItemSlot statusItemSlot = GetStatusItemSlot(srcItem);
			int num = -1;
			if (statusItemSlot.GetItem() != null)
			{
				num = statusItemSlot.GetItem().itemCode;
			}

			if (flag && itemCode != num)
			{
				statusItemSlot.PlayEffect();
				Singleton<SoundControl>.inst.PlayUISound("equipmentInstall",
					ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
			}
		}


		public EquipItemSlot GetStatusItemSlot(Item item)
		{
			if (item.ItemData.itemType == ItemType.Weapon)
			{
				return weaponSlot;
			}

			if (item.ItemData.itemType == ItemType.Armor)
			{
				switch (item.ItemData.GetSubTypeData<ItemArmorData>().armorType)
				{
					case ArmorType.Head:
						return headSlot;
					case ArmorType.Chest:
						return clothSlot;
					case ArmorType.Arm:
						return armSlot;
					case ArmorType.Leg:
						return legSlot;
					case ArmorType.Trinket:
						return trinketSlot;
				}
			}

			return null;
		}


		public void OnUpdateEquips(List<Item> items, List<Item> updates)
		{
			MyPlayerContext myPlayer = MonoBehaviourInstance<ClientService>.inst.MyPlayer;
			List<Item> list = myPlayer != null ? myPlayer.PreEquipment.GetEquips() : null;
			ResetSlots();
			foreach (Item item in updates)
			{
				bool flag = false;
				foreach (Item item2 in list)
				{
					if (item.itemCode == item2.itemCode)
					{
						flag = true;
						break;
					}
				}

				if (!flag)
				{
					PlayEffectItemSlot(item);
				}
			}

			Item item3 = null;
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].ItemData.itemType == ItemType.Weapon)
				{
					item3 = items[i];
					UpdateSlot(weaponSlot, item3);
				}
				else if (items[i].ItemData.itemType == ItemType.Armor)
				{
					switch (items[i].ItemData.GetSubTypeData<ItemArmorData>().armorType)
					{
						case ArmorType.Head:
							UpdateSlot(headSlot, items[i]);
							break;
						case ArmorType.Chest:
							UpdateSlot(clothSlot, items[i]);
							break;
						case ArmorType.Arm:
							UpdateSlot(armSlot, items[i]);
							break;
						case ArmorType.Leg:
							UpdateSlot(legSlot, items[i]);
							break;
						case ArmorType.Trinket:
							UpdateSlot(trinketSlot, items[i]);
							break;
					}
				}
			}

			masteryLock.color = item3 == null
				? new Color(masteryLock.color.r, masteryLock.color.g, masteryLock.color.b, 1f)
				: new Color(masteryLock.color.r, masteryLock.color.g, masteryLock.color.b, 0f);
			weaponType.color = item3 == null ? Color.gray : Color.white;
			reload.enabled = false;
			reloadKeyCode.enabled = false;
			if (item3 != null)
			{
				weaponType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetWeaponMasterySprite(item3.WeaponTypeInfoData.type);
				bool enabled = item3.ItemData.IsGunType();
				reload.enabled = enabled;
				reloadKeyCode.enabled = enabled;
			}
		}


		public EquipItemSlot GetEqualEquipItemId(int id)
		{
			foreach (EquipItemSlot equipItemSlot in equipSlots)
			{
				if (equipItemSlot.GetItem() != null && equipItemSlot.GetItem().id == id)
				{
					return equipItemSlot;
				}
			}

			return null;
		}


		public Item GetCompareEquipItemData(ItemData itemData)
		{
			if (itemData.itemType == ItemType.Weapon &&
			    !MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsEquipableWeapon(itemData))
			{
				return null;
			}

			foreach (EquipItemSlot equipItemSlot in equipSlots)
			{
				if (equipItemSlot.GetItem() != null)
				{
					if (equipItemSlot.GetItem().itemCode == itemData.code)
					{
						break;
					}

					if (itemData.itemType == ItemType.Weapon)
					{
						if (equipItemSlot.GetItem().ItemData.itemType == itemData.itemType)
						{
							return equipItemSlot.GetItem();
						}
					}
					else if (equipItemSlot.GetItem().ItemData.itemType == ItemType.Armor &&
					         itemData.itemType == ItemType.Armor)
					{
						ArmorType armorType =
							equipItemSlot.GetItem().ItemData.GetSubTypeData<ItemArmorData>().armorType;
						ArmorType armorType2 = itemData.GetSubTypeData<ItemArmorData>().armorType;
						if (armorType == armorType2)
						{
							return equipItemSlot.GetItem();
						}
					}
					else if (equipItemSlot.GetItem().ItemData.itemType == itemData.itemType)
					{
						return equipItemSlot.GetItem();
					}
				}
			}

			return null;
		}


		public EquipItemSlot GetWeaponEquipItemSlot()
		{
			EquipSlotType index = EquipSlotType.Weapon;
			if (equipSlots[(int) index].GetItem() != null)
			{
				return equipSlots[(int) index];
			}

			return null;
		}


		public EquipItemSlot GetEquipItemEqualType(ItemData itemData)
		{
			if (itemData.itemType == ItemType.Weapon &&
			    !MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsEquipableWeapon(itemData))
			{
				return null;
			}

			ItemType itemType = itemData.itemType;
			EquipSlotType index;
			if (itemType != ItemType.Weapon)
			{
				if (itemType != ItemType.Armor)
				{
					return null;
				}

				index = itemData.GetSubTypeData<ItemArmorData>().armorType.GetEquipSlotType();
			}
			else
			{
				index = EquipSlotType.Weapon;
			}

			EquipItemSlot equipItemSlot = equipSlots[(int) index];
			if (equipItemSlot.GetItem() == null)
			{
				return equipItemSlot;
			}

			return null;
		}


		public Item GetCompareEquipItemData(ItemData itemData, ref EquipItemSlot equipitemSlot)
		{
			if (itemData.itemType == ItemType.Weapon &&
			    !MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsEquipableWeapon(itemData))
			{
				return null;
			}

			foreach (EquipItemSlot equipItemSlot in equipSlots)
			{
				if (equipItemSlot.GetItem() != null)
				{
					if (equipItemSlot.GetItem().itemCode == itemData.code)
					{
						break;
					}

					if (itemData.itemType == ItemType.Weapon)
					{
						if (equipItemSlot.GetItem().ItemData.itemType == itemData.itemType)
						{
							equipitemSlot = equipItemSlot;
							return equipItemSlot.GetItem();
						}
					}
					else if (equipItemSlot.GetItem().ItemData.itemType == ItemType.Armor &&
					         itemData.itemType == ItemType.Armor)
					{
						ArmorType armorType =
							equipItemSlot.GetItem().ItemData.GetSubTypeData<ItemArmorData>().armorType;
						ArmorType armorType2 = itemData.GetSubTypeData<ItemArmorData>().armorType;
						if (armorType == armorType2)
						{
							equipitemSlot = equipItemSlot;
							return equipItemSlot.GetItem();
						}
					}
					else if (equipItemSlot.GetItem().ItemData.itemType == itemData.itemType)
					{
						equipitemSlot = equipItemSlot;
						return equipItemSlot.GetItem();
					}
				}
			}

			return null;
		}


		public void ShowTutorialSquareWeaponType(bool show)
		{
			tutorialSquareWeaponType.SetActive(show);
		}


		public void ShowTutorialSquareEquip(bool show)
		{
			tutorialSquareEquip.SetActive(show);
			tutorialArrowEquip.SetActive(show);
		}


		private void UpdateSlot(EquipItemSlot slot, Item item)
		{
			if (item != null)
			{
				slot.EnableBaseImage(false);
				slot.SetItem(item);
				slot.SetSprite(item.ItemData.GetSprite());
				slot.SetBackground(item.ItemData.GetGradeSprite());
				if (item.ItemData.IsThrowType())
				{
					slot.SetBulletStackText(item.itemCode, item.Bullet);
				}
			}
		}


		private void ResetSlots()
		{
			equipSlots.ForEach(delegate(EquipItemSlot x)
			{
				x.ResetSlot();
				x.EnableBaseImage(true);
			});
		}


		private void UpdateStatUI(UICharacterStat stat, bool isInCombat)
		{
			UpdateHpBar(stat.hp, stat.maxHp);
			hpRegen.text = string.Format("+{0}", stat.hpGen);
			UpdateSpBar(stat.sp, stat.maxSp);
			spRegen.text = string.Format("+{0}", stat.spGen);
			UpdateSpBar(stat.sp, stat.maxSp);
			UpdateEpBar(stat.ep, stat.maxEp);
			attack.text = Mathf.RoundToInt(stat.attack).ToString();
			defence.text = Mathf.RoundToInt(stat.defense).ToString();
			increaseAttack.text = string.Format("{0:0} | {1:0}%", stat.increaseAttack, stat.increaseAttackRatio * 100f);
			preventAttack.text = string.Format("{0:0} | {1:0}%", stat.preventAttack, stat.preventAttackRatio * 100f);
			increaseSkillRatio.text =
				string.Format("{0:0.#} | {1:0}%", stat.increaseSkill, stat.increaseSkillRatio * 100f);
			preventSkillRatio.text = string.Format("0 | {0:0}%", stat.preventSkillRatio * 100f);
			attackSpeed.text = string.Format("{0:0.##}", stat.attackSpeed);
			speed.text = isInCombat
				? string.Format("{0:0.##}", stat.moveSpeed)
				: string.Format("{0:0.##}", stat.moveSpeedOutOfCombat);
			statExtension.SetStat(stat);
		}


		public void UpdateHpBar(int hp, int hpMax)
		{
			hpBar.SetValue(hp, hpMax);
		}


		public void UpdateSpBar(int sp, int spMax)
		{
			spBar.SetValue(sp, spMax);
		}


		public void UpdateEpBar(int ep, int epMax)
		{
			if (targetEpDisplayData == null)
			{
				return;
			}

			if (drawEp == ep && drawEpMax == epMax)
			{
				return;
			}

			epBar.SetValue(ep, epMax);
			drawEp = ep;
			drawEpMax = epMax;
			UpdateEpColor();
		}


		private void UpdateEpColor()
		{
			if (targetEpDisplayData == null)
			{
				return;
			}

			Color color = Color.clear;
			if (epGaugeColor != null)
			{
				color = epGaugeColor.Value;
			}
			else
			{
				float num = drawEp / (float) drawEpMax;
				int num2 = 0;
				while (num2 < targetEpDisplayData.Colors.Count && targetEpDisplayData.ColorStarts[num2] <= num)
				{
					color = targetEpDisplayData.Colors[num2];
					num2++;
				}
			}

			epBar.SetColor(color);
		}


		public void SettingForceEpGaugeColor(Color? clr)
		{
			epGaugeColor = clr;
			UpdateEpColor();
		}


		public void OnClickPortrait() { }


		public void UpdateBuffPosition()
		{
			if (MonoBehaviourInstance<GameUI>.inst.SkillHud.IsExistSkillPoint())
			{
				buffTable.transform.localPosition = new Vector3(-308f, 206f, 0f);
				debuffTable.transform.localPosition = new Vector3(148f, 206f, 0f);
				return;
			}

			buffTable.transform.localPosition = new Vector3(-308f, 132f, 0f);
			debuffTable.transform.localPosition = new Vector3(148f, 132f, 0f);
		}


		public void AddState(CharacterStateValue characterState)
		{
			EffectType effectType = characterState.EffectType;
			if (effectType == EffectType.Buff)
			{
				buffTable.AddState(characterState);
				return;
			}

			if (effectType != EffectType.Debuff)
			{
				return;
			}

			debuffTable.AddState(characterState);
		}


		public void RemoveState(CharacterStateValue characterState)
		{
			EffectType effectType = characterState.EffectType;
			if (effectType == EffectType.Buff)
			{
				buffTable.RemoveState(characterState);
				return;
			}

			if (effectType != EffectType.Debuff)
			{
				return;
			}

			debuffTable.RemoveState(characterState);
		}


		public void RemoveAllState()
		{
			buffTable.Clear();
			debuffTable.Clear();
		}


		public void RemoveStateOnDead()
		{
			buffTable.RemoveStateOnDead();
			debuffTable.RemoveStateOnDead();
		}


		public void UpdateState(CharacterStateValue characterState)
		{
			EffectType effectType = characterState.EffectType;
			if (effectType == EffectType.Buff)
			{
				buffTable.UpdateState(characterState);
				return;
			}

			if (effectType != EffectType.Debuff)
			{
				return;
			}

			debuffTable.UpdateState(characterState);
		}


		public void StopBulletCooldown(int itemId)
		{
			if (weaponSlot.GetItem() != null && weaponSlot.GetItem().id == itemId)
			{
				weaponSlot.StopBulletCooldown();
			}
		}


		public void ToggleMasteryWindow()
		{
			MonoBehaviourInstance<GameUI>.inst.MasteryWindow.ToggleWindow();
		}


		public void ToggleStatExtension()
		{
			statSwapButton.Swap();
		}


		public void OnNavigationClear(ItemData itemData)
		{
			ItemSlot itemSlot = equipSlots.Find(x => x.GetItem() != null && x.GetItem().itemCode == itemData.code);
			if (itemSlot != null)
			{
				itemSlot.PlayEffect();
			}
		}
	}
}