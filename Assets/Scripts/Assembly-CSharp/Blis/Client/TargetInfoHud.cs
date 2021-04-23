using System.Collections.Generic;
using Blis.Client.UIModel;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TargetInfoHud : BaseUI
	{
		private readonly List<SimpleItemSlot> itemSlots = new List<SimpleItemSlot>();


		private readonly Color NON_PLAYER_SP_COLOR = new Color(255f, 255f, 255f);


		private readonly Color PLAYER_SP_COLOR = new Color(0f, 160f, 215f);


		private readonly List<SimpleSlot> simpleSlots = new List<SimpleSlot>();


		private Text attack;


		private Text attackSpeed;


		private UIStateTable buffTable;


		private Image character;


		private Text cooldown;


		private Text critical;


		private UIStateTable debuffTable;


		private Text defense;


		private Image expGage;


		private UIProgress hpBar;


		private Text level;


		private GameObject monsterHead;


		private Text monsterKill;


		private GameObject playerHead;


		private Text playerKill;


		private Text playerKillAssist;


		private UIProgress spBar;


		private Text speed;


		private Image teamColor;


		private Image weaponType;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			character = GameUtil.Bind<Image>(gameObject, "Portrait");
			teamColor = GameUtil.Bind<Image>(gameObject, "Level/Bg");
			level = GameUtil.Bind<Text>(gameObject, "Level/Label");
			expGage = GameUtil.Bind<Image>(gameObject, "Level/Gage");
			attack = GameUtil.Bind<Text>(gameObject, "StatusBox/Offense/Text");
			defense = GameUtil.Bind<Text>(gameObject, "StatusBox/Defense/Text");
			attackSpeed = GameUtil.Bind<Text>(gameObject, "StatusBox/AttackSpeed/Text");
			critical = GameUtil.Bind<Text>(gameObject, "StatusBox/CriticalRate/Text");
			speed = GameUtil.Bind<Text>(gameObject, "StatusBox/MoveSpeed/Text");
			cooldown = GameUtil.Bind<Text>(gameObject, "StatusBox/Cooldown/Text");
			playerHead = transform.Find("PlayerHead").gameObject;
			weaponType = GameUtil.Bind<Image>(playerHead, "WeaponType/Img");
			playerKill = GameUtil.Bind<Text>(playerHead, "Kill/Character/Text");
			playerKillAssist = GameUtil.Bind<Text>(playerHead, "Kill/Assist/Text");
			monsterKill = GameUtil.Bind<Text>(playerHead, "Kill/Monster/Text");
			monsterHead = transform.Find("MonsterHead").gameObject;
			monsterHead.GetComponentsInChildren<SimpleSlot>(simpleSlots);
			hpBar = GameUtil.Bind<UIProgress>(gameObject, "Detail/PointBar/HP");
			spBar = GameUtil.Bind<UIProgress>(gameObject, "Detail/PointBar/SP");
			transform.Find("Detail/Items").gameObject.GetComponentsInChildren<SimpleItemSlot>(itemSlots);
			buffTable = GameUtil.Bind<UIStateTable>(gameObject, "Buff");
			buffTable.SetSlotType(SlotType.TargetInfo);
			debuffTable = GameUtil.Bind<UIStateTable>(gameObject, "DeBuff");
			debuffTable.SetSlotType(SlotType.TargetInfo);
			HideTargetHud();
		}


		public void ShowTargetHud(LocalCharacter localCharacter)
		{
			ObjectType objectType = localCharacter.ObjectType;
			switch (objectType)
			{
				case ObjectType.PlayerCharacter:
					break;
				case ObjectType.Monster:
					SetMonsterHud((LocalMonster) localCharacter);
					goto IL_85;
				case ObjectType.Item:
				case ObjectType.ItemBox:
				case ObjectType.StaticItemBox:
				case ObjectType.ResourceItemBox:
				case ObjectType.AirSupplyItemBox:
					goto IL_78;
				case ObjectType.SummonCamera:
				case ObjectType.SummonTrap:
				case ObjectType.SummonServant:
					SetSummonHud((LocalSummonBase) localCharacter);
					goto IL_85;
				case ObjectType.Dummy:
					SetDummyHud((LocalDummy) localCharacter);
					goto IL_85;
				default:
					if (objectType != ObjectType.BotPlayerCharacter)
					{
						goto IL_78;
					}

					break;
			}

			SetPlayerHud((LocalPlayerCharacter) localCharacter);
			goto IL_85;
			IL_78:
			gameObject.SetActive(false);
			return;
			IL_85:
			SetTargetStat(new UITargetInfoHudStat(localCharacter.Stat, localCharacter.Status));
			SetTargetState(localCharacter.States);
			gameObject.SetActive(true);
		}


		public void HideTargetHud()
		{
			gameObject.SetActive(false);
		}


		private void SetPlayerHud(LocalPlayerCharacter player)
		{
			playerHead.SetActive(true);
			monsterHead.SetActive(false);
			character.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(player.CharacterCode);
			teamColor.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(1 <= player.TeamSlot
				? string.Format("Ico_Map_PointPin_{0:D2}", player.TeamSlot)
				: "Ico_Map_PointPin_04");
			Item weapon = player.GetWeapon();
			weaponType.color = weapon == null ? Color.gray : Color.white;
			if (weapon != null)
			{
				weaponType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetWeaponMasterySprite(weapon.WeaponTypeInfoData.type);
			}

			SetPlayerLevel(player.Status.Level, player.Status.Exp);
			SetTargetItems(player.GetEquipments());
			SetTargetKillScore(player.Status.PlayerKill, player.Status.PlayerKillAssist, player.Status.MonsterKill);
			spBar.SetColor(PLAYER_SP_COLOR);
		}


		private void SetMonsterHud(LocalMonster monster)
		{
			playerHead.SetActive(false);
			monsterHead.SetActive(true);
			character.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetMonsterProfileSprite(monster.CharacterCode);
			teamColor.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_PointPin_04");
			SetNonPlayerLevel(monster.Status.Level);
			int num = 0;
			if (monster.MonsterType == MonsterType.WildDog)
			{
				SkillData skillData = GameDB.skill.GetSkillData(2004201);
				simpleSlots[num].SetSimpleSlotBehaviour(new SimpleSlotSkillBehaviour(simpleSlots[num], skillData));
				simpleSlots[num].SetIcon(null);
				simpleSlots[num].Enable();
				num++;
			}
			else
			{
				foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
				{
					if (simpleSlots.Count <= num)
					{
						break;
					}

					if (skillSlotSet != SkillSlotSet.None)
					{
						SkillSlotIndex skillSlotIndex = skillSlotSet.SlotSet2Index();
						if (skillSlotIndex >= SkillSlotIndex.Active1 && skillSlotIndex < SkillSlotIndex.WeaponSkill &&
						    SetSkillSlot(monster.CharacterCode, ObjectType.Monster, skillSlotSet, simpleSlots[num]))
						{
							simpleSlots[num].Enable();
							num++;
						}
					}
				}
			}

			for (int i = num; i < simpleSlots.Count; i++)
			{
				simpleSlots[i].Disable();
			}

			ResetItemSlots();
			MonsterData monsterData = GameDB.monster.GetMonsterData(monster.CharacterCode);
			SetDropItems(monsterData);
			spBar.SetColor(NON_PLAYER_SP_COLOR);
		}


		private void SetSummonHud(LocalSummonBase summon)
		{
			playerHead.SetActive(false);
			monsterHead.SetActive(true);
			character.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetSummonProfileSprite(summon.CharacterCode);
			if (summon.Owner == null)
			{
				teamColor.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_PointPin_04");
			}
			else
			{
				teamColor.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(
					1 <= summon.Owner.TeamSlot
						? string.Format("Ico_Map_PointPin_{0:D2}", summon.Owner.TeamSlot)
						: "Ico_Map_PointPin_04");
			}

			SetNonPlayerLevel(summon.Status.Level);
			int num = 0;
			SummonData summonData = GameDB.character.GetSummonData(summon.CharacterCode);
			if (0 < summonData.stateEffect)
			{
				SetStateSlot(summonData.stateEffect, simpleSlots[0]);
				simpleSlots[0].Enable();
				num++;
			}

			for (int i = num; i < simpleSlots.Count; i++)
			{
				simpleSlots[i].Disable();
			}

			ResetItemSlots();
			spBar.SetColor(PLAYER_SP_COLOR);
		}


		private void SetDummyHud(LocalDummy dummy)
		{
			playerHead.SetActive(false);
			monsterHead.SetActive(true);
			character.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("SummonProfile_001");
			teamColor.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_PointPin_04");
			SetNonPlayerLevel(dummy.Status.Level);
			for (int i = 0; i < simpleSlots.Count; i++)
			{
				simpleSlots[i].Disable();
			}

			ResetItemSlots();
			spBar.SetColor(NON_PLAYER_SP_COLOR);
		}


		public void SetPlayerLevel(int level, int exp)
		{
			this.level.text = level.ToString();
			int levelUpExp = GameDB.character.GetExpData(level).levelUpExp;
			expGage.fillAmount = exp / (float) levelUpExp;
		}


		public void SetNonPlayerLevel(int level)
		{
			this.level.text = level.ToString();
			expGage.fillAmount = 1f;
		}


		private bool SetSkillSlot(int characterCode, ObjectType objectType, SkillSlotSet skillSlotSet, SimpleSlot slot)
		{
			SkillData skillData = GameDB.skill.GetSkillData(characterCode, objectType, skillSlotSet, 1, 0);
			slot.SetSimpleSlotBehaviour(new SimpleSlotSkillBehaviour(slot, skillData));
			slot.SetIcon(null);
			return skillData != null;
		}


		private void SetStateSlot(int stateEffectCode, SimpleSlot slot)
		{
			CharacterStateData data = GameDB.characterState.GetData(stateEffectCode);
			slot.SetSimpleSlotBehaviour(new SimpleSlotStateBehaviour(slot, data));
			slot.SetIcon(null);
		}


		private void ResetItemSlots()
		{
			foreach (SimpleItemSlot simpleItemSlot in itemSlots)
			{
				simpleItemSlot.Disable();
			}
		}


		public void SetTargetItems(List<Item> items)
		{
			for (int i = 0; i < itemSlots.Count; i++)
			{
				itemSlots[i].SetSimpleSlotBehaviour(null);
				itemSlots[i].SetBackground("Ico_ItemGradebg_01");
				itemSlots[i].Enable();
			}

			itemSlots[0].SetIcon("Ico_Status_Weapon");
			itemSlots[1].SetIcon("Ico_Status_Armor");
			itemSlots[2].SetIcon("Ico_Status_Head");
			itemSlots[3].SetIcon("Ico_Status_Arm");
			itemSlots[4].SetIcon("Ico_Status_Leg");
			itemSlots[5].SetIcon("Ico_Status_Deco");
			if (items != null)
			{
				for (int j = 0; j < items.Count; j++)
				{
					if (items[j] != null)
					{
						if (items[j].ItemData.itemType == ItemType.Weapon)
						{
							itemSlots[0]
								.SetSimpleSlotBehaviour(new SimpleSlotItemBehaviour(itemSlots[0], items[j].ItemData));
							itemSlots[0].SetIcon("Ico_Status_Weapon");
							itemSlots[0].SetBackground();
							itemSlots[0].Enable();
						}
						else if (items[j].ItemData.itemType == ItemType.Armor)
						{
							switch (items[j].ItemData.GetSubTypeData<ItemArmorData>().armorType)
							{
								case ArmorType.Head:
									itemSlots[2]
										.SetSimpleSlotBehaviour(
											new SimpleSlotItemBehaviour(itemSlots[1], items[j].ItemData));
									itemSlots[2].SetIcon("Ico_Status_Head");
									itemSlots[2].SetBackground();
									itemSlots[2].Enable();
									break;
								case ArmorType.Chest:
									itemSlots[1]
										.SetSimpleSlotBehaviour(
											new SimpleSlotItemBehaviour(itemSlots[2], items[j].ItemData));
									itemSlots[1].SetIcon("Ico_Status_Armor");
									itemSlots[1].SetBackground();
									itemSlots[1].Enable();
									break;
								case ArmorType.Arm:
									itemSlots[3]
										.SetSimpleSlotBehaviour(
											new SimpleSlotItemBehaviour(itemSlots[3], items[j].ItemData));
									itemSlots[3].SetIcon("Ico_Status_Arm");
									itemSlots[3].SetBackground();
									itemSlots[3].Enable();
									break;
								case ArmorType.Leg:
									itemSlots[4]
										.SetSimpleSlotBehaviour(
											new SimpleSlotItemBehaviour(itemSlots[4], items[j].ItemData));
									itemSlots[4].SetIcon("Ico_Status_Leg");
									itemSlots[4].SetBackground();
									itemSlots[4].Enable();
									break;
								case ArmorType.Trinket:
									itemSlots[5]
										.SetSimpleSlotBehaviour(
											new SimpleSlotItemBehaviour(itemSlots[5], items[j].ItemData));
									itemSlots[5].SetIcon("Ico_Status_Deco");
									itemSlots[5].SetBackground();
									itemSlots[5].Enable();
									break;
							}
						}
					}
				}
			}
		}


		private void SetDropItems(MonsterData monsterData)
		{
			List<ItemDropGroupData> fixedDropItems = GameDB.monster.GetFixedDropItems(monsterData.dropGroup);
			int num = 0;
			for (int i = 0; i < fixedDropItems.Count; i++)
			{
				if (fixedDropItems[i] != null)
				{
					SimpleItemSlot simpleItemSlot = null;
					if (i < itemSlots.Count)
					{
						simpleItemSlot = itemSlots[i];
					}

					if (!(simpleItemSlot == null))
					{
						ItemData itemData = GameDB.item.FindItemByCode(fixedDropItems[i].itemCode);
						simpleItemSlot.SetSimpleSlotBehaviour(new SimpleSlotItemBehaviour(simpleItemSlot, itemData));
						simpleItemSlot.SetIcon(null);
						simpleItemSlot.SetBackground();
						simpleItemSlot.Enable();
						num++;
					}
				}
			}

			if (num < itemSlots.Count - 1)
			{
				itemSlots[num].SetSimpleSlotBehaviour(new SimpleSlotDropItemInfoBehaviour(itemSlots[num],
					string.Format("MonsterDropRandom/{0}", monsterData.code)));
				itemSlots[num].SetIcon(null);
				itemSlots[num].SetBackground();
				itemSlots[num].Enable();
				num++;
			}

			for (int j = num; j < itemSlots.Count; j++)
			{
				itemSlots[j].Disable();
			}
		}


		public void SetTargetState(List<CharacterStateValue> characterStateList)
		{
			buffTable.Clear();
			debuffTable.Clear();
			for (int i = 0; i < characterStateList.Count; i++)
			{
				EffectType effectType = characterStateList[i].EffectType;
				if (effectType != EffectType.Buff)
				{
					if (effectType == EffectType.Debuff)
					{
						debuffTable.AddState(characterStateList[i]);
					}
				}
				else
				{
					buffTable.AddState(characterStateList[i]);
				}
			}
		}


		public void SetTargetKillScore(int playerKill, int playerKillAssist, int monsterKill)
		{
			this.playerKill.text = playerKill.ToString();
			this.playerKillAssist.text = playerKillAssist.ToString();
			this.monsterKill.text = monsterKill.ToString();
		}


		public void SetTargetStat(UITargetInfoHudStat stat)
		{
			hpBar.SetValue(stat.hp, stat.maxHp);
			spBar.SetValue(stat.sp, stat.maxSp);
			attack.text = Mathf.RoundToInt(stat.attack).ToString();
			defense.text = Mathf.RoundToInt(stat.defense).ToString();
			attackSpeed.text = string.Format("{0:0.##}", stat.attackSpeed);
			critical.text = string.Format("{0:0.##}%", stat.criticalStrikeChance);
			speed.text = string.Format("{0:0.##}", stat.moveSpeed);
			cooldown.text = string.Format("{0:0}%", stat.cooldown * 100f);
		}


		public void SetTargetStatHpBar(int hp, int maxHp)
		{
			hpBar.SetValue(hp, maxHp);
		}


		public void SetTargetStatSpBar(int sp, int maxSp)
		{
			spBar.SetValue(sp, maxSp);
		}


		public void AddTargetState(CharacterStateValue state)
		{
			EffectType effectType = state.EffectType;
			if (effectType == EffectType.Buff)
			{
				buffTable.AddState(state);
				return;
			}

			if (effectType != EffectType.Debuff)
			{
				return;
			}

			debuffTable.AddState(state);
		}


		public void RemoveTargetState(CharacterStateValue state)
		{
			EffectType effectType = state.EffectType;
			if (effectType == EffectType.Buff)
			{
				buffTable.RemoveState(state);
				return;
			}

			if (effectType != EffectType.Debuff)
			{
				return;
			}

			debuffTable.RemoveState(state);
		}


		public void ChangeTargetState(CharacterStateValue state)
		{
			EffectType effectType = state.EffectType;
			if (effectType == EffectType.Buff)
			{
				buffTable.UpdateState(state);
				return;
			}

			if (effectType != EffectType.Debuff)
			{
				return;
			}

			debuffTable.UpdateState(state);
		}
	}
}