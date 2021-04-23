using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.Monster)]
	public class WorldMonster : WorldMovableCharacter
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.Monster;
		}

		
		protected override int GetCharacterCode()
		{
			return this.monsterCode;
		}

		
		protected override HostileAgent GetHostileAgent()
		{
			return this.hostileAgent;
		}

		
		
		public MonsterData MonsterData
		{
			get
			{
				return this.monsterData;
			}
		}

		
		
		public override bool IsAI
		{
			get
			{
				return true;
			}
		}

		
		public void Init(MonsterData data, MonsterSpawnPoint point, int level)
		{
			this.monsterCode = data.code;
			this.monsterData = data;
			this.spawnPoint = point;
			List<Item> list = GameDB.monster.SampleDropItem(new Func<int>(MonoBehaviourInstance<GameService>.inst.IncreaseAndGetItemId), data.dropGroup, data.randomDropCount, false);
			list.RemoveAll((Item item) => item == null || item.ItemData == null);
			this.corpseBox = new ItemBox(this.objectId, list.Count);
			this.corpseBox.SetItems(list);
			this.hostileAgent = new MonsterHostileAgent(this);
			CharacterStat characterStat = new CharacterStat();
			characterStat.UpdateCharacterStat(data, level);
			GameUtil.Bind<BehaviourTreeOwner>(base.gameObject, ref this.behaviourTree);
			IBlackboard blackboard = this.behaviourTree.blackboard;
			string varName = "initCount";
			int num = this.initCount + 1;
			this.initCount = num;
			blackboard.SetVariableValue(varName, num);
			if (this.monsterData.monster == MonsterType.Wickline)
			{
				this.updateDayNightCount = 0;
				this.behaviourTree.blackboard.SetVariableValue("updateDayNightCount", this.updateDayNightCount);
			}
			this.behaviourTree.enabled = true;
			base.InitCharacterSkill(data.code, SpecialSkillId.None);
			this.SetSkillLevel(base.CharacterSkill);
			base.Init(characterStat);
			base.Status.SetLevel(level);
		}

		
		private void SetSkillLevel(CharacterSkill characterSkill)
		{
			foreach (SkillSlotIndex skillSlotIndex in EnumUtil.GetValues<SkillSlotIndex>())
			{
				if (skillSlotIndex != SkillSlotIndex.Attack && skillSlotIndex != SkillSlotIndex.Passive)
				{
					SkillSlotSet? skillSlotSet = characterSkill.GetSkillSlotSet(skillSlotIndex);
					if (skillSlotSet != null && GameDB.skill.GetSkillSetData(this.monsterCode, base.ObjectType, skillSlotSet.Value) != null)
					{
						characterSkill.UpgradeSkill(skillSlotIndex);
					}
				}
			}
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<MonsterSnapshot>(new MonsterSnapshot
			{
				statusSnapshot = WorldObject.serializer.Serialize<MonsterStatusSnapshot>(new MonsterStatusSnapshot(base.Status)),
				initialStat = base.Stat.CreateSnapshot(),
				initialStateEffect = base.StateEffector.CreateSnapshot(),
				skillController = base.SkillController.CreateSnapshot(),
				moveAgentSnapshot = this.moveAgent.CreateSnapshot(),
				isInCombat = this.IsInCombat,
				isInvisible = base.IsInvisible,
				monsterCode = this.monsterCode
			});
		}

		
		protected override void Dead(int finishingAttacker, List<int> assistants, DamageType damageType)
		{
			if (!this.isAlive)
			{
				return;
			}
			this.behaviourTree.StopBehaviour(true);
			this.behaviourTree.enabled = false;
			base.Dead(finishingAttacker, assistants, damageType);
			WorldObject assistObject = null;
			MonoBehaviourInstance<GameService>.inst.World.TryFind<WorldObject>(finishingAttacker, ref assistObject);
			MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(this, assistObject, NoiseType.MonsterKilled);
			float num;
			if ((float)this.monsterData.regenTime == 0f || (float)this.monsterData.regenTime > 60f)
			{
				num = 60f;
			}
			else
			{
				num = (float)this.monsterData.regenTime - 10f;
				num = Mathf.Max(1f, num);
			}
			base.StartCoroutine(CoroutineUtil.DelayedAction(num, new Action(this.CleanupCorpse)));
		}

		
		private void CleanupCorpse()
		{
			MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(this);
			MonoBehaviourInstance<GameService>.inst.Spawn.AddRespawnMonster(this.spawnPoint);
		}

		
		public void SetAggressive(DayNight dayNight)
		{
			this.aggressive = (this.monsterData.aggressive == dayNight || this.monsterData.aggressive == DayNight.All);
			if (this.monsterData.monster == MonsterType.Wickline)
			{
				IBlackboard blackboard = this.behaviourTree.blackboard;
				string varName = "updateDayNightCount";
				int num = this.updateDayNightCount + 1;
				this.updateDayNightCount = num;
				blackboard.SetVariableValue(varName, num);
			}
		}

		
		public override bool IsAggressive()
		{
			return this.aggressive;
		}

		
		public override bool CanFindForAttack()
		{
			return !base.IsUntargetable() && this.isInCombat;
		}

		
		public override bool IsAttackable(WorldCharacter target)
		{
			return this.isAlive && !(target == null) && target.IsAlive && !target.IsUntargetable() && base.GetHostileType(target) == HostileType.Enemy && base.SightAgent.IsVisible(target.IsInvisible);
		}

		
		public override void SetInCombat(bool isCombat, WorldCharacter target)
		{
			AIState aiState = this.behaviourTree.blackboard.GetVariable<AIState>("AIState").value;
			if (isCombat && aiState == AIState.HOME)
				return;
			int num = isCombat != this.isInCombat ? 1 : 0;
			base.SetInCombat(isCombat, target);
			if (num != 0)
				this.EnqueueCommand((CommandPacket) new CmdUpdateInCombat()
				{
					isCombat = isCombat
				});
			if (!isCombat || aiState != AIState.IDLE || (!this.isAlive || !((UnityEngine.Object) this.Controller.GetTarget() == (UnityEngine.Object) null)))
				return;
			WorldPlayerCharacter target1 = target as WorldPlayerCharacter;
			if (!((UnityEngine.Object) target1 != (UnityEngine.Object) null))
				return;
			this.Controller.ForceTargetOn(target1);
			
			// co: dotPeek
			// AIState value = this.behaviourTree.blackboard.GetVariable("AIState").value;
			// if (isCombat && value == AIState.HOME)
			// {
			// 	return;
			// }
			// bool flag = isCombat != this.isInCombat;
			// base.SetInCombat(isCombat, target);
			// if (flag)
			// {
			// 	base.EnqueueCommand(new CmdUpdateInCombat
			// 	{
			// 		isCombat = isCombat
			// 	});
			// }
			// if (isCombat && value == AIState.IDLE && this.isAlive && base.Controller.GetTarget() == null)
			// {
			// 	WorldPlayerCharacter worldPlayerCharacter = target as WorldPlayerCharacter;
			// 	if (worldPlayerCharacter != null)
			// 	{
			// 		base.Controller.ForceTargetOn(worldPlayerCharacter);
			// 	}
			// }
		}

		
		protected override void OnCrowdControl(CharacterState state)
		{
			if (this.CharacterSkill.IsConcentrating())
				this.EnqueueCommand((CommandPacket) new CmdCrowdControl()
				{
					stateType = state.StateGroupData.stateType
				});
			if (this.behaviourTree.blackboard.GetVariable<AIState>("AIState").value != AIState.HOME)
				this.controller.OnCrowdControl();
			this.SkillController.OnCrowdControl(state.StateGroupData.stateType, state.StateData.GroupData.effectType);
			if (!state.CancelMove())
				return;
			this.StopMove();
			
			// co: dotPeek
			// if (base.CharacterSkill.IsConcentrating())
			// {
			// 	base.EnqueueCommand(new CmdCrowdControl
			// 	{
			// 		stateType = state.StateGroupData.stateType
			// 	});
			// }
			// if (this.behaviourTree.blackboard.GetVariable("AIState").value != AIState.HOME)
			// {
			// 	this.controller.OnCrowdControl();
			// }
			// base.SkillController.OnCrowdControl(state.StateGroupData.stateType, state.StateData.GroupData.effectType);
			// if (state.CancelMove())
			// {
			// 	base.StopMove();
			// }
		}

		
		protected override void CompleteRemoveState(CharacterState state, bool sendPacket)
		{
			base.CompleteRemoveState(state, sendPacket);
			CharacterStateGroupData groupData = GameDB.characterState.GetGroupData(state.StateData.group);
			if (groupData != null && groupData.stateType == StateType.Polymorph)
			{
				this.SetInCombat(true, state.Caster);
			}
		}

		
		protected override void OnDying(DamageInfo damageInfo)
		{
			if (this.monsterData.monster == MonsterType.Wickline)
			{
				Log.V("[WICKLINE DEAD] ### START ### : OnDying");
			}
			base.KillProcess(damageInfo);
			this.Dead(base.CombatInvolvementResult.FinishingAttackerObjectId, base.CombatInvolvementResult.Assistants, base.CombatInvolvementResult.FinishingAttackDamageType);
		}

		
		private int monsterCode;

		
		private MonsterSpawnPoint spawnPoint;

		
		private bool aggressive;

		
		private MonsterHostileAgent hostileAgent;

		
		private MonsterData monsterData;

		
		private int initCount;

		
		private int updateDayNightCount;

		
		private BehaviourTreeOwner behaviourTree;
	}
}
