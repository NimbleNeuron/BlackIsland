using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WildDogNormalAttack)]
	public class WildDogNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster, Singleton<WildDogSkillData>.inst.AttackDelay);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<WildDogSkillData>.inst.ApCoefficient);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection, 2000006);
			AddState(Target, Singleton<WildDogSkillData>.inst.BleedState);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.79f);
			Finish();
		}
	}
}