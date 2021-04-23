using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinPassiveDeadDrunkenness)]
	public class LiDailinPassiveDeadDrunkenness : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackStateCode).group,
				Caster.ObjectId);
			AddState(Caster, Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackDummyStateCode);
			Caster.MountNormalAttack(Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackSkillCode);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackDummyStateCode)
					.group, Caster.ObjectId);
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<LiDailinSkillData>.inst.PassiveDrunkennessStateCode).group,
				Caster.ObjectId);
			Caster.UnmountNormalAttack();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			while (Caster.Status.ExtraPoint > 0)
			{
				yield return WaitForSeconds(Singleton<LiDailinSkillData>.inst.PassiveDecompositionTermTime);
				Caster.ExtraPointModifyTo(Caster,
					Singleton<LiDailinSkillData>.inst.PassiveDrunkennessDecompositionAmount);
			}

			Finish();
		}
	}
}