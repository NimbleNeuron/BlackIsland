using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SkillHud : BaseUI, ISlotEventListener
	{
		[SerializeField] private GameObject tutorialSquareSkill = default;


		private readonly Dictionary<int, int> weaponSkillLevel = new Dictionary<int, int>();


		private Dictionary<SkillSlotIndex, SkillEvolutionSlot> evolutionSlots = default;


		private GameObject remainSkillPoint = default;


		private Text remainSkillPointText = default;


		private Dictionary<SkillSlotIndex, SkillSlot> skillSlots = default;


		private Dictionary<SkillSlotIndex, bool> skillSlotUseable;


		private Dictionary<SkillSlotIndex, SkillUpgradeSlot> upgradeSlots = default;


		public bool IsSkillLevelUpUiOpen {
			get
			{
				for (SkillSlotIndex skillSlotIndex = SkillSlotIndex.Passive;
					skillSlotIndex <= SkillSlotIndex.WeaponSkill;
					skillSlotIndex++)
				{
					if (upgradeSlots[skillSlotIndex].gameObject.activeSelf)
					{
						return true;
					}
				}

				return false;
			}
		}


		public bool IsSkillEvolutionUiOpen {
			get
			{
				for (SkillSlotIndex skillSlotIndex = SkillSlotIndex.Passive;
					skillSlotIndex <= SkillSlotIndex.WeaponSkill;
					skillSlotIndex++)
				{
					if (evolutionSlots[skillSlotIndex].gameObject.activeSelf)
					{
						return true;
					}
				}

				return false;
			}
		}


		public void OnSlotLeftClick(Slot slot)
		{
			foreach (KeyValuePair<SkillSlotIndex, SkillSlot> keyValuePair in skillSlots)
			{
				if (keyValuePair.Value == slot)
				{
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UseActiveSkillExternal(keyValuePair.Key);
					break;
				}
			}
		}


		public void OnSlotRightClick(Slot slot) { }


		public void OnDropItem(Slot slot, BaseUI draggedUI) { }


		public void OnThrowItem(Slot slot) { }


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot) { }


		public void OnPointerExit(Slot slot) { }


		public void OnSlotDoubleClick(Slot slot) { }


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			skillSlots =
				new Dictionary<SkillSlotIndex, SkillSlot>(SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>
					.Instance);
			skillSlots.Add(SkillSlotIndex.Passive, GameUtil.Bind<SkillSlot>(gameObject, "Passive/P/Main"));
			skillSlots.Add(SkillSlotIndex.Active1, GameUtil.Bind<SkillSlot>(gameObject, "Active/Q/Main"));
			skillSlots.Add(SkillSlotIndex.Active2, GameUtil.Bind<SkillSlot>(gameObject, "Active/W/Main"));
			skillSlots.Add(SkillSlotIndex.Active3, GameUtil.Bind<SkillSlot>(gameObject, "Active/E/Main"));
			skillSlots.Add(SkillSlotIndex.Active4, GameUtil.Bind<SkillSlot>(gameObject, "Active/R/Main"));
			skillSlots.Add(SkillSlotIndex.WeaponSkill, GameUtil.Bind<SkillSlot>(gameObject, "Active/D/Main"));
			upgradeSlots =
				new Dictionary<SkillSlotIndex, SkillUpgradeSlot>(
					SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>.Instance);
			upgradeSlots.Add(SkillSlotIndex.Passive, GameUtil.Bind<SkillUpgradeSlot>(gameObject, "Passive/P/Upgrade"));
			upgradeSlots.Add(SkillSlotIndex.Active1, GameUtil.Bind<SkillUpgradeSlot>(gameObject, "Active/Q/Upgrade"));
			upgradeSlots.Add(SkillSlotIndex.Active2, GameUtil.Bind<SkillUpgradeSlot>(gameObject, "Active/W/Upgrade"));
			upgradeSlots.Add(SkillSlotIndex.Active3, GameUtil.Bind<SkillUpgradeSlot>(gameObject, "Active/E/Upgrade"));
			upgradeSlots.Add(SkillSlotIndex.Active4, GameUtil.Bind<SkillUpgradeSlot>(gameObject, "Active/R/Upgrade"));
			upgradeSlots.Add(SkillSlotIndex.WeaponSkill,
				GameUtil.Bind<SkillUpgradeSlot>(gameObject, "Active/D/Upgrade"));
			evolutionSlots =
				new Dictionary<SkillSlotIndex, SkillEvolutionSlot>(
					SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>.Instance);
			evolutionSlots.Add(SkillSlotIndex.Passive,
				GameUtil.Bind<SkillEvolutionSlot>(gameObject, "Passive/P/Evolution"));
			evolutionSlots.Add(SkillSlotIndex.Active1,
				GameUtil.Bind<SkillEvolutionSlot>(gameObject, "Active/Q/Evolution"));
			evolutionSlots.Add(SkillSlotIndex.Active2,
				GameUtil.Bind<SkillEvolutionSlot>(gameObject, "Active/W/Evolution"));
			evolutionSlots.Add(SkillSlotIndex.Active3,
				GameUtil.Bind<SkillEvolutionSlot>(gameObject, "Active/E/Evolution"));
			evolutionSlots.Add(SkillSlotIndex.Active4,
				GameUtil.Bind<SkillEvolutionSlot>(gameObject, "Active/R/Evolution"));
			evolutionSlots.Add(SkillSlotIndex.WeaponSkill,
				GameUtil.Bind<SkillEvolutionSlot>(gameObject, "Active/D/Evolution"));
			skillSlotUseable =
				new Dictionary<SkillSlotIndex, bool>(SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>
					.Instance);
			skillSlotUseable.Add(SkillSlotIndex.Passive, false);
			skillSlotUseable.Add(SkillSlotIndex.Active1, false);
			skillSlotUseable.Add(SkillSlotIndex.Active2, false);
			skillSlotUseable.Add(SkillSlotIndex.Active3, false);
			skillSlotUseable.Add(SkillSlotIndex.Active4, false);
			skillSlotUseable.Add(SkillSlotIndex.WeaponSkill, false);
			remainSkillPoint = transform.FindRecursively("RemainSkillPoint").gameObject;
			remainSkillPointText = GameUtil.Bind<Text>(remainSkillPoint, "Text");
			foreach (KeyValuePair<SkillSlotIndex, SkillSlot> keyValuePair in skillSlots)
			{
				keyValuePair.Value.SetEventListener(this);
			}
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			InitKeycode();
			MyPlayerContext myPlayer = MonoBehaviourInstance<ClientService>.inst.MyPlayer;
			if (myPlayer == null)
			{
				return;
			}

			myPlayer.CharacterSkill.SetSkillStackValueChangeListener(SkillStackValueChange);
		}


		private void InitKeycode()
		{
			foreach (KeyValuePair<SkillSlotIndex, SkillSlot> keyValuePair in skillSlots)
			{
				bool bgActiveFlag = true;
				GameInputEvent castslotIndexToInputEvent =
					MonoBehaviourInstance<GameInput>.inst.GetCastslotIndexToInputEvent(keyValuePair.Key);
				if (castslotIndexToInputEvent != GameInputEvent.None)
				{
					string text = MonoBehaviourInstance<GameInput>.inst.GetKeyCode(castslotIndexToInputEvent)
						.ToString();
					bool flag = MonoBehaviourInstance<GameInput>.inst.CheckCombination(castslotIndexToInputEvent);
					if (text.Length > 1 || flag)
					{
						text = "";
						bgActiveFlag = false;
					}

					keyValuePair.Value.SetKeyCode(text, bgActiveFlag);
				}
			}
		}


		public void ShowTutorialSquareSkill(bool show)
		{
			tutorialSquareSkill.SetActive(show);
		}


		public void SetCooldown(SkillSlotIndex skillSlotIndex, float cooldown, float maxCooldown, bool enableTimerText)
		{
			MyPlayerContext myPlayer = MonoBehaviourInstance<ClientService>.inst.MyPlayer;
			SkillSlot skillSlot = FindSkillSlot(skillSlotIndex);
			if (skillSlot == null)
			{
				return;
			}

			SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet != null &&
			    myPlayer.CharacterSkill.GetSkillStack(skillSlotSet.Value,
				    myPlayer.Character.GetEquipWeaponMasteryType()) > 0)
			{
				skillSlot.Cooldown.SetCooldown(cooldown, maxCooldown, UICooldown.FillAmountType.STACK_FORWARD,
					enableTimerText);
				skillSlot.SetSpriteColor(Color.white);
				return;
			}

			skillSlot.Cooldown.SetCooldown(cooldown, maxCooldown, UICooldown.FillAmountType.FORWARD, enableTimerText);
			skillSlot.SetSpriteColor(0f < cooldown ? Color.gray : Color.white);
		}


		public void ModifyCooldown(SkillSlotIndex skillSlotIndex, float modifyAmount)
		{
			SkillSlot skillSlot = FindSkillSlot(skillSlotIndex);
			if (skillSlot == null)
			{
				return;
			}

			skillSlot.Cooldown.AddCooldown(modifyAmount);
		}


		public void FinishCooldown(SkillSlotIndex skillSlotIndex)
		{
			SkillSlot skillSlot = FindSkillSlot(skillSlotIndex);
			UICooldown uicooldown = skillSlot != null ? skillSlot.Cooldown : null;
			if (uicooldown == null)
			{
				return;
			}

			if (!uicooldown.RemainCooldown())
			{
				return;
			}

			uicooldown.Finish();
		}


		public void SetStackIntervalTime(SkillSlotSet skillSlotSet, float stackIntervalTime, float stackIntervalTimeMax)
		{
			SetStackIntervalTime(skillSlotSet.SlotSet2Index(), stackIntervalTime, stackIntervalTimeMax);
		}


		public void SetStackIntervalTime(SkillSlotIndex skillSlotIndex, float stackIntervalTime,
			float stackIntervalTimeMax)
		{
			SkillSlot skillSlot = FindSkillSlot(skillSlotIndex);
			if (skillSlot == null)
			{
				return;
			}

			skillSlot.Cooldown.SetStackIntervalTime(stackIntervalTime, stackIntervalTimeMax);
		}


		public void SetSkillSlot(SkillSlotIndex skillSlotIndex, string iconName, string cost, int skillLevel,
			int skillEvolutionLevel, int maxSkillEvolutionLevel)
		{
			skillSlots[skillSlotIndex].SetSkillSlotIndex(skillSlotIndex);
			skillSlots[skillSlotIndex].SetIcon(iconName);
			if (skillSlotIndex != SkillSlotIndex.Passive && skillSlotIndex != SkillSlotIndex.WeaponSkill)
			{
				skillSlots[skillSlotIndex].SetStackText(cost);
			}

			skillSlots[skillSlotIndex].SetSkillLevel(skillLevel);
			skillSlots[skillSlotIndex].SetSkillEvolutionLevel(skillEvolutionLevel, maxSkillEvolutionLevel);
			MasteryType weaponType = MasteryType.None;
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				weaponType = MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.GetEquipWeaponMasteryType();
			}

			SkillSlotSet? skillSlotSet =
				MonoBehaviourInstance<ClientService>.inst.MyPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet != null)
			{
				skillSlots[skillSlotIndex]
					.DrawSkillStack(
						MonoBehaviourInstance<ClientService>.inst.MyPlayer.CharacterSkill.GetSkillStack(
							skillSlotSet.Value, weaponType));
			}
		}


		public void SetSkillUpgradeSlot(SkillSlotIndex skillSlotIndex, Action<SkillSlotIndex> OnClickUpgrade)
		{
			if (!upgradeSlots.ContainsKey(skillSlotIndex))
			{
				return;
			}

			upgradeSlots[skillSlotIndex].SetSkillSlotIndex(skillSlotIndex);
			upgradeSlots[skillSlotIndex].SetOnClickEvent(OnClickUpgrade);
		}


		public void SetSkillEvolutionSlot(SkillSlotIndex skillSlotIndex, Action<SkillSlotIndex> OnClickUpgrade)
		{
			if (!evolutionSlots.ContainsKey(skillSlotIndex))
			{
				return;
			}

			evolutionSlots[skillSlotIndex].SetSkillSlotIndex(skillSlotIndex);
			evolutionSlots[skillSlotIndex].SetOnClickEvent(OnClickUpgrade);
		}


		public void EnableSlot(SkillSlotIndex skillSlotIndex, bool isUsable)
		{
			if (isUsable)
			{
				skillSlots[skillSlotIndex].Enable();
				return;
			}

			skillSlots[skillSlotIndex].Disable();
		}


		public void LockSlot(SkillSlotIndex skillSlotIndex, bool isLock)
		{
			skillSlots[skillSlotIndex].SetLock(isLock);
		}


		public void EmptySkillSlot(SkillSlotIndex skillSlotIndex)
		{
			skillSlots[skillSlotIndex].SetSkillSlotIndex(skillSlotIndex);
			skillSlots[skillSlotIndex].SetIcon("SkillIcon_00000");
			skillSlots[skillSlotIndex].SetSkillLevel(0);
			skillSlots[skillSlotIndex].SetSkillEvolutionLevel(0, 0);
			skillSlots[skillSlotIndex].DrawSkillStack(0);
			skillSlots[skillSlotIndex].Cooldown.Init();
		}


		public void SetRemainSkillPoint(int remainSkillPoint, bool isEvolutionPoint)
		{
			if (remainSkillPoint == 0)
			{
				this.remainSkillPoint.SetActive(false);
				return;
			}

			this.remainSkillPoint.SetActive(true);
			remainSkillPointText.text = isEvolutionPoint
				? string.Format("{0} +{1}", Ln.Get("스킬 진화 포인트"), remainSkillPoint)
				: string.Format("{0} +{1}", Ln.Get("스킬 강화 포인트"), remainSkillPoint);
		}


		public SkillSlot FindSkillSlot(SkillSlotSet skillSlotSet)
		{
			SkillSlotIndex? skillSlotIndex =
				MonoBehaviourInstance<ClientService>.inst.MyPlayer.CharacterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex == null)
			{
				return null;
			}

			return FindSkillSlot(skillSlotIndex.Value);
		}


		public SkillSlot FindSkillSlot(SkillSlotIndex slotIndex)
		{
			if (!skillSlots.ContainsKey(slotIndex))
			{
				return null;
			}

			return skillSlots[slotIndex];
		}


		public SkillUpgradeSlot FindSkillUpgradeSlot(SkillSlotIndex slotIndex)
		{
			if (!upgradeSlots.ContainsKey(slotIndex))
			{
				return null;
			}

			return upgradeSlots[slotIndex];
		}


		public void SetSkillTextUI()
		{
			bool bgActiveFlag = true;
			foreach (KeyValuePair<SkillSlotIndex, SkillSlot> keyValuePair in skillSlots)
			{
				GameInputEvent castslotIndexToInputEvent =
					MonoBehaviourInstance<GameInput>.inst.GetCastslotIndexToInputEvent(keyValuePair.Key);
				if (castslotIndexToInputEvent != GameInputEvent.None)
				{
					KeyCode keyCode = Singleton<LocalSetting>.inst.GetKeyCode(castslotIndexToInputEvent);
					List<KeyCode> list = Singleton<LocalSetting>.inst.GetCombinationKeyCode(castslotIndexToInputEvent)
						.ToList<KeyCode>();
					list.Add(keyCode);
					string text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
					if (text.Length > 1)
					{
						text = "";
						bgActiveFlag = false;
					}

					keyValuePair.Value.SetKeyCode(text, bgActiveFlag);
				}
			}
		}


		public void OnEquipmentUpdate(List<Item> items)
		{
			bool @lock = items.Find(x => x.ItemData.itemType == ItemType.Weapon) == null;
			foreach (KeyValuePair<SkillSlotIndex, SkillSlot> keyValuePair in skillSlots)
			{
				keyValuePair.Value.SetLock(@lock);
			}
		}


		public bool IsExistSkillPoint()
		{
			bool result = false;
			foreach (KeyValuePair<SkillSlotIndex, SkillUpgradeSlot> keyValuePair in upgradeSlots)
			{
				if (keyValuePair.Value.GetEnableState())
				{
					return true;
				}
			}

			foreach (KeyValuePair<SkillSlotIndex, SkillEvolutionSlot> keyValuePair2 in evolutionSlots)
			{
				if (keyValuePair2.Value.GetEnableState())
				{
					return true;
				}
			}

			return result;
		}


		public void UpdateSkillSlot(SkillSlotIndex index, SkillData skillData, int skillLevel, int skillEvolutionLevel,
			int maxSkillEvolutionLevel, Action<SkillSlotIndex> Upgrade, Action<SkillSlotIndex> Evolution, bool isLocked)
		{
			string iconName = 0 < skillEvolutionLevel ? GameDB.skill.GetEvolutionData(skillData).Icon : skillData.Icon;
			SetSkillSlot(index, iconName, 0 < skillData.cost ? skillData.cost.ToString() : "", skillLevel,
				skillEvolutionLevel, maxSkillEvolutionLevel);
			SetSkillUpgradeSlot(index, Upgrade);
			SetSkillEvolutionSlot(index, Evolution);
			LockSlot(index, isLocked);
		}


		public void UpdateSkillSlotCanUse(SkillSlotIndex index, int skillLevel, bool isLocked, bool enoughResource,
			bool isSkillSlotCanUseInSkillScript, bool? checkCooldown)
		{
			bool flag = 0 < skillLevel && enoughResource && isSkillSlotCanUseInSkillScript;
			bool flag2 = flag && checkCooldown != null && checkCooldown.Value;
			if (!skillSlotUseable[index] && flag2)
			{
				skillSlots[index].PlayEffect();
			}

			skillSlotUseable[index] = flag2;
			LockSlot(index, isLocked);
			EnableSlot(index, flag);
		}


		public void UpdateSkillUpgradeSlot(SkillSlotIndex index, bool isHaveSkillUpgradePoint, bool canSkillUpgrade,
			bool canSkillEvolution, bool increasedPoint)
		{
			if (canSkillEvolution)
			{
				upgradeSlots[index].Disable();
				return;
			}

			if (isHaveSkillUpgradePoint)
			{
				upgradeSlots[index].Enable(canSkillUpgrade);
				if (increasedPoint)
				{
					skillSlots[index].PlayUpgradeEffect();
				}
			}
			else
			{
				upgradeSlots[index].Disable();
			}
		}


		public void UpdateSkillEvolutionSlot(SkillSlotIndex index, SkillData skillData, bool isHaveSkillEvolutionPoint,
			bool canSkillEvolution, bool increasedPoint)
		{
			if (skillData == null)
			{
				evolutionSlots[index].Disable();
				return;
			}

			if (!skillData.Evolutionable)
			{
				evolutionSlots[index].Disable();
				return;
			}

			SkillEvolutionData evolutionData = GameDB.skill.GetEvolutionData(skillData);
			if (evolutionData == null)
			{
				evolutionSlots[index].Disable();
				return;
			}

			if (isHaveSkillEvolutionPoint)
			{
				evolutionSlots[index].Enable(canSkillEvolution);
				evolutionSlots[index].SetIcon(evolutionData.Icon);
				if (canSkillEvolution && increasedPoint)
				{
					skillSlots[index].PlayEvolutionEffect();
				}
			}
			else
			{
				evolutionSlots[index].Disable();
			}
		}


		public void UpdateWeaponSkillSlot(SkillData skillData, int skillLevel, int skillEvolutionLevel,
			int maxSkillEvolutionLevel, Action<SkillSlotIndex> Upgrade, Action<SkillSlotIndex> Evolution, bool isLocked)
		{
			if (skillData != null)
			{
				string iconName = 0 < skillEvolutionLevel
					? GameDB.skill.GetEvolutionData(skillData).Icon
					: skillData.Icon;
				SetSkillSlot(SkillSlotIndex.WeaponSkill, iconName, 0 < skillData.cost ? skillData.cost.ToString() : "",
					skillLevel, skillEvolutionLevel, maxSkillEvolutionLevel);
				SetSkillUpgradeSlot(SkillSlotIndex.WeaponSkill, Upgrade);
				SetSkillEvolutionSlot(SkillSlotIndex.WeaponSkill, Evolution);
				weaponSkillLevel[skillData.group] = skillLevel;
			}
			else
			{
				EmptySkillSlot(SkillSlotIndex.WeaponSkill);
			}

			LockSlot(SkillSlotIndex.WeaponSkill, isLocked);
		}


		public void UpdateWeaponSkillSlotCanUse(int skillLevel, bool isLocked, bool enoughResource,
			bool isSkillSlotCanUseInSkillScript, bool? checkCooldown)
		{
			bool flag = 0 < skillLevel && enoughResource && isSkillSlotCanUseInSkillScript;
			bool flag2 = flag && checkCooldown != null && checkCooldown.Value;
			if (!skillSlotUseable[SkillSlotIndex.WeaponSkill] && flag2)
			{
				skillSlots[SkillSlotIndex.WeaponSkill].PlayEffect();
			}

			skillSlotUseable[SkillSlotIndex.WeaponSkill] = flag2;
			EnableSlot(SkillSlotIndex.WeaponSkill, flag);
			LockSlot(SkillSlotIndex.WeaponSkill, isLocked);
		}


		public void SetCastWaitTime(SkillSlotIndex index, float duration, float sequenceCooldown)
		{
			if (sequenceCooldown > 0f)
			{
				SkillSlot skillSlot = FindSkillSlot(index);
				if (skillSlot == null)
				{
					return;
				}

				skillSlot.Cooldown.SetCooldown(sequenceCooldown, sequenceCooldown, UICooldown.FillAmountType.OVERLAP,
					true, delegate
					{
						SkillSlot skillSlot3 = FindSkillSlot(index);
						if (skillSlot3 == null)
						{
							return;
						}

						skillSlot3.Cooldown.SetCooldown(duration, duration, UICooldown.FillAmountType.RADIAL, false);
					});
			}
			else
			{
				SkillSlot skillSlot2 = FindSkillSlot(index);
				if (skillSlot2 == null)
				{
					return;
				}

				skillSlot2.Cooldown.SetCooldown(duration, duration, UICooldown.FillAmountType.RADIAL, false);
			}
		}


		public void SetSlotFillAmountTypeChange(SkillSlotIndex index, UICooldown.FillAmountType fillAmountType)
		{
			SkillSlot skillSlot = FindSkillSlot(index);
			if (skillSlot == null)
			{
				return;
			}

			skillSlot.Cooldown.FillAmountTypeChange(fillAmountType);
		}


		public void OnUpdateEffectState(MyPlayerContext myPlayer) { }


		public void SkillStackValueChange(SkillSlotSet skillSlotSet, MasteryType masteryType, int stack)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill && masteryType !=
				MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.GetEquipWeaponMasteryType())
			{
				return;
			}

			SkillSlot skillSlot = FindSkillSlot(skillSlotSet);
			if (skillSlot != null)
			{
				skillSlot.DrawSkillStack(stack);
				skillSlot.SetSpriteColor(stack <= 0 ? Color.gray : Color.white);
			}
		}
	}
}