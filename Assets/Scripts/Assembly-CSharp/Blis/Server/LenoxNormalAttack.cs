using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxNormalAttack)]
	public class LenoxNormalAttack : NormalAttackScript
	{
		
		private float lastNormalAttackTime;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private int playCount;

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			playCount++;
			float currentServerFrameTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			if (Singleton<LenoxSkillNormalAttackData>.inst.NormalAttackNextTime <
			    currentServerFrameTime - lastNormalAttackTime)
			{
				playCount = 0;
			}

			lastNormalAttackTime = currentServerFrameTime;
			LookAtTarget(Caster, Target);
			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<LenoxSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<LenoxSkillNormalAttackData>.inst.NormalAttackApCoef);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection,
				Singleton<LenoxSkillNormalAttackData>.inst.EffectAndSoundWeaponType[masteryType][playCount % 2]);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}