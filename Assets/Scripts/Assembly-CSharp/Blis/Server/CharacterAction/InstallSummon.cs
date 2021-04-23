using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server.CharacterAction
{
	
	public class InstallSummon : ActionBase
	{
		
		private const float MoveToTargetTick = 0.1f;

		
		private readonly float createRange;

		
		private readonly Item item;

		
		private readonly Vector3 point;

		
		private readonly int summonCode;

		
		private readonly CastingActionType castingActionType;

		
		private float lastMoveToTargetTick;

		
		protected new WorldPlayerCharacter self;

		
		public InstallSummon(WorldPlayerCharacter self, Item item, Vector3 point) : base(self, false)
		{
			this.self = self;
			this.item = item;
			ItemSpecialData itemData = item.GetItemData<ItemSpecialData>();
			summonCode = itemData.summonCode;
			SummonData summonData = GameDB.character.GetSummonData(itemData.summonCode);
			createRange = summonData.createRange;
			castingActionType = summonData.castingActionType;
			this.point = point;
		}

		
		public override void Start()
		{
			if (GameUtil.DistanceOnPlane(point, self.GetPosition()) > createRange && self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToDestination(point);
			}
		}

		
		protected override ActionBase Update()
		{
			if (GameUtil.DistanceOnPlane(point, self.GetPosition()) <= createRange)
			{
				SingletonMonoBehaviour<BattleEventCollector>.inst.OnBeforeInstallSummon(self, summonCode);
				if (castingActionType == CastingActionType.None)
				{
					StopMove();
					SpawnSummonObject();
				}
				else
				{
					self.PlayerSession.Character.StartActionCasting(castingActionType, true, null, SpawnSummonObject,
						null, item.ItemData.code);
				}

				return new Idle(self, true);
			}

			if (self.CanMove() &&
			    MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastMoveToTargetTick > 0.1f)
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToDestination(point);
				lastMoveToTargetTick = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			}

			return null;
		}

		
		private void SpawnSummonObject()
		{
			if (IsImpossibleSummonPosition())
			{
				self.SendToastMessage(ToastMessageType.IsImpossibleSummonPosition);
				return;
			}

			self.UseItem(item.id, item.madeType);
			WorldSummonTrap worldSummonTrap =
				MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(self, summonCode, point) as WorldSummonTrap;
			if (worldSummonTrap == null)
			{
				return;
			}

			worldSummonTrap.SetActionOnTrapBurst(delegate(List<WorldCharacter> pTargets, WorldSummonBase pWorldSummon)
			{
				WorldPlayerCharacter owner = pWorldSummon.Owner;
				SummonData summonData = pWorldSummon.SummonData;
				int num = pWorldSummon.Stat.AttackPower;
				if (owner != null)
				{
					num = Mathf.RoundToInt(num * (1f + owner.Stat.TrapDamageRatio));
				}

				foreach (WorldCharacter worldCharacter in pTargets)
				{
					DirectDamageCalculator damageCalculator = new DirectDamageCalculator(WeaponType.None,
						DamageType.Normal, DamageSubType.Trap, summonData.code, 0, num, 0, 1f);
					Singleton<DamageService>.inst.DamageTo(worldCharacter, pWorldSummon.AttackerInfo, damageCalculator,
						pWorldSummon.GetPosition(), 0, false);
					if (summonData.stateEffect > 0)
					{
						worldCharacter.AddState(new CommonState(summonData.stateEffect, worldCharacter, owner),
							owner.ObjectId);
					}
				}
			});
		}

		
		private bool IsImpossibleSummonPosition()
		{
			ItemSpecialData subTypeData = item.ItemData.GetSubTypeData<ItemSpecialData>();
			if (subTypeData == null)
			{
				return true;
			}

			SummonData summonData = GameDB.character.GetSummonData(subTypeData.summonCode);
			if (summonData == null)
			{
				return true;
			}

			List<WorldPlayerCharacter> teamMembers = self.PlayerSession.GetTeamCharacters();
			teamMembers.Add(self);
			for (int i = 0; i < teamMembers.Count; ++i)
			{
				foreach (WorldSummonBase worldSummonBase in MonoBehaviourInstance<GameService>.inst.World
					.FindAll<WorldSummonBase>(s => s.IsOwner(teamMembers[i].ObjectId)))
				{
					if (worldSummonBase.SummonData.pileRange > 0.0)
					{
						float magnitude = Vector3
							.Scale(point - worldSummonBase.GetPosition(), new Vector3(1f, 0.0f, 1f)).magnitude;
						if (magnitude <= summonData.radius + (double) worldSummonBase.SummonData.radius ||
						    summonData.objectType == worldSummonBase.SummonData.objectType &&
						    magnitude <= (double) worldSummonBase.SummonData.pileRange)
						{
							return true;
						}
					}
				}
			}

			return false;

			// co: dotPeek
			// InstallSummon.<>c__DisplayClass12_0 CS$<>8__locals1 = new InstallSummon.<>c__DisplayClass12_0();
			// ItemSpecialData subTypeData = this.item.ItemData.GetSubTypeData<ItemSpecialData>();
			// if (subTypeData == null)
			// {
			// 	return true;
			// }
			// SummonData summonData = GameDB.character.GetSummonData(subTypeData.summonCode);
			// if (summonData == null)
			// {
			// 	return true;
			// }
			// CS$<>8__locals1.teamMembers = this.self.PlayerSession.GetTeamCharacters();
			// CS$<>8__locals1.teamMembers.Add(this.self);
			// int i;
			// Func<WorldSummonBase, bool> <>9__0;
			// int j;
			// for (i = 0; i < CS$<>8__locals1.teamMembers.Count; i = j)
			// {
			// 	WorldBase<WorldObject> world = MonoBehaviourInstance<GameService>.inst.World;
			// 	Func<WorldSummonBase, bool> checkCondition;
			// 	if ((checkCondition = <>9__0) == null)
			// 	{
			// 		checkCondition = (<>9__0 = ((WorldSummonBase s) => s.IsOwner(CS$<>8__locals1.teamMembers[i].ObjectId)));
			// 	}
			// 	foreach (WorldSummonBase worldSummonBase in world.FindAll<WorldSummonBase>(checkCondition))
			// 	{
			// 		if (worldSummonBase.SummonData.pileRange > 0f)
			// 		{
			// 			float magnitude = Vector3.Scale(this.point - worldSummonBase.GetPosition(), new Vector3(1f, 0f, 1f)).magnitude;
			// 			if (magnitude <= summonData.radius + worldSummonBase.SummonData.radius)
			// 			{
			// 				return true;
			// 			}
			// 			if (summonData.objectType == worldSummonBase.SummonData.objectType && magnitude <= worldSummonBase.SummonData.pileRange)
			// 			{
			// 				return true;
			// 			}
			// 		}
			// 	}
			// 	j = i + 1;
			// }
			// return false;
		}
	}
}