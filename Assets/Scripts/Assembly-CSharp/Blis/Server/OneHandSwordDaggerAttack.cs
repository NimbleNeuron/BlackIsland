using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.OneHandSwordDaggerAttack)]
	public class OneHandSwordDaggerAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 vector = Target.Position;
			vector += (Caster.Stat.AttackRange + Target.Stat.Radius * 0.5f) * -Target.Forward;
			Vector3 vector2;
			bool flag;
			float num;
			Caster.MoveToDestinationForTime(vector, 0f, EasingFunction.Ease.Linear, true, out vector2, out flag,
				out num);
			LookAtTarget(Caster, Target);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<OneHandSwordSkillActiveData>.inst.BuffState_2[info.SkillLevel]);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
			yield return WaitForSeconds(0.1f);
			damage.Clear();
			damage.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<OneHandSwordSkillActiveData>.inst.ApCoefficient);
			damage.Add(SkillScriptParameterType.CriticalChance, 1f);
			damage.Add(SkillScriptParameterType.FinalAddDamage,
				Target.Character.Status.Hp * Singleton<OneHandSwordSkillActiveData>.inst.DaggerAttackDamageRatioPerHp *
				0.01f);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, damage,
				Singleton<OneHandSwordSkillActiveData>.inst.EffectAndSoundCode);
			FinishNormalAttack();
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}