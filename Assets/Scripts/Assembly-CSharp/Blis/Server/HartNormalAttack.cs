using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HartNormalAttack)]
	public class HartNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private float lastNormalAttackTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;

		
		private int playCount;

		
		protected override void Start()
		{
			base.Start();
			playCount++;
			if (3f < MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastNormalAttackTime)
			{
				playCount = 0;
			}

			lastNormalAttackTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<HartSkillNormalAttackData>.inst.NormalAttackDelay);
			int dataCode;
			if (IsEnchanted())
			{
				if (playCount % 2 == 0)
				{
					dataCode = Singleton<HartSkillNormalAttackData>.inst.EnchantedProjectileCode_0;
				}
				else
				{
					dataCode = Singleton<HartSkillNormalAttackData>.inst.EnchantedProjectileCode_1;
				}
			}
			else if (playCount % 2 == 0)
			{
				dataCode = Singleton<HartSkillNormalAttackData>.inst.ProjectileCode_0;
			}
			else
			{
				dataCode = Singleton<HartSkillNormalAttackData>.inst.ProjectileCode_1;
			}

			ProjectileProperty projectile = PopProjectileProperty(Caster, dataCode);
			projectile.SetTargetObject(Target.ObjectId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<HartSkillNormalAttackData>.inst.NormalAttackApCoef);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			LaunchProjectile(projectile);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}

		
		protected bool IsEnchanted()
		{
			return Caster.AnyHaveStateByGroup(Singleton<HartSkillActive2Data>.inst.Active2BuffGroup);
		}
	}
}