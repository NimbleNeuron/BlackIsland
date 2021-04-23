using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaHumanActive4)]
	public class SilviaHumanActive4 : SkillScript
	{
		
		private WorldMovableCharacter wmcCaster;

		
		protected override void Start()
		{
			base.Start();
			if (wmcCaster == null)
			{
				wmcCaster = Caster.Character as WorldMovableCharacter;
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanInitStateGroup, Caster.ObjectId);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanSkillStateGroup, Caster.ObjectId);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanNonSkillStateGroup, Caster.ObjectId);
			if (!Caster.IsHaveStateByGroup(Singleton<SilviaSkillCommonData>.inst.bikeStateGroup, Caster.ObjectId))
			{
				AddState(Caster, Singleton<SilviaSkillCommonData>.inst.bikeStateCode);
			}

			wmcCaster.StartSkillCooldown(SkillSlotSet.Active4_2, MasteryType.None, SkillCooldown);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		public override UseSkillErrorCode IsCanUseSkill(WorldCharacter hitTarget, Vector3? cursorPosition,
			WorldMovableCharacter caster)
		{
			if (caster.Status.ExtraPoint < Singleton<SilviaSkillHumanData>.inst.A4CanUseConditionEp)
			{
				return UseSkillErrorCode.NotEnoughExCost;
			}

			return UseSkillErrorCode.None;
		}
	}
}