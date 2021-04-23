using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using NodeCanvas.BehaviourTrees;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.BotPlayerCharacter)]
	public class WorldBotPlayerCharacter : WorldPlayerCharacter
	{
		
		
		public override bool IsAI
		{
			get
			{
				return true;
			}
		}

		
		
		public BotTeamRole TeamRole
		{
			get
			{
				return this.teamRole;
			}
		}

		
		
		public int StartAreaCode
		{
			get
			{
				return this.startAreaCode;
			}
		}

		
		protected override HostileAgent GetHostileAgent()
		{
			return this.hostileAgent;
		}

		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.BotPlayerCharacter;
		}

		
		
		public override PlayerType PlayerType
		{
			get
			{
				return PlayerType.BotPlayer;
			}
		}

		
		
		public WorldBotPlayerCharacter Leader
		{
			get
			{
				return this.leader;
			}
		}

		
		public override void Init(int characterCode, int skinCode, int teamNumber, SpecialSkillId specialSkillId)
		{
			base.Init(characterCode, skinCode, teamNumber, specialSkillId);
			if (MonoBehaviourInstance<GameService>.inst.MatchingMode == MatchingMode.Tutorial5)
			{
				this.survivableTime = 120f;
			}
			this.hostileAgent = new BotPlayerHostileAgent(this);
			GameUtil.Bind<BehaviourTreeOwner>(base.gameObject, ref this.behaviourTree);
			this.leader = null;
			this.teamRole = BotTeamRole.FOLLOWER;
		}

		
		public override bool IsAggressive()
		{
			return true;
		}

		
		protected override void Dead(int finishingAttacker, List<int> assistants, DamageType damageType)
		{
			if (!this.isAlive)
			{
				return;
			}
			this.StopAI();
			base.Dead(finishingAttacker, assistants, damageType);
			MonoBehaviourInstance<GameService>.inst.Bot.SetNextLeader(this);
		}

		
		public void StartAI()
		{
			this.behaviourTree.enabled = true;
			this.behaviourTree.StartBehaviour();
			base.Controller.Hold(null);
		}

		
		public void StopAI()
		{
			this.behaviourTree.StopBehaviour(true);
			this.behaviourTree.enabled = false;
			base.Controller.Stop();
		}

		
		public void MasteryLevelUpToTarget(MasteryType masteryType, int targetLevel)
		{
			this.mastery.MasteryLevelUpToTarget(masteryType, targetLevel);
		}

		
		public void SetMMRContext(int mmr)
		{
			this.mmrContext = new MMRContext(this, mmr);
		}

		
		public void SetTeamRole(BotTeamRole teamRole)
		{
			this.teamRole = teamRole;
		}

		
		public void SetLeader(WorldBotPlayerCharacter leader)
		{
			this.leader = leader;
		}

		
		public void SetStartAreaCode(int code)
		{
			this.startAreaCode = code;
		}

		
		private BotPlayerHostileAgent hostileAgent;

		
		private BotTeamRole teamRole;

		
		private BehaviourTreeOwner behaviourTree;

		
		private int startAreaCode;

		
		private WorldBotPlayerCharacter leader;
	}
}
