using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class WorldPlayerCharacterSkillAgent : WorldMovableCharacterSkillAgent
	{
		
		public WorldPlayerCharacterSkillAgent(WorldObject worldObject) : base(worldObject)
		{
			this.worldPlayerCharacter = (worldObject as WorldPlayerCharacter);
		}

		
		protected override WorldObject GetWorldObject()
		{
			return this.worldPlayerCharacter;
		}

		
		public override MasteryType GetEquipWeaponMasteryType()
		{
			return this.worldPlayerCharacter.GetEquipWeaponMasteryType();
		}

		
		public override bool IsEnoughBullet()
		{
			Item weapon = this.worldPlayerCharacter.GetWeapon();
			return weapon != null && weapon.ItemData.IsGunType() && 0 < this.worldPlayerCharacter.Status.Bullet;
		}

		
		public override bool IsFullBullet()
		{
			Item weapon = this.worldPlayerCharacter.GetWeapon();
			return weapon != null && weapon.ItemData.IsGunType() && weapon.MaxBulletCount == this.worldPlayerCharacter.Status.Bullet;
		}

		
		public override void ConsumeBullet(ProjectileData projectileData)
		{
			if (projectileData.isBullet)
			{
				this.worldPlayerCharacter.ConsumeBullet();
			}
		}

		
		public override void GunReload(bool playReloadAnimation)
		{
			this.worldPlayerCharacter.GunReload(playReloadAnimation);
		}

		
		public override void GunReload(bool playReloadAnimation, float reloadTime)
		{
			this.worldPlayerCharacter.GunReload(playReloadAnimation, reloadTime);
		}

		
		public override void ConsumeSkillResources(SkillCostType costType, int costKey, int cost)
		{
			this.worldPlayerCharacter.ConsumeSkillResources(costType, costKey, cost);
		}

		
		public override DamageInfo DirectDamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool isCheckAlly, bool targetInCombat)
		{
			if (target.Character == null)
			{
				return null;
			}
			if (!target.IsAlive)
			{
				return null;
			}
			parameters.Merge(this.worldPlayerCharacter.SkillController.GetPlayingScriptParameterCollection(target.Character, type, subType, damageId));
			WeaponType weaponType = WeaponType.None;
			Item weapon = this.worldPlayerCharacter.GetWeapon();
			if (weapon != null)
			{
				weaponType = weapon.ItemData.GetSubTypeData<ItemWeaponData>().weaponType;
			}
			int baseDamage = (int)parameters.Get(SkillScriptParameterType.Damage);
			DirectDamageCalculator calculator = new DirectDamageCalculator(weaponType, type, subType, damageDataCode, damageId, baseDamage, minRemain, damageMasteryModifier);
			calculator.SetAttackerSkillScriptParameter(parameters);
			target.Character.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter targetCharacter)
			{
				SkillScriptParameterCollection playingScriptParameterCollection = targetCharacter.SkillController.GetPlayingScriptParameterCollection(this.GetOwner(), type, subType, damageId);
				calculator.SetVictimSkillScriptParameter(playingScriptParameterCollection);
			});
			return Singleton<DamageService>.inst.DamageTo(target.Character, attackerInfo, calculator, new Vector3?(target.Position), skillSlotSet, isCheckAlly, effectAndSoundCode, targetInCombat);
		}

		
		public override DamageInfo DamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, Vector3 damagePoint, Vector3 damageDirection, bool isCheckAlly, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool targetInCombat)
		{
			if (target.Character == null)
			{
				return null;
			}
			if (!target.IsAlive)
			{
				return null;
			}
			parameters.Merge(this.worldPlayerCharacter.SkillController.GetPlayingScriptParameterCollection(target.Character, type, subType, damageId));
			WeaponType weaponType = WeaponType.None;
			Item weapon = this.worldPlayerCharacter.GetWeapon();
			if (weapon != null)
			{
				weaponType = weapon.ItemData.GetSubTypeData<ItemWeaponData>().weaponType;
			}
			int baseDamage = (int)parameters.Get(SkillScriptParameterType.Damage);
			DamageCalculator calculator = new BasicDamageCalculator(weaponType, type, subType, damageDataCode, damageId, baseDamage, minRemain, damageMasteryModifier);
			calculator.SetAttackerSkillScriptParameter(parameters);
			target.Character.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter targetCharacter)
			{
				SkillScriptParameterCollection playingScriptParameterCollection = targetCharacter.SkillController.GetPlayingScriptParameterCollection(this.GetOwner(), type, subType, damageId);
				calculator.SetVictimSkillScriptParameter(playingScriptParameterCollection);
			});
			return Singleton<DamageService>.inst.DamageTo(target.Character, attackerInfo, calculator, new Vector3?(damagePoint), skillSlotSet, isCheckAlly, effectAndSoundCode, targetInCombat);
		}

		
		public override void LockSkillSlot(SkillSlotSet skillSlotSet, bool isLock)
		{
			this.worldPlayerCharacter.LockSkillSlot(skillSlotSet, isLock);
		}

		
		public override void LockSkillSlot(SpecialSkillId specialSkillId, bool isLock)
		{
			this.worldPlayerCharacter.LockSkillSlot(specialSkillId, isLock);
		}

		
		public override void LockSkillSlotsWithPacket(SkillSlotSet skillSlotSetFlag, bool isLock)
		{
			this.worldPlayerCharacter.LockSkillSlotsWithPacket(skillSlotSetFlag, isLock);
		}

		
		public override bool SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
		{
			return this.worldPlayerCharacter.SwitchSkillSet(skillSlotIndex, skillSlotSet);
		}

		
		private readonly WorldPlayerCharacter worldPlayerCharacter;
	}
}
