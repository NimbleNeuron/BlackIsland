using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiActive1Attack)]
	public class YukiActive1Attack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			yield return WaitForSeconds(Singleton<YukiSkillActive1Data>.inst.NormalAttackDelay);
			float value = masteryType == MasteryType.DualSword
				? Singleton<YukiSkillActive1Data>.inst.DualSworldNormalAttackApCoef
				: Singleton<YukiSkillActive1Data>.inst.NormalAttackApCoef;
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef, value);
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<YukiSkillActive1Data>.inst.AttackDamage[info.SkillLevel]);
			bool flag = Caster.Status.ExtraPoint > 0;
			int effectAndSoundCode =
				flag
					? Singleton<YukiSkillActive1Data>.inst.PassiveEffectAndSoundWeaponType
					: Singleton<YukiSkillActive1Data>.inst.EffectAndSoundWeaponType;
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection, effectAndSoundCode);
			FinishNormalAttack();
			AddState(Target, Singleton<YukiSkillActive1Data>.inst.DebuffState);
			CharacterStateData data = GameDB.characterState.GetData(Singleton<YukiSkillActive1Data>.inst.BuffState);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
			if (flag)
			{
				CharacterState state =
					CreateState(Target, 2000009, 0, Singleton<YukiSkillActive1Data>.inst.StunDuration);
				AddState(Target, state);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}