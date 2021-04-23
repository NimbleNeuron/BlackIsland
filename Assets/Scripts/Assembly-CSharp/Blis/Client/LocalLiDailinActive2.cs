using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LiDailinActive2)]
	public class LocalLiDailinActive2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanSkill02, true);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return ((int) Mathf.Round(Singleton<LiDailinSkillData>.inst.A2ChargeTerm *
				                          Singleton<LiDailinSkillData>.inst.A2ChargeCount * 10f) / 10f).ToString();
			}

			return "";
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/CoolTime";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return skillData.cooldown.ToString();
			}

			return "";
		}


		[SkillScript(SkillId.LiDailinActive2EndNormalReinforce)]
		public class LocalLiDailinActive2EndNormalReinforce : LocalSkillScript
		{
			public override void Start() { }


			public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


			public override void Finish(bool cancel) { }


			public override string GetTooltipValue(SkillData skillData, int index)
			{
				return "";
			}
		}
	}
}