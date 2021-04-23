using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.BoarNormalAttack)]
	public class BoarNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster, Singleton<BoarSkillNormalAttackData>.inst.AttackDelay);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<BoarSkillNormalAttackData>.inst.ApCoefficient);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection, 2000005);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.79f);
			Finish();
		}
	}
}