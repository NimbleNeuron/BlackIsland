using System;
using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Client.UIModel;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class MyPlayerContext : PlayerContext
	{
		private readonly HashSet<int> interactedObjects;


		private readonly Dictionary<ItemData, float> ItemCooldowns = new Dictionary<ItemData, float>();


		private readonly Dictionary<MasteryType, LocalMastery> masteryMap =
			new Dictionary<MasteryType, LocalMastery>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private int characterExp;


		private List<MasteryValue> masteryValues;


		private int skillPoint;


		public HashSet<int> unvisitAreaCodeList = new HashSet<int>();


		public MyPlayerContext(long userId, string nickname, int startingWeaponCode) : base(userId, nickname,
			startingWeaponCode)
		{
			Inventory = new Inventory();
			PreEquipment = new Equipment();
			interactedObjects = new HashSet<int>();
			foreach (AreaData areaData in GameDB.level.Areas)
			{
				unvisitAreaCodeList.Add(areaData.code);
			}
		}


		public Inventory Inventory { get; }


		public Equipment PreEquipment { get; }


		public int SkillPoint => skillPoint;


		public List<int> StopBulletCoolDownIdList { get; } = new List<int>();


		public List<BulletCooldown> ListCooldowns { get; } = new List<BulletCooldown>();


		public List<LocalPlayerCharacter> TeamMembers { get; } = new List<LocalPlayerCharacter>();


		public override void Init(byte[] playerSnapshot)
		{
			base.Init(playerSnapshot);
			PlayerSnapshot playerSnapshot2 = Serializer.Default.Deserialize<PlayerSnapshot>(playerSnapshot);
			skillPoint = playerSnapshot2.skillPoint;
			masteryValues = playerSnapshot2.masteryValues;
			OnUpdateMastery(masteryValues, false);
			masteryValues.Clear();
			MonoBehaviourInstance<GameUI>.inst.MasteryWindow.SetNickname(nickname);
			MonoBehaviourInstance<GameUI>.inst.StatusHud.Init(Character.CharacterCode, Character.SkinIndex,
				Character.Status.Level, Character.Status.Exp, new UICharacterStat(Character.Stat, Character.Status));
			MonoBehaviourInstance<GameUI>.inst.CombineWindow.Init(Character.CharacterCode);
			OnUpdateStat();
			OnUpdateEquipment(Character.GetEquipments(), true);
			OnUpdateExp(Character.Status.Exp);
			foreach (AreaData areaData in GameDB.level.Areas)
			{
				if (areaData.maskCode == (playerSnapshot2.visitedAreaMaskCodeFlag & areaData.maskCode))
				{
					unvisitAreaCodeList.Remove(areaData.code);
				}
			}

			MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SetSnapShotIgnoreUsers(playerSnapshot2
				.ignoreTargets);
		}


		public void UpdateInternal()
		{
			CharacterSkill.UpdateStackSkillTimer(MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateSkillHudSlotsCanUseSkill();
		}


		public void AddInteractObject(LocalObject target)
		{
			interactedObjects.Add(target.ObjectId);
		}


		public void OnStartSkillCasting(int skillGroup, float castingTime, CastingBarType barType,
			Action<bool> callback)
		{
			if (0f < castingTime)
			{
				MonoBehaviourInstance<GameUI>.inst.Caster.StartCasting(LnUtil.GetSkillName(skillGroup), castingTime,
					barType, callback);
			}
		}


		public void OnStartActionCasting(CastingActionType type, float castTime, int extraParam)
		{
			string skillName;
			if (type.IsCraftAction())
			{
				ItemData itemData = GameDB.item.FindItemByCode(extraParam);
				string param_ = itemData.itemGrade.GetRichText() + LnUtil.GetItemName(itemData.code) + "</color>";
				skillName = Ln.Format("CraftTextForm", param_, LnUtil.GetCastingActionDesc(type));
				MonoBehaviourInstance<GameUI>.inst.CombineWindow.Close();
			}
			else
			{
				skillName = LnUtil.GetCastingActionDesc(type);
			}

			MonoBehaviourInstance<GameUI>.inst.Caster.StartCasting(skillName, castTime, CastingBarType.LeftToRight,
				delegate(bool result)
				{
					if (result)
					{
						SingletonMonoBehaviour<PlayerController>.inst.SetCursorStatus(CursorStatus.Normal);
					}
				});
		}


		public void OnCancelActionCasting()
		{
			MonoBehaviourInstance<GameUI>.inst.Caster.CancelCasting();
			SingletonMonoBehaviour<PlayerController>.inst.ClearNextInteractionObject();
			SingletonMonoBehaviour<PlayerController>.inst.CancelIndicator();
			if (MonoBehaviourInstance<ClientService>.inst.isCrafting)
			{
				foreach (EquipItemSlot equipItemSlot in MonoBehaviourInstance<GameUI>.inst.StatusHud.EquipSlots)
				{
					equipItemSlot.StopFocusFrame();
					equipItemSlot.StopSourceItemFrame();
				}

				foreach (InvenItemSlot invenItemSlot in MonoBehaviourInstance<GameUI>.inst.InventoryHud.InvenSlots)
				{
					invenItemSlot.StopFocusFrame();
					invenItemSlot.StopSourceItemFrame();
				}

				MonoBehaviourInstance<ClientService>.inst.isCrafting = false;
			}
		}


		public void OnUpdateExp(int exp)
		{
			MonoBehaviourInstance<GameUI>.inst.MasteryWindow.OnLevelUpdate(Character.Status.Level, exp);
			MonoBehaviourInstance<GameUI>.inst.StatusHud.SetCharacterLevel(Character.Status.Level, exp);
		}


		public void OnDead()
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateHpBar(0, Character.Stat.MaxHp);
		}


		public void OnKill(LocalCharacter deadCharacter)
		{
			MonoBehaviourInstance<GameUI>.inst.BattleInfoHud.SetPlayerKillCount(Character.Status.PlayerKill);
			MonoBehaviourInstance<GameUI>.inst.BattleInfoHud.SetMonsterKillCount(Character.Status.MonsterKill);
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial && deadCharacter.IsTypeOf<LocalMonster>())
			{
				MonoBehaviourInstance<TutorialController>.inst.MonsterKillTutorial();
			}
		}


		public void OnKillAssist(LocalCharacter deadCharacter)
		{
			MonoBehaviourInstance<GameUI>.inst.BattleInfoHud
				.SetPlayerKillAssistCount(Character.Status.PlayerKillAssist);
		}


		public void OnUpdateSurvivableTime(float survivalTime)
		{
			MonoBehaviourInstance<GameUI>.inst.RestrictedArea.UpdateSurvivableTime(survivalTime);
		}


		public bool HasMastery(MasteryType masteryType)
		{
			return masteryMap.ContainsKey(masteryType);
		}


		public bool HasItem(int itemCode)
		{
			return Inventory.HasItem(itemCode) || Character.Equipment.HasItem(itemCode);
		}


		public bool IsCombinableWithMaterial(ItemData targetItem, ItemData material)
		{
			return (targetItem.makeMaterial1 == material.code || HasItem(targetItem.makeMaterial1)) &&
			       (targetItem.makeMaterial2 == material.code || HasItem(targetItem.makeMaterial2));
		}


		public bool IsInteractedObject(LocalObject target)
		{
			return interactedObjects.Contains(target.ObjectId);
		}


		public void StopBulletCooldownIds()
		{
			using (List<int>.Enumerator enumerator = StopBulletCoolDownIdList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int itemId = enumerator.Current;
					if (ListCooldowns.Exists(x => x.WeaponItem.id == itemId))
					{
						MonoBehaviourInstance<GameUI>.inst.InventoryHud.StopBulletCooldown(itemId);
						MonoBehaviourInstance<GameUI>.inst.StatusHud.StopBulletCooldown(itemId);
					}
				}
			}

			StopBulletCoolDownIdList.Clear();
		}


		public void OnUpdateInventory(List<InvenItem> updates, UpdateInventoryType updateType)
		{
			foreach (InvenItem invenItem in updates)
			{
				Inventory.UpdateItem(invenItem);
			}

			List<Item> items = Inventory.GetItems();
			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateInventory(items, Inventory.CreateItemIndexes());
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.UpdateInventoryTutorial(items);
				MonoBehaviourInstance<TutorialController>.inst.CreateDiscardItemTutorial(items.Count);
			}

			switch (updateType)
			{
				case UpdateInventoryType.DropItem:
					Singleton<SoundControl>.inst.PlayUISound("DropItem",
						ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
					return;
				case UpdateInventoryType.TakeItem:
				case UpdateInventoryType.PickupItem:
				case UpdateInventoryType.MakeItem:
				case UpdateInventoryType.ResourceGather:
					Singleton<SoundControl>.inst.PlayUISound("PickUpItem",
						ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
					break;
				case UpdateInventoryType.InsertItem:
					break;
				default:
					return;
			}
		}


		public void OnUpdateStat()
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateHpBar(Character.Status.Hp, Character.Stat.MaxHp);
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateSpBar(Character.Status.Sp, Character.Stat.MaxSp);
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateEpBar(Character.Status.ExtraPoint,
				Character.Stat.MaxExtraPoint);
			MonoBehaviourInstance<GameUI>.inst.StatusHud.SetCharacterLevel(Character.Status.Level,
				Character.Status.Exp);
			MonoBehaviourInstance<GameUI>.inst.StatusHud.OnUpdateStat(
				new UICharacterStat(Character.Stat, Character.Status), Character.IsInCombat);
		}


		public void OnUpdateMastery(List<MasteryValue> updates, bool showExp)
		{
			for (int i = 0; i < updates.Count; i++)
			{
				MasteryValue masteryValue = updates[i];
				if (masteryMap.ContainsKey(masteryValue.MasteryType))
				{
					LocalMastery localMastery = masteryMap[masteryValue.MasteryType];
					localMastery.SetExp(masteryValue.MasteryExp);
					localMastery.SetLevel(masteryValue.MasteryLevel);
				}
				else
				{
					masteryMap.Add(masteryValue.MasteryType,
						new LocalMastery(masteryValue.MasteryType, masteryValue.MasteryLevel, masteryValue.MasteryExp,
							masteryValue.WeaponSkillPoint));
				}

				UISystem.Action(new UpdateMastery
				{
					masteryType = masteryValue.MasteryType,
					level = masteryValue.MasteryLevel,
					exp = masteryValue.MasteryExp,
					maxExp = (int) masteryMap[masteryValue.MasteryType].MaxExp
				});
				if (showExp)
				{
					MonoBehaviourInstance<GameUI>.inst.MasteryExpHud.ShowExp(masteryValue.MasteryType,
						masteryValue.AddExp);
				}

				UpdateSkillPoint(masteryValue.MasteryType, masteryValue.WeaponSkillPoint);
			}

			if (HasSightRange(updates))
			{
				OnUpdateCameraFV();
			}
		}


		public void OnMasteryLevelUp(MasteryType masteryType)
		{
			Character.OnMasteryLevelUp(masteryType);
		}


		public bool IsEquipableWeapon(ItemData itemData)
		{
			if (itemData.itemType == ItemType.Weapon)
			{
				ItemWeaponData subTypeData = itemData.GetSubTypeData<ItemWeaponData>();
				foreach (KeyValuePair<MasteryType, LocalMastery> keyValuePair in masteryMap)
				{
					if (keyValuePair.Key == subTypeData.GetMasteryType())
					{
						return true;
					}
				}

				return false;
			}

			return false;
		}


		public void OnUpdateCameraFV()
		{
			float num = 0f;
			CharacterData characterData = GameDB.character.GetCharacterData(Character.CharacterCode);
			num += characterData.sightRange;
			foreach (KeyValuePair<MasteryType, LocalMastery> keyValuePair in masteryMap)
			{
				MasteryLevelData masteryLevelData =
					GameDB.mastery.GetMasteryLevelData(keyValuePair.Key, keyValuePair.Value.Level);
				if (masteryLevelData.option1.Equals(StatType.SightRange))
				{
					num += masteryLevelData.optionValue1;
				}

				if (masteryLevelData.option2.Equals(StatType.SightRange))
				{
					num += masteryLevelData.optionValue2;
				}
			}

			num += Character.Equipment.GetStatValue(StatType.SightRange);
			MonoBehaviourInstance<MobaCamera>.inst.SetCameraFV(MonoBehaviourInstance<MobaCamera>.inst.SightToFV(num));
		}


		public bool HasSightRange(List<MasteryValue> updates)
		{
			foreach (MasteryValue masteryValue in updates)
			{
				MasteryLevelData masteryLevelData =
					GameDB.mastery.GetMasteryLevelData(masteryValue.MasteryType, masteryValue.MasteryLevel);
				if (masteryLevelData.option1.Equals(StatType.SightRange))
				{
					return true;
				}

				if (masteryLevelData.option2.Equals(StatType.SightRange))
				{
					return true;
				}
			}

			return false;
		}


		public void OnUpdateEquipment(List<Item> updates, bool updateWeapon)
		{
			List<Item> equipments = Character.GetEquipments();
			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateEquipment(equipments, updates);
			if (updateWeapon)
			{
				SingletonMonoBehaviour<PlayerController>.inst.OnUpdateWeaponEquip(equipments);
			}

			Equipment equipment = PreEquipment;
			if (equipment == null)
			{
				return;
			}

			equipment.UpdateEquipment(updates);
		}


		public void OnAddEffectState(CharacterStateValue state)
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.AddState(state);
			MonoBehaviourInstance<GameUI>.inst.SkillHud.OnUpdateEffectState(this);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.SetCrowdControl(state, true);
		}


		public void OnRemoveEffectState(CharacterStateValue state)
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.RemoveState(state);
			MonoBehaviourInstance<GameUI>.inst.SkillHud.OnUpdateEffectState(this);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.SetCrowdControl(state, false);
		}


		public void OnRemoveStateOnDead()
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.RemoveStateOnDead();
			MonoBehaviourInstance<GameUI>.inst.SkillHud.OnUpdateEffectState(this);
		}


		public void OnRemoveAllEffectState()
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.RemoveAllState();
			MonoBehaviourInstance<GameUI>.inst.SkillHud.OnUpdateEffectState(this);
		}


		public void OnUpdateEffectState(CharacterStateValue state)
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateState(state);
			MonoBehaviourInstance<GameUI>.inst.SkillHud.OnUpdateEffectState(this);
		}


		public Dictionary<string, string> GetEquipmentLogInfo()
		{
			return Character.Equipment.GetEquipmentLogInfo();
		}


		public MasteryLogInfo GetMasteryLogInfo()
		{
			bool flag = false;
			string bestWeapon = "";
			int num = 0;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<MasteryType, LocalMastery> keyValuePair in masteryMap)
			{
				string text = keyValuePair.Key.ToString();
				dictionary.Add(text, keyValuePair.Value.Level);
				if (keyValuePair.Key.IsWeaponMastery())
				{
					if (keyValuePair.Value.Level > num)
					{
						num = keyValuePair.Value.Level;
						bestWeapon = text;
						flag = false;
					}
					else if (keyValuePair.Value.Level == num)
					{
						flag = true;
					}
				}
			}

			if (flag)
			{
				bestWeapon = "Multi";
			}

			if (num == 1)
			{
				bestWeapon = "None";
			}

			return new MasteryLogInfo
			{
				mastery = dictionary,
				bestWeapon = bestWeapon,
				bestWeaponLevel = num
			};
		}


		public bool CanSkillUpgrade(SkillSlotIndex skillSlotIndex)
		{
			if (!IsHaveSkillUpgradePoint(skillSlotIndex))
			{
				return false;
			}

			SkillSlotSet? skillSlotSet = CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return false;
			}

			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				int level = 0 < Character.GetSkillLevel(skillSlotIndex)
					? Character.GetSkillLevel(skillSlotIndex) + 1
					: 1;
				if (GameDB.skill.GetSkillData(Character.GetEquipWeaponMasteryType(), level,
					GetLocalSkillSequence(skillSlotSet.Value)) != null)
				{
					return true;
				}
			}
			else
			{
				if (skillSlotIndex == SkillSlotIndex.SpecialSkill)
				{
					return false;
				}

				int level2 = 0 < Character.GetSkillLevel(skillSlotIndex)
					? Character.GetSkillLevel(skillSlotIndex) + 1
					: 1;
				List<SkillData> characterSkills = GameDB.skill.GetCharacterSkills(Character.CharacterCode,
					ObjectType.PlayerCharacter, skillSlotSet.Value, level2);
				if (0 < characterSkills.Count && characterSkills[0].activeLevel <= Character.Status.Level)
				{
					return true;
				}
			}

			return false;
		}


		public bool CanSkillEvolution(SkillSlotIndex skillSlotIndex)
		{
			SkillData skillData = Character.GetSkillData(skillSlotIndex);
			if (skillData == null)
			{
				return false;
			}

			if (!skillData.Evolutionable)
			{
				return false;
			}

			SkillEvolutionData evolutionData = GameDB.skill.GetEvolutionData(skillData);
			return evolutionData != null &&
			       CharacterSkill.CanSkillEvolution(skillSlotIndex, evolutionData,
				       Character.GetEquipWeaponMasteryType());
		}


		public void OnStartMove()
		{
			SingletonMonoBehaviour<PlayerController>.inst.CloseBox();
			MonoBehaviourInstance<GameUI>.inst.HyperloopWindow.Close();
		}


		public void OnStop()
		{
			SingletonMonoBehaviour<PlayerController>.inst.ClearNextInteractionObject();
		}


		public void UpdateSkillPoint(int skillPoint)
		{
			bool increasedPoint = this.skillPoint < skillPoint;
			this.skillPoint = skillPoint;
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnChangedSkillPoint(increasedPoint);
		}


		public void UpdateSkillPoint(MasteryType masteryType, int skillPoint)
		{
			if (!masteryType.IsWeaponMastery())
			{
				return;
			}

			LocalMastery localMastery = masteryMap[masteryType];
			bool increasedPoint = localMastery.WeaponSkillPoint < skillPoint;
			localMastery.SetWeaponSkillPoint(skillPoint);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnChangedSkillPoint(increasedPoint);
		}


		public override void UpdateSkillEvolutionPoint(SkillEvolutionPointType pointType, int evolutionPoint)
		{
			bool flag = CharacterSkill.SkillEvolution.GetPoint(pointType) < evolutionPoint;
			base.UpdateSkillEvolutionPoint(pointType, evolutionPoint);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnChangedEvolutionSkillPoint(flag);
			if (flag)
			{
				Singleton<SoundControl>.inst.PlayUISound("SkillUp",
					ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
			}
		}


		public bool IsHaveSkillUpgradePoint()
		{
			return Internal_IsHaveSkillUpgradePoint() ||
			       Internal_IsHaveSkillUpgradePoint(Character.GetEquipWeaponMasteryType());
		}


		public bool IsHaveSkillUpgradePoint(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				return Internal_IsHaveSkillUpgradePoint(Character.GetEquipWeaponMasteryType());
			}

			return skillSlotIndex != SkillSlotIndex.SpecialSkill && Internal_IsHaveSkillUpgradePoint();
		}


		private bool Internal_IsHaveSkillUpgradePoint()
		{
			return 0 < skillPoint;
		}


		private bool Internal_IsHaveSkillUpgradePoint(MasteryType masteryType)
		{
			return masteryMap.ContainsKey(masteryType) && 0 < masteryMap[masteryType].WeaponSkillPoint;
		}


		public override void UpdateSkills(Dictionary<SkillSlotIndex, int> characterSkillLevels, int skillPoint)
		{
			base.UpdateSkills(characterSkillLevels, skillPoint);
			if (skillPoint < 0)
			{
				return;
			}

			this.skillPoint = skillPoint;
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateAllSkillHud();
		}


		public override void UpgradeSkill(SkillSlotIndex skillSlotIndex)
		{
			base.UpgradeSkill(skillSlotIndex);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnSkillUpgrade(skillSlotIndex);
		}


		public override void EvolutionSkill(SkillSlotIndex skillSlotIndex)
		{
			base.EvolutionSkill(skillSlotIndex);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnSkillEvolution(skillSlotIndex);
		}


		public override void ResetSkillCooldown()
		{
			base.ResetSkillCooldown();
			for (SkillSlotIndex skillSlotIndex = SkillSlotIndex.Passive;
				skillSlotIndex <= SkillSlotIndex.SpecialSkill;
				skillSlotIndex++)
			{
				SkillSlot skillSlot = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotIndex);
				if (!(skillSlot == null))
				{
					skillSlot.Cooldown.Init();
					skillSlot.Cooldown.InitIntervalTime();
				}
			}
		}


		public override void StartSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, float cooldown)
		{
			base.StartSkillCooldown(skillSlotSet, masteryType, cooldown);
			SkillSlotIndex? skillSlotIndex = CharacterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex == null)
			{
				return;
			}

			float cooldown2;
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				cooldown2 = CharacterSkill.GetCooldown(Character.GetEquipWeaponMasteryType(),
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
			}
			else
			{
				cooldown2 = CharacterSkill.GetCooldown(skillSlotSet,
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
			}

			MonoBehaviourInstance<GameUI>.inst.SkillHud.SetCooldown(skillSlotIndex.Value, cooldown2, cooldown2,
				1f <= cooldown2);
		}


		public override bool SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
		{
			SkillSlotSet? skillSlotSet2 = GetSkillSlotSet(skillSlotIndex);
			bool flag = base.SwitchSkillSet(skillSlotIndex, skillSlotSet);
			if (flag && skillSlotIndex != SkillSlotIndex.Attack)
			{
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateSkillHudSlot(skillSlotIndex);
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateEvolutionSkilllSlots(false);
				float cooldown = CharacterSkill.GetCooldown(skillSlotSet,
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
				if (cooldown <= 0f)
				{
					MonoBehaviourInstance<GameUI>.inst.SkillHud.FinishCooldown(skillSlotIndex);
					SkillData skillData = GameDB.skill.GetSkillData(Character.CharacterCode, ObjectType.PlayerCharacter,
						skillSlotSet, Character.GetSkillLevel(skillSlotIndex),
						CharacterSkill.GetSkillSequence(skillSlotSet));
					if (skillData != null && 0f < skillData.CastWaitTime)
					{
						MonoBehaviourInstance<GameUI>.inst.SkillHud.SetCastWaitTime(skillSlotIndex,
							skillData.CastWaitTime, skillData.SequenceCooldown);
					}
				}
				else
				{
					float maxCooldown = CharacterSkill.GetMaxCooldown(skillSlotSet);
					MonoBehaviourInstance<GameUI>.inst.SkillHud.SetCooldown(skillSlotIndex, cooldown, maxCooldown,
						1f <= cooldown);
				}

				SkillSlot skillSlot = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotSet);
				if (skillSlot != null)
				{
					skillSlot.Cooldown.Hold(Character.IsHoldCooldown(skillSlotSet));
				}
			}

			if (skillSlotSet2 != null)
			{
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.SwitchSkillSet(skillSlotIndex,
					skillSlotSet2.Value);
			}

			return flag;
		}


		public override void ModifySkillCooldown(SkillSlotSet skillSlotSet, float modifyValue)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				CharacterSkill.ModifyCooldown(Character.GetEquipWeaponMasteryType(),
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime, modifyValue);
			}
			else
			{
				CharacterSkill.ModifyCooldown(skillSlotSet, MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime,
					modifyValue);
			}

			SkillSlotIndex? skillSlotIndex = CharacterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex != null)
			{
				MonoBehaviourInstance<GameUI>.inst.SkillHud.ModifyCooldown(skillSlotIndex.Value, modifyValue);
			}
		}


		public override void HoldSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, bool isHold)
		{
			base.HoldSkillCooldown(skillSlotSet, masteryType, isHold);
			if (isHold)
			{
				SkillSlot skillSlot = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotSet);
				if (skillSlot == null)
				{
					return;
				}

				skillSlot.Cooldown.Hold(true);
			}
			else
			{
				SkillSlot skillSlot2 = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotSet);
				if (skillSlot2 == null)
				{
					return;
				}

				skillSlot2.Cooldown.Hold(false);
			}
		}


		public override void ResetSkillSequence(SkillSlotSet skillSlotSet)
		{
			base.ResetSkillSequence(skillSlotSet);
			SkillSlotIndex? skillSlotIndex = CharacterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex == null)
			{
				return;
			}

			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateSkillHudSlot(skillSlotIndex.Value);
		}


		public override void SetSkillSequence(SkillSlotSet skillSlotSet, MasteryType masteryType, int sequence,
			float duration, float sequenceCooldown)
		{
			base.SetSkillSequence(skillSlotSet, masteryType, sequence, duration, sequenceCooldown);
			SkillSlotIndex? skillSlotIndex = CharacterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex != null)
			{
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateSkillHudSlot(skillSlotIndex.Value);
				MonoBehaviourInstance<GameUI>.inst.SkillHud.SetCastWaitTime(skillSlotIndex.Value, duration,
					sequenceCooldown);
			}
		}


		public void OnUpdatePath(List<Vector3> corners)
		{
			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.SetCorners(corners);
			MonoBehaviourInstance<GameUI>.inst.CombineWindow.UIMap.SetCorners(corners);
			MonoBehaviourInstance<GameUI>.inst.MapWindow.UIMap.SetCorners(corners);
		}


		public void SetItemCooldown(ItemData itemData, float cooldown)
		{
			ItemCooldowns[itemData] = MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime + cooldown;
			MonoBehaviourInstance<GameUI>.inst.InventoryHud.UpdateCooldown(ItemCooldowns);
		}


		public void RemoveItemCooldown(ItemData itemData)
		{
			ItemCooldowns.Remove(itemData);
			MonoBehaviourInstance<GameUI>.inst.InventoryHud.UpdateCooldown(ItemCooldowns);
		}


		public void OnStartConcentration(SkillData skillData)
		{
			if (skillData.ConcentrationTime > 0.0 &&
			    Character.FloatingUi != null)
			{
				Character.FloatingUi.ResetName(Ln.Get("Concentration"));
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnSkillFinishAction +=
					OnSkillFinish;
			}

			MonoBehaviourInstance<GameUI>.inst.Caster.StartConcentration(LnUtil.GetSkillName(skillData.group),
				skillData.ConcentrationTime, skillData.ConcentrationBarType, null);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnStartConcentration();
			Character.LocalSkillPlayer.StartConcentration(skillData.SkillId);

			void OnSkillFinish(LocalPlayerCharacter playerCharacter, SkillId skillId)
			{
				if (skillId != skillData.SkillId || playerCharacter == null ||
				    Character == null ||
				    !playerCharacter.ObjectId.Equals(Character.ObjectId))
				{
					return;
				}

				Character.FloatingUi.ResetStateName();
				// ISSUE: method pointer
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnSkillFinishAction -= OnSkillFinish;
				// new Action<LocalPlayerCharacter, SkillId>((object) this, skillId));
			}

			// co: dotPeek
			// MyPlayerContext.<>c__DisplayClass80_0 CS$<>8__locals1 = new MyPlayerContext.<>c__DisplayClass80_0();
			// CS$<>8__locals1.skillData = skillData;
			// CS$<>8__locals1.<>4__this = this;
			// if (CS$<>8__locals1.skillData.ConcentrationTime > 0f && base.Character.FloatingUi != null)
			// {
			// 	base.Character.FloatingUi.ResetName(Ln.Get("Concentration"));
			// 	SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnSkillFinishAction += CS$<>8__locals1.<OnStartConcentration>g__OnSkillFinish|0;
			// }
			// MonoBehaviourInstance<GameUI>.inst.Caster.StartConcentration(LnUtil.GetSkillName(CS$<>8__locals1.skillData.group), CS$<>8__locals1.skillData.ConcentrationTime, CS$<>8__locals1.skillData.ConcentrationBarType, null);
			// SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnStartConcentration();
			// base.Character.LocalSkillPlayer.StartConcentration(CS$<>8__locals1.skillData.SkillId);
		}


		public void OnEndConcentration(SkillData skillData, bool cancel)
		{
			if (!cancel && 0f < skillData.CastingTime2)
			{
				OnStartSkillCasting(skillData.group, skillData.CastingTime2, skillData.CastingBarType2, null);
			}
			else
			{
				MonoBehaviourInstance<GameUI>.inst.Caster.CancelCasting();
			}

			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnEndConcentration(skillData);
		}


		public void OnUpdateHpSp()
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateHpBar(Character.Status.Hp, Character.Stat.MaxHp);
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateSpBar(Character.Status.Sp, Character.Stat.MaxSp);
		}


		public void OnUpdateEp()
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateEpBar(Character.Status.ExtraPoint,
				Character.Stat.MaxExtraPoint);
		}


		public void OnDamaged(int damage)
		{
			int hp = Character.Status.Hp;
			int maxHp = Character.Stat.MaxHp;
			if (MonoBehaviourInstance<GameUI>.inst.BloodFx.CurrentAnimation != BloodEffect.AnimationType.Restricted &&
			    hp / (float) maxHp <= 0.35f)
			{
				if (damage > hp * 0.1f)
				{
					MonoBehaviourInstance<GameUI>.inst.BloodFx.Play(BloodEffect.AnimationType.HeavyDamaged);
					return;
				}

				if (MonoBehaviourInstance<GameUI>.inst.BloodFx.CurrentAnimation !=
				    BloodEffect.AnimationType.HeavyDamaged)
				{
					MonoBehaviourInstance<GameUI>.inst.BloodFx.Play(BloodEffect.AnimationType.Damaged);
				}
			}
			else if (MonoBehaviourInstance<GameUI>.inst.BloodFx.CurrentAnimation !=
			         BloodEffect.AnimationType.Restricted && hp / (float) maxHp <= 0.9f &&
			         MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.CreateRestingTutorial();
			}
		}


		public void ClearTeam()
		{
			TeamMembers.Clear();
		}


		public void AddTeamMember(LocalPlayerCharacter player)
		{
			TeamMembers.Add(player);
		}


		public HostileType GetHostileType(LocalCharacter target)
		{
			return Character.HostileAgent.GetHostileType(target.HostileAgent);
		}


		public void MakeSourceItemBulletCooldownClean()
		{
			if (ListCooldowns.Count == 0)
			{
				return;
			}

			foreach (int num in SingletonMonoBehaviour<PlayerController>.inst.MakeSourceItems)
			{
				EquipItemSlot weaponEquipItemSlot =
					MonoBehaviourInstance<GameUI>.inst.StatusHud.GetWeaponEquipItemSlot();
				if (weaponEquipItemSlot != null)
				{
					Item item = weaponEquipItemSlot.GetItem();
					if (item != null && item.id == num)
					{
						weaponEquipItemSlot.StopBulletCooldown();
						FinishBulletCooldown(num);
						continue;
					}
				}

				if (MonoBehaviourInstance<GameUI>.inst.InventoryHud.GetInvenItemSlot(num) != null)
				{
					MonoBehaviourInstance<GameUI>.inst.InventoryHud.StopBulletCooldown(num);
					FinishBulletCooldown(num);
				}
			}
		}


		public void StartBulletCooldown(Item weaponItem, bool updateInven)
		{
			BulletCooldown bulletCooldown = ListCooldowns.Find(x => x.WeaponItem.id == weaponItem.id);
			if (bulletCooldown == null)
			{
				BulletCooldown bulletCooldown2 = new BulletCooldown(Character, weaponItem);
				bulletCooldown2.addBullet += AddBullet;
				bulletCooldown2.stopCooldown += FinishBulletCooldown;
				bulletCooldown2.StartBulletCooldown();
				ListCooldowns.Add(bulletCooldown2);
				return;
			}

			bulletCooldown.MergeItem(weaponItem);
			if (updateInven)
			{
				bulletCooldown.UpdateInvenItemCooldown();
				return;
			}

			bulletCooldown.UpdateEquipItemCooldown();
		}


		private void AddBullet(int itemId, ItemMadeType madeType, int addBullet)
		{
			Item invenItem = Inventory.FindItemById(itemId, madeType);
			if (invenItem != null)
			{
				invenItem.AddBullet(addBullet);
				MonoBehaviourInstance<GameUI>.inst.InventoryHud.GetInvenItemSlot(invenItem)
					.SetBulletStackText(invenItem.itemCode, invenItem.Bullet);
				ListCooldowns.Find(x => x.WeaponItem.id == invenItem.id).MergeItem(invenItem);
				return;
			}

			Item weaponItem = Character.GetWeapon();
			if (weaponItem != null)
			{
				weaponItem.AddBullet(addBullet);
				MonoBehaviourInstance<GameUI>.inst.StatusHud.GetStatusItemSlot(weaponItem)
					.SetBulletStackText(weaponItem.itemCode, weaponItem.Bullet);
				Character.FloatingUi.UpdateThrowAmmo(weaponItem.WeaponTypeInfoData.type, weaponItem.Bullet);
				ListCooldowns.Find(x => x.WeaponItem.id == weaponItem.id).MergeItem(weaponItem);
			}
		}


		public void FinishBulletCooldown(int itemId)
		{
			BulletCooldown bulletCooldown = ListCooldowns.Find(x => x.WeaponItem.id == itemId);
			if (bulletCooldown != null)
			{
				bulletCooldown.FinishBulletCooldown();
				ListCooldowns.Remove(bulletCooldown);
			}
		}


		public void SetBulletRemainCooldown(Item item, float remainCooldown)
		{
			BulletCooldown bulletCooldown = ListCooldowns.Find(x => x.WeaponItem.id == item.id);
			if (bulletCooldown != null)
			{
				item.RemainCoolTime = remainCooldown;
				bulletCooldown.SetRemainCooldown(remainCooldown);
			}
		}


		public void AddVisitedArea(int areaMaskCode)
		{
			foreach (AreaData areaData in GameDB.level.Areas)
			{
				if (areaData.maskCode == areaMaskCode)
				{
					unvisitAreaCodeList.Remove(areaData.code);
					MonoBehaviourInstance<GameUI>.inst.MapWindow.UIMap.RefrashAreaText(areaData.code);
					MonoBehaviourInstance<GameUI>.inst.CombineWindow.UIMap.RefrashAreaText(areaData.code);
					MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.RefrashAreaText(areaData.code);
					MonoBehaviourInstance<GameUI>.inst.HyperloopWindow.UIMap.RefrashAreaText(areaData.code);
					break;
				}
			}
		}


		public bool IsValid()
		{
			if (userId <= 0L)
			{
				Debug.LogError($"UserId : {userId}!");
				return false;
			}

			if (Character == null)
			{
				return false;
			}

			if (Character.ObjectId <= 0)
			{
				return false;
			}
			
			return true;
		}


		public void StartSkillIndicatorHide(string guideline)
		{
			SingletonMonoBehaviour<PlayerController>.inst.StartSkillIndicatorHide(guideline);
		}
	}
}