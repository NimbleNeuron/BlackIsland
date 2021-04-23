using System.Collections.Generic;
using System.Linq;
using Blis.Common;

namespace Blis.Server
{
	
	public class CombatInvolvementAgent
	{
		
		
		private float CurrentTime
		{
			get
			{
				return this.gameService.CurrentServerFrameTime;
			}
		}

		
		public CombatInvolvementAgent(GameService gameService, WorldMovableCharacter movableCharacter)
		{
			this.gameService = gameService;
			this.myCharacter = movableCharacter;
		}

		
		public void AddCombatEvent(DamageInfo damageInfo)
		{
			if (damageInfo.Attacker == null)
			{
				if (this.systemCombatEvent == null)
				{
					this.systemCombatEvent = new CombatInvolvementAgent.SystemCombatEvent(this.CurrentTime);
				}
				if (damageInfo.DamageType != DamageType.Skill)
				{
					DamageSubType damageSubType = damageInfo.DamageSubType;
				}
				this.systemCombatEvent.Init(this.CurrentTime, damageInfo.Damage, damageInfo.DamageType, damageInfo.DamageSubType, damageInfo.DamageDataCode);
				return;
			}
			if (this.myCharacter.GetHostileType(damageInfo.Attacker) == HostileType.Ally)
			{
				return;
			}
			this.CleaningCombatEvents();
			if (damageInfo.Attacker is WorldPlayerCharacter)
			{
				WorldPlayerCharacter worldPlayerCharacter = (WorldPlayerCharacter)damageInfo.Attacker;
				CombatInvolvementAgent.PlayerCombatEvent item = new CombatInvolvementAgent.PlayerCombatEvent(this.CurrentTime, worldPlayerCharacter.ObjectId, worldPlayerCharacter.TeamNumber, worldPlayerCharacter.PlayerSession.nickname, worldPlayerCharacter.CharacterCode, damageInfo.Damage, damageInfo.DamageDataCode, damageInfo.DamageType, damageInfo.DamageSubType, this.GetCombatAssistantEvents(worldPlayerCharacter));
				this.playerCombatEvents.Add(item);
				return;
			}
			if (damageInfo.Attacker is WorldSummonBase)
			{
				WorldPlayerCharacter owner = ((WorldSummonBase)damageInfo.Attacker).Owner;
				if (owner != null)
				{
					CombatInvolvementAgent.PlayerCombatEvent item2 = new CombatInvolvementAgent.PlayerCombatEvent(this.CurrentTime, owner.ObjectId, owner.TeamNumber, owner.PlayerSession.nickname, owner.CharacterCode, damageInfo.Damage, damageInfo.DamageDataCode, damageInfo.DamageType, damageInfo.DamageSubType, this.GetCombatAssistantEvents(owner));
					this.playerCombatEvents.Add(item2);
				}
			}
			else if (damageInfo.Attacker is WorldMonster)
			{
				WorldMonster worldMonster = (WorldMonster)damageInfo.Attacker;
				if (this.monsterCombatEvent == null)
				{
					this.monsterCombatEvent = new CombatInvolvementAgent.MonsterCombatEvent(this.CurrentTime);
				}
				this.monsterCombatEvent.Init(this.CurrentTime, damageInfo.Attacker.ObjectId, worldMonster.MonsterData.code, damageInfo.Damage, damageInfo.DamageDataCode, damageInfo.DamageType, damageInfo.DamageSubType);
			}
		}

		
		public void SetDyingConditionEvent()
		{
			this.dyingConditionEvent = this.GetFinishingEvent();
		}

		
		public void ClearDyingConditionEvent()
		{
			this.dyingConditionEvent = null;
		}

		
		public void AddSupportEvent(HealInfo healInfo)
		{
			this.CleaningSupportEvents();
			if (healInfo.Healer == null)
			{
				return;
			}
			if (this.myCharacter.ObjectId == healInfo.Healer.ObjectId)
			{
				return;
			}
			if (this.myCharacter.GetHostileType(healInfo.Healer) != HostileType.Ally)
			{
				return;
			}
			CombatInvolvementAgent.PlayerAssistantEvent item = new CombatInvolvementAgent.PlayerAssistantEvent(this.CurrentTime, healInfo.Healer.ObjectId, healInfo.Healer.TeamNumber);
			this.playerSupportEvents.Add(item);
		}

		
		public void GetCombatInvolvementResult(ref CombatInvolvementAgent.CombatInvolvementResult result)
		{
			result.Clear();
			this.CleaningCombatEvents();
			this.CleaningSupportEvents();
			this.CleaningMonsterCombatEvent();
			this.CleaningSystemCombatEvent();
			result.SetFinishingAttackEvent(this.GetFinishingEvent());
			if (result.HasFinishingAttackEvent())
			{
				foreach (CombatInvolvementAgent.CombatEvent combatEvent in this.playerCombatEvents)
				{
					result.AddAssistantEvents(combatEvent);
					foreach (CombatInvolvementAgent.CombatEvent combatEvent2 in combatEvent.AssistantEvents())
					{
						result.AddAssistantEvents(combatEvent2);
					}
				}
			}
			if (!result.HasFinishingAttackEvent() && this.dyingConditionEvent != null)
			{
				result.SetFinishingAttackEvent(this.dyingConditionEvent);
			}
			if (!result.HasFinishingAttackEvent() && this.IsValidEvent(this.systemCombatEvent))
			{
				result.SetFinishingAttackEvent(this.systemCombatEvent);
			}
			if (!result.HasFinishingAttackEvent() && this.IsValidEvent(this.monsterCombatEvent))
			{
				result.SetFinishingAttackEvent(this.monsterCombatEvent);
			}
		}

		
		private CombatInvolvementAgent.CombatEvent GetFinishingEvent()
		{
			int num = this.playerCombatEvents.Count - 1;
			while (0 <= num)
			{
				CombatInvolvementAgent.CombatEvent combatEvent = this.playerCombatEvents[num];
				if (combatEvent.Damage > 0 && combatEvent.DamageType != DamageType.Sp)
				{
					return combatEvent;
				}
				num--;
			}
			if (this.dyingConditionEvent != null)
			{
				return this.dyingConditionEvent;
			}
			if (this.systemCombatEvent != null)
			{
				return this.systemCombatEvent;
			}
			if (this.monsterCombatEvent != null)
			{
				return this.monsterCombatEvent;
			}
			return null;
		}

		
		private List<CombatInvolvementAgent.CombatEvent> GetCombatAssistantEvents(WorldPlayerCharacter player)
		{
			this.tempAssistantEvents.Clear();
			using (List<KeyValuePair<int, int>>.Enumerator enumerator = player.GetBuffers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, int> buffer = enumerator.Current;
					if (!this.tempAssistantEvents.Any((CombatInvolvementAgent.CombatEvent x) => x.ObjectId == buffer.Key))
					{
						CombatInvolvementAgent.PlayerAssistantEvent item = new CombatInvolvementAgent.PlayerAssistantEvent(this.CurrentTime, buffer.Key, buffer.Value);
						this.tempAssistantEvents.Add(item);
					}
				}
			}
			using (List<KeyValuePair<int, int>>.Enumerator enumerator = this.myCharacter.GetDebuffers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, int> debuffer = enumerator.Current;
					if (!this.tempAssistantEvents.Any((CombatInvolvementAgent.CombatEvent x) => x.ObjectId == debuffer.Key))
					{
						CombatInvolvementAgent.PlayerAssistantEvent item2 = new CombatInvolvementAgent.PlayerAssistantEvent(this.CurrentTime, debuffer.Key, debuffer.Value);
						this.tempAssistantEvents.Add(item2);
					}
				}
			}
			using (List<CombatInvolvementAgent.CombatEvent>.Enumerator enumerator2 = player.CombatInvolvementAgent.GetPlayerSupportEvents().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					CombatInvolvementAgent.CombatEvent supportEvent = enumerator2.Current;
					if (!this.tempAssistantEvents.Any((CombatInvolvementAgent.CombatEvent x) => x.ObjectId == supportEvent.ObjectId) && this.IsValidEvent(supportEvent))
					{
						this.tempAssistantEvents.Add(supportEvent);
					}
				}
			}
			return this.tempAssistantEvents;
		}

		
		private List<CombatInvolvementAgent.CombatEvent> GetPlayerSupportEvents()
		{
			this.CleaningSupportEvents();
			return this.playerSupportEvents;
		}

		
		private bool IsValidEvent(CombatInvolvementAgent.CombatEvent combatEvent)
		{
			return combatEvent != null && this.CurrentTime - combatEvent.EventTime <= 10f;
		}

		
		private void CleaningCombatEvents()
		{
			for (int i = 0; i < this.playerCombatEvents.Count; i++)
			{
				if (!this.IsValidEvent(this.playerCombatEvents[i]))
				{
					this.playerCombatEvents.RemoveAt(i--);
				}
			}
		}

		
		private void CleaningSupportEvents()
		{
			for (int i = 0; i < this.playerSupportEvents.Count; i++)
			{
				if (!this.IsValidEvent(this.playerSupportEvents[i]))
				{
					this.playerSupportEvents.RemoveAt(i--);
				}
			}
		}

		
		private void CleaningMonsterCombatEvent()
		{
			if (!this.IsValidEvent(this.monsterCombatEvent))
			{
				this.monsterCombatEvent = null;
			}
		}

		
		private void CleaningSystemCombatEvent()
		{
			if (!this.IsValidEvent(this.systemCombatEvent))
			{
				this.systemCombatEvent = null;
			}
		}

		
		private readonly GameService gameService;

		
		private readonly WorldMovableCharacter myCharacter;

		
		private const float CombatInvolvementTime = 10f;

		
		private readonly List<CombatInvolvementAgent.CombatEvent> playerCombatEvents = new List<CombatInvolvementAgent.CombatEvent>();

		
		private CombatInvolvementAgent.CombatEvent dyingConditionEvent;

		
		private CombatInvolvementAgent.MonsterCombatEvent monsterCombatEvent;

		
		private CombatInvolvementAgent.SystemCombatEvent systemCombatEvent;

		
		private readonly List<CombatInvolvementAgent.CombatEvent> playerSupportEvents = new List<CombatInvolvementAgent.CombatEvent>();

		
		private readonly List<CombatInvolvementAgent.CombatEvent> tempAssistantEvents = new List<CombatInvolvementAgent.CombatEvent>();

		
		public abstract class CombatEvent
		{
			
			
			public float EventTime
			{
				get
				{
					return this.eventTime;
				}
			}

			
			
			public int ObjectId
			{
				get
				{
					return this.GetObjectId();
				}
			}

			
			
			public int TeamNumber
			{
				get
				{
					return this.GetTeamNumber();
				}
			}

			
			
			public string Name
			{
				get
				{
					return this.GetName();
				}
			}

			
			
			public int DataCode
			{
				get
				{
					return this.GetDataCode();
				}
			}

			
			
			public int Damage
			{
				get
				{
					return this.GetDamage();
				}
			}

			
			
			public DamageType DamageType
			{
				get
				{
					return this.GetDamageType();
				}
			}

			
			
			public DamageSubType DamageSubType
			{
				get
				{
					return this.GetDamageSubType();
				}
			}

			
			
			public int DamageDataCode
			{
				get
				{
					return this.GetDamageDataCode();
				}
			}

			
			protected CombatEvent(float eventTime)
			{
				this.eventTime = eventTime;
			}

			
			protected virtual int GetObjectId()
			{
				return 0;
			}

			
			protected virtual int GetTeamNumber()
			{
				return 0;
			}

			
			public bool HasTeam()
			{
				return 0 < this.TeamNumber;
			}

			
			protected virtual string GetName()
			{
				return "";
			}

			
			protected virtual int GetDataCode()
			{
				return 0;
			}

			
			protected virtual int GetDamage()
			{
				return 0;
			}

			
			protected virtual DamageType GetDamageType()
			{
				return DamageType.None;
			}

			
			protected virtual DamageSubType GetDamageSubType()
			{
				return DamageSubType.Normal;
			}

			
			protected virtual int GetDamageDataCode()
			{
				return 0;
			}

			
			public virtual List<CombatInvolvementAgent.CombatEvent> AssistantEvents()
			{
				return CombatInvolvementAgent.CombatEvent.Empty;
			}

			
			protected float eventTime;

			
			protected static readonly List<CombatInvolvementAgent.CombatEvent> Empty = new List<CombatInvolvementAgent.CombatEvent>();
		}

		
		private class SystemCombatEvent : CombatInvolvementAgent.CombatEvent
		{
			
			public SystemCombatEvent(float eventTime) : base(eventTime)
			{
			}

			
			public void Init(float eventTime, int damage, DamageType damageType, DamageSubType damageSubType, int damageDataCode)
			{
				this.eventTime = eventTime;
				this.damage = damage;
				this.damageType = damageType;
				this.damageSubType = damageSubType;
				this.damageDataCode = damageDataCode;
			}

			
			protected override int GetDamage()
			{
				return this.damage;
			}

			
			protected override DamageType GetDamageType()
			{
				return this.damageType;
			}

			
			protected override DamageSubType GetDamageSubType()
			{
				return this.damageSubType;
			}

			
			protected override int GetDamageDataCode()
			{
				return this.damageDataCode;
			}

			
			private int damage;

			
			private DamageType damageType;

			
			private DamageSubType damageSubType;

			
			private int damageDataCode;
		}

		
		private class PlayerCombatEvent : CombatInvolvementAgent.CombatEvent
		{
			
			public PlayerCombatEvent(float eventTime, int objectId, int teamNumber, string name, int dataCode, int damage, int damageDataCode, DamageType damageType, DamageSubType damageSubType, List<CombatInvolvementAgent.CombatEvent> assistantEvents) : base(eventTime)
			{
				this.objectId = objectId;
				this.teamNumber = teamNumber;
				this.name = name;
				this.dataCode = dataCode;
				this.damage = damage;
				this.damageDataCode = damageDataCode;
				this.damageType = damageType;
				this.damageSubType = damageSubType;
				this.assistantEvents = assistantEvents;
			}

			
			protected override int GetObjectId()
			{
				return this.objectId;
			}

			
			protected override int GetTeamNumber()
			{
				return this.teamNumber;
			}

			
			protected override string GetName()
			{
				return this.name;
			}

			
			protected override int GetDataCode()
			{
				return this.dataCode;
			}

			
			protected override int GetDamage()
			{
				return this.damage;
			}

			
			protected override DamageType GetDamageType()
			{
				return this.damageType;
			}

			
			protected override DamageSubType GetDamageSubType()
			{
				return this.damageSubType;
			}

			
			public override List<CombatInvolvementAgent.CombatEvent> AssistantEvents()
			{
				return this.assistantEvents ?? CombatInvolvementAgent.CombatEvent.Empty;
			}

			
			protected override int GetDamageDataCode()
			{
				return this.damageDataCode;
			}

			
			private readonly int objectId;

			
			private readonly int teamNumber;

			
			private readonly string name;

			
			private readonly int dataCode;

			
			private readonly int damage;

			
			private readonly int damageDataCode;

			
			private readonly DamageType damageType;

			
			private readonly DamageSubType damageSubType;

			
			private readonly List<CombatInvolvementAgent.CombatEvent> assistantEvents;
		}

		
		private class PlayerAssistantEvent : CombatInvolvementAgent.CombatEvent
		{
			
			public PlayerAssistantEvent(float eventTime, int objectId, int teamNumber) : base(eventTime)
			{
				this.objectId = objectId;
				this.teamNumber = teamNumber;
			}

			
			protected override int GetTeamNumber()
			{
				return this.teamNumber;
			}

			
			protected override int GetObjectId()
			{
				return this.objectId;
			}

			
			private readonly int objectId;

			
			private readonly int teamNumber;
		}

		
		private class MonsterCombatEvent : CombatInvolvementAgent.CombatEvent
		{
			
			public MonsterCombatEvent(float eventTime) : base(eventTime)
			{
			}

			
			public void Init(float eventTime, int objectId, int monsterCode, int damage, int damageDataCode, DamageType damageType, DamageSubType damageSubType)
			{
				this.eventTime = eventTime;
				this.objectId = objectId;
				this.monsterCode = monsterCode;
				this.damage = damage;
				this.damageDataCode = damageDataCode;
				this.damageType = damageType;
				this.damageSubType = damageSubType;
			}

			
			protected override int GetObjectId()
			{
				return this.objectId;
			}

			
			protected override int GetDataCode()
			{
				return this.monsterCode;
			}

			
			protected override int GetDamage()
			{
				return this.damage;
			}

			
			protected override DamageType GetDamageType()
			{
				return this.damageType;
			}

			
			protected override DamageSubType GetDamageSubType()
			{
				return this.damageSubType;
			}

			
			protected override int GetDamageDataCode()
			{
				return this.damageDataCode;
			}

			
			private int objectId;

			
			private int monsterCode;

			
			private int damage;

			
			private int damageDataCode;

			
			private DamageType damageType;

			
			private DamageSubType damageSubType;
		}

		
		public class CombatInvolvementResult
		{
			
			
			public ObjectType FinishingAttackerObjectType
			{
				get
				{
					return this.GetFinishingAttackerObjectType();
				}
			}

			
			
			public int FinishingAttackerObjectId
			{
				get
				{
					CombatInvolvementAgent.CombatEvent combatEvent = this.finishingAttackEvent;
					if (combatEvent == null)
					{
						return 0;
					}
					return combatEvent.ObjectId;
				}
			}

			
			
			public string FinishingAttackerName
			{
				get
				{
					CombatInvolvementAgent.CombatEvent combatEvent = this.finishingAttackEvent;
					return ((combatEvent != null) ? combatEvent.Name : null) ?? "";
				}
			}

			
			
			public DamageType FinishingAttackDamageType
			{
				get
				{
					CombatInvolvementAgent.CombatEvent combatEvent = this.finishingAttackEvent;
					if (combatEvent == null)
					{
						return DamageType.RedZone;
					}
					return combatEvent.DamageType;
				}
			}

			
			
			public DamageSubType FinishingAttackDamageSubType
			{
				get
				{
					CombatInvolvementAgent.CombatEvent combatEvent = this.finishingAttackEvent;
					if (combatEvent == null)
					{
						return DamageSubType.Normal;
					}
					return combatEvent.DamageSubType;
				}
			}

			
			
			public int FinishingAttackerDataCode
			{
				get
				{
					CombatInvolvementAgent.CombatEvent combatEvent = this.finishingAttackEvent;
					if (combatEvent == null)
					{
						return 0;
					}
					return combatEvent.DataCode;
				}
			}

			
			
			public int FinishingAttackDamageData
			{
				get
				{
					CombatInvolvementAgent.CombatEvent combatEvent = this.finishingAttackEvent;
					if (combatEvent == null)
					{
						return 0;
					}
					return combatEvent.DamageDataCode;
				}
			}

			
			
			public List<int> Assistants
			{
				get
				{
					return this.GetAssistants();
				}
			}

			
			private ObjectType GetFinishingAttackerObjectType()
			{
				if (this.finishingAttackEvent == null)
				{
					return ObjectType.None;
				}
				if (this.finishingAttackEvent is CombatInvolvementAgent.PlayerCombatEvent)
				{
					if (this.finishingAttackEvent.ObjectId != 0)
					{
						return ObjectType.PlayerCharacter;
					}
					return ObjectType.None;
				}
				else
				{
					if (this.finishingAttackEvent is CombatInvolvementAgent.MonsterCombatEvent)
					{
						return ObjectType.Monster;
					}
					return ObjectType.None;
				}
			}

			
			public bool HasFinishingAttackEvent()
			{
				return this.finishingAttackEvent != null;
			}

			
			public void SetFinishingAttackEvent(CombatInvolvementAgent.CombatEvent killEvent)
			{
				this.finishingAttackEvent = killEvent;
			}

			
			public void AddAssistantEvents(CombatInvolvementAgent.CombatEvent combatEvent)
			{
				if (combatEvent.ObjectId == this.FinishingAttackerObjectId)
				{
					return;
				}
				CombatInvolvementAgent.CombatEvent combatEvent2 = this.assistantEvents.Find((CombatInvolvementAgent.CombatEvent x) => x.ObjectId == combatEvent.ObjectId);
				if (combatEvent2 != null)
				{
					if (combatEvent2.EventTime < combatEvent.EventTime)
					{
						this.assistantEvents.Remove(combatEvent2);
						this.assistantEvents.Add(combatEvent);
					}
				}
				else
				{
					this.assistantEvents.Add(combatEvent);
				}
			}

			
			private List<int> GetAssistants()
			{
				this.assistants.Clear();
				if (this.finishingAttackEvent == null)
				{
					return null;
				}
				using (List<CombatInvolvementAgent.CombatEvent>.Enumerator enumerator = this.assistantEvents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CombatInvolvementAgent.CombatEvent assistantEvent = enumerator.Current;
						if (assistantEvent.ObjectId > 0 && this.finishingAttackEvent.HasTeam() && assistantEvent.HasTeam() && this.finishingAttackEvent.TeamNumber == assistantEvent.TeamNumber && !this.assistants.Any((int x) => x == assistantEvent.ObjectId))
						{
							this.assistants.Add(assistantEvent.ObjectId);
						}
					}
				}
				return this.assistants;
			}

			
			public void Clear()
			{
				this.finishingAttackEvent = null;
				this.assistantEvents.Clear();
			}

			
			private CombatInvolvementAgent.CombatEvent finishingAttackEvent;

			
			private List<CombatInvolvementAgent.CombatEvent> assistantEvents = new List<CombatInvolvementAgent.CombatEvent>();

			
			private List<int> assistants = new List<int>();
		}
	}
}
