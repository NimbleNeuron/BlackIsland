using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AyaActive2)]
	public class AyaActive2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private Vector3 direction = Vector3.zero;

		
		protected override void Start()
		{
			base.Start();
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
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

			direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			CasterLockRotation(true);
			StartConcentration();
			int shootCount = Singleton<AyaSkillActive2Data>.inst.ShootCount;
			float tick = SkillConcentrationTime / shootCount;
			int num;
			for (int index = 0; index < shootCount; index = num)
			{
				yield return WaitForSeconds(tick);
				LaunchProjectile();
				PlaySkillAction(Caster, 1);
				num = index + 1;
			}

			FinishConcentration(false);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}

		
		private void LaunchProjectile()
		{
			int equipWeaponMasteryType = (int) GetEquipWeaponMasteryType(Caster);
			
			ProjectileProperty projectile = PopProjectileProperty(Caster,
				Singleton<AyaSkillActive2Data>.inst.ProjectileCode[equipWeaponMasteryType]);
			
			projectile.SetTargetDirection(direction);
			projectile.SetActionOnCollisionCharacter(onCollideCharacter);
			
			base.LaunchProjectile(projectile);
			
			void onCollideCharacter(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			{
				parameterCollection.Clear();
				int skillLevel = SkillLevel;
				
				parameterCollection.Add(SkillScriptParameterType.Damage, Singleton<AyaSkillActive2Data>.inst.DamageByLevel[skillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef, Singleton<AyaSkillActive2Data>.inst.SkillApCoefByLevel[skillLevel]);
				DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType, projectile.ProjectileData.damageSubType, 0, parameterCollection, projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
			}
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			Finish();
		}
	}
}