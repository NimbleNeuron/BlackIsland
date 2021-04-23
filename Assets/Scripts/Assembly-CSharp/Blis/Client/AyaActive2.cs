using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AyaActive2)]
	public class AyaActive2 : LocalSkillScript
	{
		private readonly int hMoveHash = Animator.StringToHash("hMove");


		private readonly int vMoveHash = Animator.StringToHash("vMove");


		private float hMoveOld;


		private Vector3 lastFramePosition;


		private Vector3 lastMoveDirection;


		private int playCount;


		private float updateTime;


		private float vMoveOld;


		public override void Start()
		{
			PlayAnimation(Self, TriggerReloadCancel);
			SetAnimation(Self, BooleanSkill02, true);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			LocalPlayerCharacter localPlayerCharacter = Self as LocalPlayerCharacter;
			MasteryType equipWeaponMasteryType = GetEquipWeaponMasteryType(localPlayerCharacter);
			CharacterMasteryData characterMasteryData =
				GameDB.mastery.GetCharacterMasteryData(localPlayerCharacter.CharacterCode);
			SetAnimatorLayerWeight(equipWeaponMasteryType, characterMasteryData, "Aiming", 1f);
			SetAnimatorLayerWeight(equipWeaponMasteryType, characterMasteryData, "AimingShot", 0.7f);
			Vector3 offset = new Vector3(0f, 1f, 0f);
			PlayEffectPoint(Self, "FX_BI_Character_Skill", offset);
			PlaySoundPoint(Self, "aya_Skill02_Activation");
			lastFramePosition = Self.GetPosition();
			StartCoroutine(UpdateAnimationParameter());
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayAnimation(Self, TriggerSkill02_2);
			}
		}


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
			SetAnimation(Self, BooleanSkill02, false);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			StopCoroutines();
		}


		private IEnumerator UpdateAnimationParameter()
		{
			updateTime = 0f;
			vMoveOld = 0f;
			hMoveOld = 0f;
			lastMoveDirection = Vector3.zero;
			lastFramePosition = Self.GetPosition();
			lastFramePosition.y = 0f;
			for (;;)
			{
				Vector3 position = Self.GetPosition();
				position.y = 0f;
				float animatorFloatParameter = GetAnimatorFloatParameter(Self, vMoveHash);
				float animatorFloatParameter2 = GetAnimatorFloatParameter(Self, hMoveHash);
				Vector3 normalized = (position - lastFramePosition).normalized;
				if (0.01f <= (normalized - lastMoveDirection).sqrMagnitude)
				{
					updateTime = 0f;
					vMoveOld = animatorFloatParameter;
					hMoveOld = animatorFloatParameter2;
					lastMoveDirection = normalized;
				}

				float b = Vector3.Dot(normalized, SelfForward);
				float b2 = Vector3.Dot(normalized, SelfRight);
				updateTime += Time.deltaTime * 5f;
				if (1f < updateTime)
				{
					updateTime = 1f;
				}

				SetAnimation(Self, vMoveHash, Mathf.Lerp(vMoveOld, b, updateTime));
				SetAnimation(Self, hMoveHash, Mathf.Lerp(hMoveOld, b2, updateTime));
				lastFramePosition = position;
				yield return null;
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return Singleton<AyaSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return ((int) (Singleton<AyaSkillActive2Data>.inst.SkillApCoefByLevel[skillData.level] *
			               SelfStat.AttackPower)).ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/SkillApCoef";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
					return "ToolTipType/Cost";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AyaSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return Singleton<AyaSkillActive2Data>.inst.SkillApCoefByLevel[skillData.level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}