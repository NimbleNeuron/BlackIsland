using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WolfNormalAttack)]
	public class WolfNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster, Singleton<WolfSkillNormalAttackData>.inst.AttackDelay);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<WolfSkillNormalAttackData>.inst.ApCoefficient);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection, 2000007);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.79f);
			Finish();
		}
	}
}