using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinNormalAttack)]
	public class LiDailinNormalAttack : NormalAttackScript
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
			int masteryType = (int) GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<LiDailinSkillData>.inst.NormalAttackDelay[masteryType]);
			cacheParameterCollection.Clear();
			cacheParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<LiDailinSkillData>.inst.NormalAttackApCoef);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, cacheParameterCollection,
				Singleton<LiDailinSkillData>.inst.NormalAttackEffectAndSoundWeaponType[masteryType]);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}