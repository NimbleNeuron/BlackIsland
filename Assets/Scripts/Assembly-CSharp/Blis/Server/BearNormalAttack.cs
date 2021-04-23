using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.BearNormalAttack)]
	public class BearNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster, Singleton<BearSkillNormalAttackData>.inst.AttackDelay);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<BearSkillNormalAttackData>.inst.ApCoefficient);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection, 2000008);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.79f);
			Finish();
		}
	}
}