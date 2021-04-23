using System;
using Blis.Common;
using Blis.Server.CharacterAction;
using UnityEngine;

namespace Blis.Server
{
	
	[RequireComponent(typeof(WorldMovableCharacter))]
	public class MovableCharacterController : MonoBehaviour
	{
		
		public WorldObject GetTarget()
		{
			if (this.currentAction == null)
			{
				return null;
			}
			return this.currentAction.GetTarget();
		}

		
		public void Init(WorldMovableCharacter self)
		{
			this.self = self;
		}

		
		public void MoveTo(Vector3 destination, bool findAttackTarget)
		{
			if (this.self.IsDyingCondition)
			{
				this.SetAction(new MoveTo(this.self, destination, false));
				return;
			}
			if (this.self.SkillController.SkillScriptOnMove(destination))
			{
				return;
			}
			if (findAttackTarget && !this.self.IsInvisible)
			{
				WorldCharacter worldCharacter = this.self.FindEnemyForAttack(destination, 3f);
				if (worldCharacter == null)
				{
					float num = this.self.Stat.AttackRange;
					if (num < 5f)
					{
						num = 5f;
					}
					else
					{
						num = num * 2f - 5f;
					}
					worldCharacter = this.self.FindEnemyForAttack(this.self.GetPosition(), num);
				}
				if (worldCharacter != null && !worldCharacter.IsUntargetable())
				{
					this.SetAction(new AttackTarget(this.self, worldCharacter));
					return;
				}
			}
			this.SetAction(new MoveTo(this.self, destination, findAttackTarget));
		}

		
		public void TargetOn(WorldObject target)
		{
			if (target == null)
			{
				return;
			}
			if (this.self.IsDyingCondition)
			{
				this.SetAction(new MoveTo(this.self, target.GetPosition(), false));
				return;
			}
			WorldCharacter worldCharacter;
			if ((worldCharacter = (target as WorldCharacter)) != null)
			{
				if (worldCharacter.IsAlive)
				{
					if (this.self.GetHostileType(worldCharacter) == HostileType.Enemy)
					{
						if (!this.self.IsInAttackableDistance(worldCharacter) && this.self.SkillController.SkillScriptOnTargetOn(target))
						{
							return;
						}
						if (this.self.IsAttackable(worldCharacter) && this.self.IsExistNormalAttack())
						{
							this.SetAction(new AttackTarget(this.self, worldCharacter));
							return;
						}
						this.SetAction(new MoveTo(this.self, target.GetPosition(), false));
						return;
					}
					else
					{
						WorldPlayerCharacter worldPlayerCharacter;
						if ((worldPlayerCharacter = (target as WorldPlayerCharacter)) == null)
						{
							this.SetAction(new MoveTo(this.self, target.GetPosition(), false));
							return;
						}
						if (worldPlayerCharacter.TeamNumber == this.self.TeamNumber && worldPlayerCharacter.IsDyingCondition && worldPlayerCharacter.IsAlive)
						{
							this.SetAction(new TeamRevivalTarget(this.self, worldPlayerCharacter));
							return;
						}
					}
				}
				if (!this.self.SightAgent.IsInAllySight(worldCharacter.SightAgent, worldCharacter.GetPosition(), worldCharacter.Stat.Radius, worldCharacter.SightAgent.IsInvisibleCheckWithMemorizer(this.self.ObjectId)))
				{
					return;
				}
			}
			if (this.self.SkillController.SkillScriptOnTargetOn(target))
			{
				return;
			}
			this.SetAction(new InteractTarget(this.self, target));
		}

		
		public void ForceTargetOn(WorldPlayerCharacter target)
		{
			if (target == null)
			{
				return;
			}
			this.SetAction(new AttackTarget(this.self, target));
		}

		
		public void OnCrowdControl()
		{
			ActionBase actionBase = this.currentAction;
			if (actionBase != null)
			{
				UsePointSkill usePointSkill;
				if ((usePointSkill = (actionBase as UsePointSkill)) == null)
				{
					UseTargetSkill useTargetSkill;
					if ((useTargetSkill = (actionBase as UseTargetSkill)) == null)
					{
						WaitForSkillFinish waitForSkillFinish;
						if ((waitForSkillFinish = (actionBase as WaitForSkillFinish)) != null)
						{
							WaitForSkillFinish waitForSkillFinish2 = waitForSkillFinish;
							if (waitForSkillFinish2.NextAction != null)
							{
								waitForSkillFinish2.ReplaceNextAction(this.ChangeActionOnCrowdControl(waitForSkillFinish2.GetTargetCharacter()));
							}
						}
					}
					else
					{
						UseTargetSkill useTargetSkill2 = useTargetSkill;
						if (!useTargetSkill2.IsCastSkill)
						{
							this.SetAction(this.ChangeActionOnCrowdControl(useTargetSkill2.GetTargetCharacter()));
						}
					}
				}
				else
				{
					UsePointSkill usePointSkill2 = usePointSkill;
					if (!usePointSkill2.IsCastSkill)
					{
						this.SetAction(this.ChangeActionOnCrowdControl(usePointSkill2.GetTargetCharacter()));
					}
				}
			}
		}

		
		private ActionBase ChangeActionOnCrowdControl(WorldCharacter targetCharacter)
		{
			if (targetCharacter != null && this.self.GetHostileType(targetCharacter) == HostileType.Enemy && !targetCharacter.IsUntargetable())
			{
				return new AttackTarget(this.self, targetCharacter);
			}
			return new Idle(this.self, true);
		}

		
		public void SetIdle(bool findEnemy)
		{
			this.SetAction(new Idle(this.self, findEnemy));
		}

		
		public void Stop()
		{
			this.SetAction(new Stop(this.self));
		}

		
		public void Hold(WorldObject target = null)
		{
			this.SetAction(new Hold(this.self, target as WorldCharacter));
		}

		
		public void WarpTo(Vector3 destination, bool needCheckNavMesh)
		{
			this.Stop();
			this.self.WarpTo(destination, needCheckNavMesh);
		}

		
		public void UseInjectSkill(SkillUseInfo skillUseInfo)
		{
			SkillData skillData = skillUseInfo.skillData;
			if (skillData.PlayType != SkillPlayType.Overlap || !skillData.InstantCast())
			{
				this.SetAction(new UseInjectSkill(this.self, skillUseInfo));
				return;
			}
			this.self.SkillController.Cast(skillUseInfo, 0);
			if (skillData.StopAttackWhenStartSkill && !this.self.IsAI)
			{
				this.SetAction(new Idle(this.self, false));
				return;
			}
			if (skillData.StopWhenStartSkill && !this.self.IsAI)
			{
				this.Hold(null);
				return;
			}
			this.TargetOn(this.GetTarget());
		}

		
		public void UsePointSkill(SkillData skillData, SkillSlotSet skillSlotSet, MasteryType masteryType, Vector3 point, Vector3 release)
		{
			if (skillData.PlayType != SkillPlayType.Overlap || !skillData.InstantCast())
			{
				WorldCharacter target = this.GetTarget() as WorldCharacter;
				this.SetAction(new UsePointSkill(this.self, skillSlotSet, masteryType, point, release, target));
				return;
			}
			this.self.SkillController.Cast(SkillUseInfo.Point(this.self.SkillAgent, skillSlotSet, masteryType, this.self.CharacterSkill.GetSkillEvolutionLevel(skillSlotSet.SlotSet2Index()), point, release), this.self.GetSkillSequence(skillSlotSet));
			if (skillData.StopAttackWhenStartSkill && !this.self.IsAI)
			{
				this.SetAction(new Idle(this.self, false));
				return;
			}
			if (skillData.StopWhenStartSkill && !this.self.IsAI)
			{
				this.Hold(null);
				return;
			}
			this.TargetOn(this.GetTarget());
		}

		
		public void UseTargetSkill(SkillData skillData, SkillSlotSet skillSlotSet, MasteryType masteryType, WorldCharacter target)
		{
			if (skillData.PlayType != SkillPlayType.Overlap || !skillData.InstantCast())
			{
				this.SetAction(new UseTargetSkill(this.self, skillSlotSet, masteryType, target));
				return;
			}
			this.self.SkillController.Cast(SkillUseInfo.Target(this.self.SkillAgent, skillSlotSet, masteryType, this.self.CharacterSkill.GetSkillEvolutionLevel(skillSlotSet.SlotSet2Index()), target.SkillAgent), this.self.GetSkillSequence(skillSlotSet));
			if (skillData.StopAttackWhenStartSkill && !this.self.IsAI)
			{
				this.SetAction(new Idle(this.self, false));
				return;
			}
			if (skillData.StopWhenStartSkill && !this.self.IsAI)
			{
				this.Hold(target);
				return;
			}
			this.TargetOn(target);
		}

		
		public void InstallSummon(Item item, Vector3 position)
		{
			this.self.IfTypeOf<WorldPlayerCharacter>(delegate(WorldPlayerCharacter playerSelf)
			{
				this.SetAction(new InstallSummon(playerSelf, item, position));
			});
		}

		
		public void FrameUpdate()
		{
			try
			{
				if (this.currentAction != null)
				{
					ActionBase actionBase = this.currentAction.FrameUpdate();
					if (actionBase != null)
					{
						this.SetAction(actionBase);
					}
				}
			}
			catch (Exception ex)
			{
				Log.V(ex.ToString());
				this.currentAction = null;
			}
		}

		
		private void SetAction(ActionBase nextAction)
		{
			SkillActionBase skillActionBase = null;
			ChainActionBase chainActionBase;
			if ((chainActionBase = (this.currentAction as ChainActionBase)) != null)
			{
				if (!this.self.SkillController.CanMoveDuringSkillPlaying())
				{
					UseInjectSkill useInjectSkill;
					if ((useInjectSkill = (chainActionBase.NextAction as UseInjectSkill)) != null)
					{
						useInjectSkill.SetNextAction(nextAction);
						return;
					}
					UseInjectSkill useInjectSkill2;
					if ((useInjectSkill2 = (nextAction as UseInjectSkill)) != null)
					{
						useInjectSkill2.SetNextAction(chainActionBase.NextAction);
					}
					chainActionBase.ReplaceNextAction(nextAction);
					return;
				}
				else
				{
					skillActionBase = (chainActionBase.NextAction as SkillActionBase);
				}
			}
			if (skillActionBase != null)
			{
				skillActionBase.self.OnSkillReserveCancel(skillActionBase);
			}
			SkillActionBase skillActionBase2;
			if ((skillActionBase2 = (this.currentAction as SkillActionBase)) != null && !(nextAction is WaitForSkillFinish) && !nextAction.GetType().IsSubclassOf(typeof(WaitForSkillFinish)))
			{
				if (!skillActionBase2.IsCastSkill)
				{
					skillActionBase2.self.OnSkillReserveCancel(skillActionBase2);
				}
				else if (this.self.SkillController.IsPlaying(skillActionBase2.SkillSlotSet))
				{
					skillActionBase2.SetNextAction(nextAction);
					return;
				}
			}
			SkillActionBase skillActionBase3;
			if ((skillActionBase3 = (nextAction as SkillActionBase)) != null)
			{
				skillActionBase3.SetNextAction(this.currentAction);
			}
			try
			{
				nextAction.Start();
				this.currentAction = nextAction;
			}
			catch (Exception ex)
			{
				Log.E(ex.ToString());
				this.currentAction = null;
			}
		}

		
		public void SetInCombat(WorldCharacter target)
		{
			if (target == null || target.IsUntargetable())
			{
				return;
			}
			if (this.self.IsDyingCondition)
			{
				return;
			}
			if (this.currentAction != null)
			{
				if (this.currentAction.FindAttackTarget && !this.self.IsInvisible)
				{
					this.SetAction(new AttackTarget(this.self, target));
				}
			}
			else
			{
				this.SetAction(new AttackTarget(this.self, target));
			}
		}

		
		public bool IsReservedAction(SkillSlotSet skillSet)
		{
			if (this.currentAction == null)
			{
				return false;
			}
			ChainActionBase chainActionBase = this.currentAction as ChainActionBase;
			SkillActionBase skillActionBase;
			if (chainActionBase != null && chainActionBase.NextAction != null)
			{
				skillActionBase = (chainActionBase.NextAction as SkillActionBase);
			}
			else
			{
				skillActionBase = (this.currentAction as SkillActionBase);
			}
			return skillActionBase != null && skillActionBase.SkillSlotSet == skillSet;
		}

		
		public void ReserveAddtionalAction(SkillSlotSet skillSlotSet, Vector3 cursorPosition, SkillData skillData)
		{
			if (this.currentAction == null)
			{
				return;
			}
			ChainActionBase chainActionBase = this.currentAction as ChainActionBase;
			SkillActionBase skillActionBase;
			if (chainActionBase != null && chainActionBase.NextAction != null)
			{
				skillActionBase = (chainActionBase.NextAction as SkillActionBase);
			}
			else
			{
				skillActionBase = (this.currentAction as SkillActionBase);
			}
			if (skillActionBase == null)
			{
				return;
			}
			if (skillActionBase.SkillSlotSet != skillSlotSet)
			{
				return;
			}
			skillActionBase.AddPlayAgain(cursorPosition, skillData);
		}

		
		private WorldMovableCharacter self;

		
		private ActionBase currentAction;
	}
}
