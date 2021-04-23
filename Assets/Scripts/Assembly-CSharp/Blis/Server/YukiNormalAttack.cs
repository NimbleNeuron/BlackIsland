using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiNormalAttack)]
	public class YukiNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollection2 = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, Target.Position, 0.1f);
			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			int intMasteryType = (int) masteryType;
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<YukiSkillNormalAttackData>.inst.NormalAttackDelay[intMasteryType]);
			int effectAndSoundCode = Caster.Status.ExtraPoint > 0
				? Singleton<YukiSkillNormalAttackData>.inst.PassiveEffectAndSoundWeaponType[intMasteryType]
				: Singleton<YukiSkillNormalAttackData>.inst.EffectAndSoundWeaponType[intMasteryType];
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<YukiSkillNormalAttackData>.inst.NormalAttackApCoef);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection, effectAndSoundCode);
			FinishNormalAttack();
			if (masteryType == MasteryType.DualSword)
			{
				yield return WaitForSecondsByAttackSpeed(Caster,
					Singleton<YukiSkillNormalAttackData>.inst.NormalAttackDelay_2);
				int effectAndSoundCode2 = Caster.Status.ExtraPoint > 0
					? Singleton<YukiSkillNormalAttackData>.inst.PassiveEffectAndSoundWeaponType[intMasteryType]
					: Singleton<YukiSkillNormalAttackData>.inst.EffectAndSoundWeaponType[intMasteryType];
				parameterCollection2.Clear();
				parameterCollection2.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<YukiSkillNormalAttackData>.inst.NormalAttackApCoef);
				DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection2, effectAndSoundCode2);
			}

			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}