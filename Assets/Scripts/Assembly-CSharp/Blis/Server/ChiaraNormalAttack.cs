using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraNormalAttack)]
	public class ChiaraNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection cacheParameterCollection =
			SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<ChiaraSkillData>.inst.NormalAttackDelay[masteryType]);
			cacheParameterCollection.Clear();
			cacheParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<ChiaraSkillData>.inst.NormalAttackApCoef);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, cacheParameterCollection,
				Singleton<ChiaraSkillData>.inst.NormalAttackEffectAndSoundWeaponType[masteryType]);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}