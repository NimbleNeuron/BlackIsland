using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinActive4Projectile)]
	public class HyejinActive4Projectile : SkillScript
	{
		
		private bool FindProjectileCondition(WorldProjectile projectile)
		{
			return projectile.Property.ProjectileData.code == Singleton<HyejinSkillData>.inst.A4ProjectileCode;
		}

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterProjectileDestoryEvent = (Action<int, int, int>) Delegate.Combine(
				inst.OnAfterProjectileDestoryEvent, new Action<int, int, int>(OnAfterProjectileDestory));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterProjectileDestoryEvent = (Action<int, int, int>) Delegate.Remove(
				inst.OnAfterProjectileDestoryEvent, new Action<int, int, int>(OnAfterProjectileDestory));
			WorldMovableCharacter worldMovableCharacter = Caster.Character as WorldMovableCharacter;
			if (worldMovableCharacter == null)
			{
				Log.E("HyeJin R state skill can not cast WorldMovableCharacter 1");
				return;
			}

			WorldProjectile ownProjectile = worldMovableCharacter.GetOwnProjectile(FindProjectileCondition);
			while (ownProjectile != null)
			{
				ownProjectile.DestroySelf();
				ownProjectile = worldMovableCharacter.GetOwnProjectile(FindProjectileCondition);
			}
		}

		
		private void OnAfterProjectileDestory(int projectileCode, int projectileObjectId, int ownerObjectId)
		{
			if (ownerObjectId != Caster.ObjectId || projectileCode != Singleton<HyejinSkillData>.inst.A4ProjectileCode)
			{
				return;
			}

			WorldMovableCharacter character = Caster.Character as WorldMovableCharacter;
			if (character == null)
			{
				Log.E("HyeJin R state skill can not cast WorldMovableCharacter");
				Finish();
			}
			else
			{
				if (character.GetOwnProjectile(Condition) != null)
				{
					return;
				}

				Caster.RemoveStateByGroup(
					GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.A4ProjectileState).group,
					Caster.ObjectId);
				Finish();
			}

			bool Condition(WorldProjectile projectile)
			{
				return FindProjectileCondition(projectile) && projectile.IsAlive &&
				       projectileObjectId != projectile.ObjectId;
			}

			// co: dotPeek
			// HyejinActive4Projectile.<>c__DisplayClass3_0 CS$<>8__locals1 = new HyejinActive4Projectile.<>c__DisplayClass3_0();
			// CS$<>8__locals1.<>4__this = this;
			// CS$<>8__locals1.projectileObjectId = projectileObjectId;
			// if (ownerObjectId != base.Caster.ObjectId)
			// {
			// 	return;
			// }
			// if (projectileCode != Singleton<HyejinSkillData>.inst.A4ProjectileCode)
			// {
			// 	return;
			// }
			// WorldMovableCharacter worldMovableCharacter = base.Caster.Character as WorldMovableCharacter;
			// if (worldMovableCharacter == null)
			// {
			// 	Log.E("HyeJin R state skill can not cast WorldMovableCharacter");
			// 	this.Finish(false);
			// 	return;
			// }
			// if (worldMovableCharacter.GetOwnProjectile(new Func<WorldProjectile, bool>(CS$<>8__locals1.<OnAfterProjectileDestory>g__Condition|0)) != null)
			// {
			// 	return;
			// }
			// base.Caster.RemoveStateByGroup(GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.A4ProjectileState).group, base.Caster.ObjectId);
			// this.Finish(false);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield break;
		}
	}
}