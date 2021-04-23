using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaHumanActive1)]
	public class SilviaHumanActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private readonly List<SkillActionTarget> hitEnemyObjects = new List<SkillActionTarget>();

		
		private CollisionBox3D collision;

		
		protected override void Start()
		{
			base.Start();
			if (collision == null)
			{
				collision = new CollisionBox3D(Caster.Position, SkillWidth,
					Singleton<SilviaSkillHumanData>.inst.A1HitRange, Vector3.zero);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Vector3 direction = GameUtil.Direction(Caster.Position, Target.Position);
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			PlaySkillAction(Caster, 1);
			int skillLevel = SkillLevel;
			HpHealTo(Caster, Caster.Stat.AttackPower, Singleton<SilviaSkillHumanData>.inst.A1ApHeal,
				Singleton<SilviaSkillHumanData>.inst.A1BaseHeal[skillLevel], true,
				Singleton<SilviaSkillHumanData>.inst.A1HealEffectSoundCode);
			Vector3 forward = Caster.Forward;
			collision.UpdatePosition(Caster.Position +
			                         forward * (Singleton<SilviaSkillHumanData>.inst.A1HitRange * 0.5f));
			collision.UpdateNormalized(forward);
			hitEnemyObjects.Clear();
			List<SkillAgent> characters = GetCharacters(collision, true, true);
			if (characters.Count > 0)
			{
				foreach (SkillAgent skillAgent in characters)
				{
					if (Caster.GetHostileType(skillAgent.Character) == HostileType.Enemy)
					{
						hitEnemyObjects.Add(new SkillActionTarget
						{
							targetId = skillAgent.ObjectId,
							targetPos = null
						});
						damage.Clear();
						damage.Add(SkillScriptParameterType.Damage,
							Singleton<SilviaSkillHumanData>.inst.A1BaseDamage[skillLevel]);
						damage.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<SilviaSkillHumanData>.inst.A1ApDamage);
						DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, damage,
							Singleton<SilviaSkillHumanData>.inst.A1DamageEffectSoundCode);
					}
					else
					{
						HpHealTo(skillAgent, Caster.Stat.AttackPower, Singleton<SilviaSkillHumanData>.inst.A1ApHeal,
							Singleton<SilviaSkillHumanData>.inst.A1BaseHeal[skillLevel], true,
							Singleton<SilviaSkillHumanData>.inst.A1HealEffectSoundCode);
					}
				}

				Caster.ExtraPointModifyTo(Caster,
					characters.Count * Singleton<SilviaSkillHumanData>.inst.A1EpGainPerHit);
				if (hitEnemyObjects.Count > 0)
				{
					PlaySkillAction(Caster, 2, hitEnemyObjects);
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}