using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyunwooNormalAttack)]
	public class HyunwooNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
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
				Singleton<HyunwooSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
			damage.Clear();
			damage.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<HyunwooSkillNormalAttackData>.inst.NormalAttackApCoef);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, damage,
				Singleton<HyunwooSkillNormalAttackData>.inst.EffectAndSoundWeaponType[masteryType]);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}