using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class WorldCreatedObject : SkillAgent
	{
		
		public WorldCreatedObject(WorldObject worldObject) : base(worldObject)
		{
			this.worldObject = worldObject;
			this.stat = new CharacterStat();
			this.status = new CharacterStatus();
		}

		
		protected override WorldObject GetWorldObject()
		{
			return this.worldObject;
		}

		
		protected override WorldCharacter GetWorldCharacter()
		{
			return null;
		}

		
		protected override WorldMovableCharacter GetOwner()
		{
			return null;
		}

		
		protected override bool GetIsAlive()
		{
			return false;
		}

		
		protected override CharacterStat GetStat()
		{
			return this.stat;
		}

		
		protected override CharacterStatus GetStatus()
		{
			return this.status;
		}

		
		protected override CollisionObject3D GetCollisionObject()
		{
			return new CollisionCircle3D(Vector3.zero, 0f);
		}

		
		public override HostileType GetHostileType(WorldCharacter target)
		{
			return HostileType.Enemy;
		}

		
		public override WorldObject GetTarget()
		{
			return null;
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets)
		{
		}

		
		public override DamageInfo DirectDamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool isCheckAlly, bool targetInCombat)
		{
			return null;
		}

		
		public override DamageInfo DamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, Vector3 damagePoint, Vector3 damageDirection, bool isCheckAlly, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool targetInCombat)
		{
			return null;
		}

		
		public override void HealTo(SkillAgent target, int hpBaseAmount, float hpCoefficient, int hpFixAmount, int spBaseAmount, float spCoefficient, int spFixAmount, bool showUI, int effectAndSoundCode)
		{
		}

		
		public override void HpHealTo(SkillAgent target, int baseAmount, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
		}

		
		public override void LostHpHealTo(SkillAgent target, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
		}

		
		public override void SpHealTo(SkillAgent target, int baseAmount, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
		}

		
		public override void ExtraPointModifyTo(SkillAgent target, int deltaAmount)
		{
		}

		
		public override void ModifySkillCooldown(SkillSlotSet skillSlotSetFlag, float time)
		{
		}

		
		private readonly WorldObject worldObject;

		
		private readonly CharacterStat stat;

		
		private readonly CharacterStatus status;
	}
}
