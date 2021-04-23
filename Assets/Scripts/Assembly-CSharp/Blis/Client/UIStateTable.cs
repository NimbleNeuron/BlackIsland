using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class UIStateTable : BaseUI
	{
		private readonly List<StateSlot> slots = new List<StateSlot>();


		private SlotType slotType;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GetComponentsInChildren<StateSlot>(true, slots);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			foreach (StateSlot stateSlot in slots)
			{
				if (stateSlot.CharacterState == null)
				{
					stateSlot.gameObject.SetActive(false);
				}
			}
		}


		public void SetSlotType(SlotType slotType)
		{
			this.slotType = slotType;
		}


		public void AddState(CharacterStateValue characterState)
		{
			if (!GameDB.characterState.GetGroupData(characterState.Group).uiVisible)
			{
				return;
			}

			StateSlot stateSlot = GetExistSameSlot(characterState);
			if (stateSlot == null)
			{
				stateSlot = GetEmptySlot();
				if (stateSlot != null)
				{
					SetStateSlot(characterState, stateSlot);
				}
			}
			else
			{
				SetStateSlot(characterState, stateSlot);
			}
		}


		private void SetStateSlot(CharacterStateValue characterState, StateSlot stateSlot)
		{
			stateSlot.ResetSlot();
			stateSlot.SetBackground(GetBackground(characterState.EffectType));
			stateSlot.gameObject.SetActive(true);
			stateSlot.SetState(characterState);
			stateSlot.SetSlotType(slotType);
			CharacterStateGroupData groupData = GameDB.characterState.GetGroupData(characterState.Group);
			if (characterState.StackCount == 0 && groupData.defaultStack != 0)
			{
				stateSlot.SetSpriteColor(Color.gray);
			}

			if (characterState.ReserveCount + 1 > 1)
			{
				stateSlot.SetStackText((characterState.ReserveCount + 1).ToString());
			}
			else if (characterState.StackCount > 1 || groupData.alwaysShowStack)
			{
				stateSlot.SetStackText(characterState.StackCount.ToString());
			}

			if (groupData != null)
			{
				stateSlot.SetSprite(groupData.GetSprite());
				StateUIBehaviourType uiBehaviourType = groupData.uiBehaviourType;
				if (uiBehaviourType != StateUIBehaviourType.Nothing)
				{
					if (uiBehaviourType == StateUIBehaviourType.ActivateOnMaxStack)
					{
						stateSlot.SetSpriteColor(characterState.MaxStack <= characterState.StackCount
							? Color.white
							: Color.gray);
					}
				}
				else
				{
					stateSlot.SetSpriteColor(Color.white);
				}
			}

			float num = 0f;
			if (characterState.DurationPauseEndTime == 0f)
			{
				num = MonoBehaviourInstance<ClientService>.inst.CurrentServerFrameTime - characterState.createdTime;
			}
			else
			{
				stateSlot.Cooldown.SetPauseTime(characterState.DurationPauseEndTime);
			}

			float cooldown = characterState.Duration - num;
			bool usingTimer = characterState.OriginalDuration < 0f;
			stateSlot.Cooldown.SetCooldown(cooldown, characterState.OriginalDuration, UICooldown.FillAmountType.FORWARD,
				usingTimer);
		}


		private StateSlot GetExistSameSlot(CharacterStateValue characterState)
		{
			return slots.Find(x =>
				x.CharacterState != null && x.CharacterState.Group == characterState.Group &&
				x.CharacterState.CasterId == characterState.CasterId);
		}


		private StateSlot GetEmptySlot()
		{
			return slots.Find(x => x.CharacterState == null);
		}


		public void RemoveState(CharacterStateValue characterState)
		{
			StateSlot existSameSlot = GetExistSameSlot(characterState);
			if (existSameSlot != null)
			{
				existSameSlot.SetState(null);
				existSameSlot.ResetSlot();
				existSameSlot.gameObject.SetActive(false);
			}
		}


		public void UpdateState(CharacterStateValue characterState)
		{
			StateSlot existSameSlot = GetExistSameSlot(characterState);
			if (existSameSlot == null)
			{
				return;
			}

			SetStateSlot(characterState, existSameSlot);
		}


		public void RemoveStateOnDead()
		{
			int i = slots.Count - 1;
			while (i >= 0)
			{
				StateSlot stateSlot = slots[i];
				if (stateSlot.CharacterState == null)
				{
					goto IL_46;
				}

				CharacterStateGroupData groupData = GameDB.characterState.GetGroupData(stateSlot.CharacterState.Group);
				if (groupData == null || groupData.removeOnDead)
				{
					goto IL_46;
				}

				IL_5F:
				i--;
				continue;
				IL_46:
				stateSlot.SetState(null);
				stateSlot.ResetSlot();
				stateSlot.gameObject.SetActive(false);
				goto IL_5F;
			}
		}


		public void Clear()
		{
			slots.ForEach(delegate(StateSlot x)
			{
				x.SetState(null);
				x.ResetSlot();
				x.gameObject.SetActive(false);
			});
		}


		private Sprite GetBackground(EffectType effectType)
		{
			if (effectType == EffectType.Buff)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_SkillBuffs_Bg_01");
			}

			if (effectType != EffectType.Debuff)
			{
				return null;
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_SkillDebuffs_Bg_01");
		}
	}
}