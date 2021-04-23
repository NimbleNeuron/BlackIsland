using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.MagnusActive1)]
	public class MagnusActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<MagnusSkillActive1Data>.inst.ProjectileCode);
			projectileProperty.SetTargetDirection(GameUtil.Direction(Caster.Position, GetSkillPoint()));
			projectileProperty.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<MagnusSkillActive1Data>.inst.DamageByLevel[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<MagnusSkillActive1Data>.inst.SkillApCoef);
					DamageTo(targetAgent, attackerInfo, projectileProperty.ProjectileData.damageType,
						projectileProperty.ProjectileData.damageSubType, 0, parameterCollection,
						projectileProperty.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					AddState(targetAgent, Singleton<MagnusSkillActive1Data>.inst.DebuffState[SkillLevel]);
				});
			LaunchProjectile(projectileProperty);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}