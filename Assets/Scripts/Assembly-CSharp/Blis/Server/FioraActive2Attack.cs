using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.FioraActive2Attack)]
	public class FioraActive2Attack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollection2 = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, Target.Position);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			yield return WaitForSeconds(Singleton<FioraSkillActive2Data>.inst.NormalAttackDelay);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<FioraSkillActive2Data>.inst.NormalAttackApCoef[info.SkillLevel]);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection,
				Singleton<FioraSkillActive2Data>.inst.EffectAndSound);
			FinishNormalAttack();
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<FioraSkillActive2Data>.inst.BuffState[info.SkillLevel]);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
			yield return WaitForSeconds(Singleton<FioraSkillActive2Data>.inst.NormalAttackDelay_2);
			parameterCollection2.Clear();
			parameterCollection2.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<FioraSkillActive2Data>.inst.NormalAttackApCoef2[info.SkillLevel]);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection2,
				Singleton<FioraSkillActive2Data>.inst.EffectAndSound);
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}