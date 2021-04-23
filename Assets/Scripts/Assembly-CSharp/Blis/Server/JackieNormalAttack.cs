using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieNormalAttack)]
	public class JackieNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection1 = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollection2 = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			if (Caster.IsHaveStateByGroup(Singleton<JackieSkillActive2Data>.inst.Active4BuffGroup, Caster.ObjectId))
			{
				MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
				yield return WaitForSecondsByAttackSpeed(Caster,
					Singleton<JackieSkillNormalAttackData>.inst.SpecialNormalAttackDelay);
				parameterCollection1.Clear();
				parameterCollection1.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<JackieSkillNormalAttackData>.inst.NormalAttackApCoef);
				DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection1, 1001501);
				FinishNormalAttack();
				if (masteryType == MasteryType.DualSword)
				{
					yield return WaitForSecondsByAttackSpeed(Caster,
						Singleton<JackieSkillNormalAttackData>.inst.SpecialNormalAttackDelay);
					parameterCollection2.Clear();
					parameterCollection2.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<JackieSkillNormalAttackData>.inst.NormalAttackApCoef);
					DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection2, 1001501);
				}
			}
			else
			{
				int masteryType2 = (int) GetEquipWeaponMasteryType(Caster);
				yield return WaitForSecondsByAttackSpeed(Caster,
					Singleton<JackieSkillNormalAttackData>.inst.NormalAttackDelay[masteryType2]);
				parameterCollection1.Clear();
				parameterCollection1.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<JackieSkillNormalAttackData>.inst.NormalAttackApCoef);
				DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection1,
					Singleton<JackieSkillNormalAttackData>.inst.EffectAndSoundWeaponType[masteryType2]);
				FinishNormalAttack();
				if (masteryType2 == 18)
				{
					yield return WaitForSecondsByAttackSpeed(Caster,
						Singleton<JackieSkillNormalAttackData>.inst.NormalAttackDelay_2);
					parameterCollection2.Clear();
					parameterCollection2.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<JackieSkillNormalAttackData>.inst.NormalAttackApCoef);
					DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection2,
						Singleton<JackieSkillNormalAttackData>.inst.EffectAndSoundWeaponType[masteryType2]);
				}
			}

			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}