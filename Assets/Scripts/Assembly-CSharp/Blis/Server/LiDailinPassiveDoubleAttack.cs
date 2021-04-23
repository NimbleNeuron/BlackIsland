using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinPassiveDoubleAttack)]
	public class LiDailinPassiveDoubleAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection cacheParameterCollectionFirst =
			SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection cacheParameterCollectionSecond =
			SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			LookAtTarget(Caster, Target, 0.1f);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			int masteryType = (int) GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackDelayFirst[masteryType]);
			cacheParameterCollectionFirst.Clear();
			cacheParameterCollectionFirst.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<LiDailinSkillData>.inst.NormalAttackApCoef);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 1, cacheParameterCollectionFirst,
				Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackEffectAndSoundWeaponType[masteryType]);
			FinishNormalAttack();
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackStateCode).group,
				Caster.ObjectId);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackDelaySecond[masteryType]);
			cacheParameterCollectionSecond.Clear();
			cacheParameterCollectionSecond.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackAp[Caster.GetSkillLevel(SkillSlotIndex.Passive)]);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, cacheParameterCollectionSecond,
				Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackEffectAndSoundWeaponType[masteryType]);
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}