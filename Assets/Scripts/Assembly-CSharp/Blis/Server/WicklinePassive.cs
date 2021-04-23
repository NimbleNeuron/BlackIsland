using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WicklinePassive)]
	public class WicklinePassive : SkillScript
	{
		
		private const float COOLTIME_WICKLINE_POS_MESSAGE = 15f;

		
		private readonly Collider[] colliders = new Collider[20];

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			StartCoroutine(WicklineAreaMoveNotice());
			while (Caster.IsAlive)
			{
				yield return WaitForSeconds(Singleton<WicklineSkillPassiveData>.inst.Interval);
				if (Caster.Character.IsInCombat && !Caster.IsHaveStateByGroup(4007140, Caster.ObjectId))
				{
					AddState(Caster, 4007141);
				}

				if (!Caster.Character.IsInCombat && Caster.IsHaveStateByGroup(4007140, Caster.ObjectId))
				{
					Caster.RemoveStateByGroup(4007140, Caster.ObjectId);
				}

				int projectileCode = Singleton<WicklineSkillPassiveData>.inst.ProjectileCode;
				if (projectileCode != 0 && !IsOverlap(projectileCode))
				{
					ProjectileProperty projectileProperty = PopProjectileProperty(Caster, projectileCode);
					projectileProperty.SetExplosionSkill(SkillId.WicklinePassiveProjectile);
					LaunchProjectile(projectileProperty);
				}
			}
		}

		
		private bool IsOverlap(int projectileCode)
		{
			int num = Physics.OverlapSphereNonAlloc(Caster.Position, 0.3f, colliders,
				GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			for (int i = 0; i < num; i++)
			{
				WorldProjectile component = colliders[i].GetComponent<WorldProjectile>();
				if (!(component == null) && projectileCode.Equals(component.Property.ProjectileData.code) &&
				    Caster.ObjectId.Equals(component.Owner.ObjectId) && component.IsAlive)
				{
					return true;
				}
			}

			return false;
		}

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector instance = SingletonMonoBehaviour<BattleEventCollector>.Instance;
			instance.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(instance.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
		}

		
		private void OnKillEvent(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (Caster.ObjectId != victim.ObjectId)
			{
				return;
			}

			if (damageInfo.Attacker == null)
			{
				Log.V("[WICKLINE DEAD] Passive : Attacker is Null!!!");
				return;
			}

			try
			{
				if (GameDB.characterState.GetData(5000000 + Caster.Character.Status.Level) != null)
				{
					MonoBehaviourInstance<GameService>.inst.Announce.WicklineKilled(damageInfo.Attacker.ObjectId);
					AddState(damageInfo.Attacker.SkillAgent, damageInfo.Attacker.SkillAgent,
						5000000 + Caster.Character.Status.Level);
					damageInfo.Attacker.IfTypeOf<WorldPlayerCharacter>(delegate(WorldPlayerCharacter player)
					{
						foreach (WorldPlayerCharacter worldPlayerCharacter in player.PlayerSession.GetTeamCharacters())
						{
							if (worldPlayerCharacter.IsAlive)
							{
								AddState(worldPlayerCharacter.SkillAgent, worldPlayerCharacter.SkillAgent,
									5000000 + Caster.Character.Status.Level);
							}
						}
					});
				}
			}
			catch (Exception arg)
			{
				Log.V(string.Format("[WICKLINE DEAD] Passive : Exception({0})", arg));
			}
		}

		
		private IEnumerator WicklineAreaMoveNotice()
		{
			float coolTimeWicklinePosMessage = 0f;
			int preWicklineAreaCode = 16;
			WorldMovableCharacter casterCharacter = Caster.Character as WorldMovableCharacter;
			WaitForFrameUpdate waitFrame = new WaitForFrameUpdate();
			while (Caster.IsAlive)
			{
				yield return waitFrame.Frame(1);
				if (coolTimeWicklinePosMessage <= 0f)
				{
					int code = casterCharacter.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel)
						.code;
					if (code != 0 && code != preWicklineAreaCode)
					{
						preWicklineAreaCode = code;
						coolTimeWicklinePosMessage += 15f;
						MonoBehaviourInstance<GameService>.inst.Announce.WicklineMoveArea(preWicklineAreaCode);
					}
				}
				else
				{
					coolTimeWicklinePosMessage -= MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				}
			}
		}
	}
}