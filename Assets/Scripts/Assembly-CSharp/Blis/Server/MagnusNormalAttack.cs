using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.MagnusNormalAttack)]
	public class MagnusNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			int masteryType = (int) GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<MagnusSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<MagnusSkillNormalAttackData>.inst.NormalAttackApCoef);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection,
				Singleton<MagnusSkillNormalAttackData>.inst.EffectAndSoundWeaponType[masteryType]);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}