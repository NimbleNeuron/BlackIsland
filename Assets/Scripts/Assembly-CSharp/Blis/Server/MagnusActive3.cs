using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.MagnusActive3)]
	public class MagnusActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtTarget(Caster, Target);
			}

			int equipWeaponMasteryType = (int) GetEquipWeaponMasteryType(Caster);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<MagnusSkillActive3Data>.inst.SkillApCoef);
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<MagnusSkillActive3Data>.inst.DamageByLevel[SkillLevel]);
			DamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
				Singleton<MagnusSkillActive3Data>.inst.EffectAndSoundWeaponType[equipWeaponMasteryType]);
			Vector3 direction;
			if (0.0001f <= (Caster.Position - Target.Position).sqrMagnitude)
			{
				direction = Quaternion.Euler(0f, 90f, 0f) * (Caster.Position - Target.Position).normalized;
			}
			else
			{
				direction = Quaternion.Euler(0f, 90f, 0f) * -Caster.Forward;
			}

			KnockbackState knockbackState = CreateState<KnockbackState>(Target, 2000010);
			knockbackState.Init(direction, Singleton<MagnusSkillActive3Data>.inst.TargetMoveDistance,
				Singleton<MagnusSkillActive3Data>.inst.TargetMoveDuration, EasingFunction.Ease.EaseOutQuad, false);
			knockbackState.SetActionOnCollisionWall(delegate
			{
				AddState(Target, Singleton<MagnusSkillActive3Data>.inst.StunCode);
			});
			Target.AddState(knockbackState, Caster.ObjectId);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}