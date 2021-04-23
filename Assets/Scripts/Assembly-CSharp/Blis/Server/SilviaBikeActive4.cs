using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaBikeActive4)]
	public class SilviaBikeActive4 : SkillScript
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
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanNonSkillStateGroup, Caster.ObjectId);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.bikeStateGroup, Caster.ObjectId);
			if (!Caster.IsHaveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanSkillStateGroup, Caster.ObjectId))
			{
				AddState(Caster, Singleton<SilviaSkillCommonData>.inst.humanSkillStateCode);
			}

			wmcCaster.StartSkillCooldown(SkillSlotSet.Active4_1, MasteryType.None, SkillCooldown);
			wmcCaster.StartSkillCooldown(SkillSlotSet.Active4_3, MasteryType.None, SkillCooldown);
			AddState(Caster, Singleton<SilviaSkillHumanData>.inst.NormalAttackReinforceState);
			Caster.MountNormalAttack(Singleton<SilviaSkillHumanData>.inst.NormalAttackMountSkillCode[SkillLevel]);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}