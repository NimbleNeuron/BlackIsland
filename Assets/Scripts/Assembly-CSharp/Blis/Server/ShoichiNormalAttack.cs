using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiNormalAttack)]
	public class ShoichiNormalAttack : NormalAttackScript
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
			if (Singleton<ShoichiSkillNormalAttackData>.inst.NormalAttackNextTime <
			    currentServerFrameTime - lastNormalAttackTime)
			{
				playCount = 0;
			}

			lastNormalAttackTime = currentServerFrameTime;
			LookAtTarget(Caster, Target, 0.1f);
			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<ShoichiSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
			parameterCollection.Clear();
			if (Caster.IsHaveStateByGroup(Singleton<ShoichiSkillPassiveData>.inst.BuffMaxStateGroup, Caster.ObjectId))
			{
				int skillLevel = Caster.GetSkillLevel(SkillSlotIndex.Passive);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<ShoichiSkillNormalAttackData>.inst.NormalAttackApCoef +
					Singleton<ShoichiSkillPassiveData>.inst.BuffMaxDamageByLevel[skillLevel]);
			}
			else
			{
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<ShoichiSkillNormalAttackData>.inst.NormalAttackApCoef);
			}

			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection,
				Singleton<ShoichiSkillNormalAttackData>.inst.EffectAndSoundWeaponType[masteryType][playCount % 2]);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}