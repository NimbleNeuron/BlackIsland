using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeNormalAttack)]
	public class LukeNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			int masteryType = (int) GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<LukeSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<LukeSkillNormalAttackData>.inst.NormalAttackApCoef);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection,
				Singleton<LukeSkillNormalAttackData>.inst.EffectAndSoundWeaponType[masteryType]);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}