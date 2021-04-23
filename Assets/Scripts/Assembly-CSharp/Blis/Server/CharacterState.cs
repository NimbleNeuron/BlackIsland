using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public abstract class CharacterState
	{
		
		
		public WorldCharacter Self
		{
			get
			{
				return this.self;
			}
		}

		
		
		public WorldCharacter Caster
		{
			get
			{
				return this.caster;
			}
		}

		
		
		public float CreatedTime
		{
			get
			{
				return this.createdTime;
			}
		}

		
		
		public float DurationPauseEndTime
		{
			get
			{
				return this.durationPauseEndTime;
			}
		}

		
		
		public float Duration
		{
			get
			{
				return this.duration;
			}
		}

		
		
		public float OriginalDuration
		{
			get
			{
				return this.originalDuration;
			}
		}

		
		
		public int StackCount
		{
			get
			{
				return this.stackCount;
			}
		}

		
		
		public List<StatParameter> ExternalStats
		{
			get
			{
				return this.externalStats;
			}
		}

		
		
		public CharacterStateData StateData
		{
			get
			{
				return this.stateData;
			}
		}

		
		
		public int Code
		{
			get
			{
				return this.StateData.code;
			}
		}

		
		
		public int Group
		{
			get
			{
				return this.StateData.group;
			}
		}

		
		
		public int Level
		{
			get
			{
				return this.StateData.level;
			}
		}

		
		
		public int MaxStack
		{
			get
			{
				return this.StateData.maxStack;
			}
		}

		
		
		public CharacterStateGroupData StateGroupData
		{
			get
			{
				return this.StateData.GroupData;
			}
		}

		
		
		public int ReserveCount
		{
			get
			{
				return this.reserveCount;
			}
		}

		
		
		public bool CanReserve
		{
			get
			{
				return this.StateData.canReserve;
			}
		}

		
		
		public int IsSkillPause
		{
			get
			{
				return this.isSkillPause;
			}
		}

		
		protected CharacterState(int stateCode, WorldCharacter self, WorldCharacter caster)
		{
			this.stateData = GameDB.characterState.GetData(stateCode);
			this.self = self;
			this.caster = caster;
			this.duration = this.stateData.duration;
			this.originalDuration = this.stateData.duration;
			this.isSkillPause = 0;
			this.externalStats.Clear();
			this.CreateStatCalculator(this.stateData.GroupData.activateStatCalculatorOnCreate);
		}

		
		private void CreateStatCalculator(bool activateStatCalculatorOnCreate)
		{
			if (activateStatCalculatorOnCreate)
			{
				switch (this.stateData.GroupData.statCalculationType)
				{
				case StateStatCalculationType.None:
					this.statCalculator = new BasicStatCalculator(this);
					break;
				case StateStatCalculationType.Common:
					this.statCalculator = new CommonStatCalculator(this);
					break;
				case StateStatCalculationType.LostHpRate:
					this.statCalculator = new LostHpRateStatCalculator(this);
					break;
				case StateStatCalculationType.ApplyOverTime:
					this.statCalculator = new ApplyStatOverTimeStatCalculator(this);
					break;
				case StateStatCalculationType.RestoreOverTime:
					this.statCalculator = new RestoreStatOverTimeStatCalculator(this);
					break;
				}
			}
			else
			{
				StateStatCalculationType statCalculationType = this.stateData.GroupData.statCalculationType;
				if (statCalculationType > StateStatCalculationType.Common)
				{
					if (statCalculationType - StateStatCalculationType.LostHpRate <= 2)
					{
						this.statCalculator = new NoneStatCalculator(this);
					}
				}
				else
				{
					this.statCalculator = new BasicStatCalculator(this);
				}
			}
			this.stats.Clear();
		}

		
		public static CharacterState Create(SkillUseInfo skillUseInfo, WorldCharacter self, WorldCharacter caster, int initStack = 0, float? duration = null)
		{
			CharacterState characterState = CharacterState.Create(skillUseInfo.StateCode, self, caster, initStack, duration);
			characterState.skillUseInfo = skillUseInfo;
			return characterState;
		}

		
		public static CharacterState Create(int stateCode, WorldCharacter self, WorldCharacter caster, int initStack = 0, float? duration = null)
		{
			CharacterStateData data = GameDB.characterState.GetData(stateCode);
			CharacterStateGroupData groupData = GameDB.characterState.GetGroupData(data.group);
			CharacterState characterState = null;
			switch (groupData.stateBehaviourType)
			{
			case StateBehaviourType.Common:
				characterState = new CommonState(data.code, self, caster);
				break;
			case StateBehaviourType.ExtensionCommon:
				characterState = new ExtensionCommonState(data.code, self, caster);
				break;
			case StateBehaviourType.Rest:
				characterState = new RestState(data.code, self, caster);
				break;
			case StateBehaviourType.Regen:
				characterState = new RegenState(data.code, self, caster);
				break;
			case StateBehaviourType.SpRegen:
				characterState = new SpRegenState(data.code, self, caster);
				break;
			case StateBehaviourType.MonsterReset:
				characterState = new MonsterResetState(data.code, self, caster);
				break;
			case StateBehaviourType.Shield:
				characterState = new ShieldState(data.code, self, caster);
				break;
			case StateBehaviourType.Blocking:
				characterState = new BlockingState(data.code, self, caster);
				break;
			case StateBehaviourType.Airborne:
				characterState = new AirborneState(data.code, self, caster);
				break;
			case StateBehaviourType.Knockback:
				characterState = new KnockbackState(data.code, self, caster);
				break;
			case StateBehaviourType.Grab:
				characterState = new GrabState(data.code, self, caster);
				break;
			}
			characterState.CreateStatCalculator(data.GroupData.activateStatCalculatorOnCreate);
			if (initStack > 0)
			{
				characterState.stackCount = initStack;
			}
			if (duration != null)
			{
				characterState.duration = duration.Value;
				characterState.originalDuration = duration.Value;
			}
			return characterState;
		}

		
		public void ActivateCalculator(bool activate)
		{
			this.CreateStatCalculator(activate);
			this.CalculateStats();
		}

		
		public void AddExternalStat(StatType statType, float statValue, StatType coefStatType, float coefStatValue)
		{
			this.externalStats.Add(new StatParameter(statType, statValue, coefStatType, coefStatValue));
		}

		
		public void SetExternalNonCalculateStatValue(float value)
		{
			this.externalNonCalculateStatValue = value;
			if (this.stateData.nonCalculateStatType == StatType.None)
			{
				return;
			}
			this.CalculateStats();
		}

		
		public void ModifyDuration(float changeAmount)
		{
			this.duration += changeAmount;
		}

		
		public void SetDuration(float changeAmount)
		{
			this.duration = changeAmount;
		}

		
		public void UpdateData(CharacterStateData stateData)
		{
			this.stateData = stateData;
		}

		
		public float RemainTime()
		{
			float num;
			if (this.durationPauseEndTime == 0f)
			{
				num = this.createdTime + this.Duration;
			}
			else
			{
				num = this.durationPauseEndTime + this.Duration;
			}
			return num - MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}

		
		public float ElapsedTime()
		{
			return MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.createdTime;
		}

		
		public bool IsPause()
		{
			return this.durationPauseEndTime > MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}

		
		public void DurationPause(float deltaPauseTime)
		{
			if (!this.IsPause())
			{
				if (this.durationPauseEndTime == 0f)
				{
					this.duration -= MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.createdTime;
				}
				else
				{
					this.duration -= MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.durationPauseEndTime;
				}
			}
			this.durationPauseEndTime = deltaPauseTime + MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}

		
		public void Start(int stackCount)
		{
			this.createdTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.stackCount = ((0 < stackCount) ? stackCount : this.StateGroupData.defaultStack);
			this.Execute();
			this.CalculateStats();
		}

		
		public void AddStack(int stackCount)
		{
			if (stackCount <= 0)
			{
				stackCount = 1;
			}
			this.stackCount += stackCount;
			if (this.MaxStack <= 1)
			{
				this.stackCount = this.StateGroupData.defaultStack;
			}
			else if (this.MaxStack < this.stackCount)
			{
				this.stackCount = this.MaxStack;
			}
			WorldPlayerCharacter worldPlayerCharacter = this.Self as WorldPlayerCharacter;
			if (worldPlayerCharacter != null)
			{
				worldPlayerCharacter.CharacterSkill.SkillEvolution.OnStateStack(worldPlayerCharacter.CharacterCode, this.stateData.group, this.stackCount, new Action<SkillEvolutionPointType, int>(worldPlayerCharacter.UpdateSkillEvolutionPoint));
			}
			this.Execute();
			this.CalculateStats();
		}

		
		public void ResetCreateTime()
		{
			this.createdTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}

		
		protected virtual void Execute()
		{
		}

		
		public void UseSkill()
		{
			if (this.skillUseInfo == null)
			{
				return;
			}
			if (this.StateData.GroupData.skillId == SkillId.None)
			{
				return;
			}
			if (!this.self.IsTypeOf<WorldMovableCharacter>() || !this.caster.IsTypeOf<WorldMovableCharacter>())
			{
				if (this.self.IsTypeOf<WorldSummonBase>() && this.caster.IsTypeOf<WorldSummonBase>())
				{
					this.skillUseInfo.caster = this.caster.SkillAgent;
					this.skillUseInfo.target = this.self.SkillAgent;
					this.skillUseInfo.stateData = this.StateData;
					WorldSummonBase worldSummonBase = this.self as WorldSummonBase;
					if (worldSummonBase == null)
					{
						return;
					}
					worldSummonBase.StartStateSkill(this.skillUseInfo, this);
				}
				return;
			}
			this.skillUseInfo.caster = this.caster.SkillAgent;
			this.skillUseInfo.target = this.self.SkillAgent;
			this.skillUseInfo.stateData = this.StateData;
			WorldMovableCharacter worldMovableCharacter = this.self as WorldMovableCharacter;
			if (worldMovableCharacter == null)
			{
				return;
			}
			worldMovableCharacter.PlayStateSkill(this.skillUseInfo, this);
		}

		
		public void RemoveStack(int stackCount)
		{
			this.stackCount -= stackCount;
			if (this.stackCount < 0)
			{
				this.stackCount = 0;
			}
		}

		
		public Dictionary<StatType, float> GetStats()
		{
			return this.stats;
		}

		
		public void CalculateStats()
		{
			if (this.statCalculator.Calculation(this.stackCount, this.externalNonCalculateStatValue, this.externalStats, ref this.stats))
			{
				CharacterState.CalculateStat onCalculateStat = this.OnCalculateStat;
				if (onCalculateStat == null)
				{
					return;
				}
				onCalculateStat();
			}
		}

		
		public abstract bool IsDone();

		
		public void FrameUpdate()
		{
			if (this.stateData.GroupData.statCalculationType == StateStatCalculationType.ApplyOverTime || this.stateData.GroupData.statCalculationType == StateStatCalculationType.RestoreOverTime)
			{
				this.lastStatCalculationTick += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (0.2f <= this.lastStatCalculationTick)
				{
					this.lastStatCalculationTick -= 0.2f;
					if (this.statCalculator.Calculation(this.stackCount, this.externalNonCalculateStatValue, this.externalStats, ref this.stats))
					{
						CharacterState.CalculateStat onCalculateStat = this.OnCalculateStat;
						if (onCalculateStat != null)
						{
							onCalculateStat();
						}
					}
				}
			}
			this.Update();
		}

		
		protected virtual void Update()
		{
		}

		
		public virtual void Reserve(CharacterState state)
		{
			this.reserveCount++;
		}

		
		public virtual void ExecuteReserve()
		{
			this.reserveCount--;
		}

		
		public void Terminate(bool cancel)
		{
			this.CancelSkill(cancel);
		}

		
		protected void CancelSkill(bool cancel)
		{
			SkillUseInfo skillUseInfo = this.skillUseInfo;
			if (((skillUseInfo != null) ? skillUseInfo.stateData : null) == null)
			{
				return;
			}
			WorldMovableCharacter worldMovableCharacter = this.self as WorldMovableCharacter;
			if (worldMovableCharacter == null)
			{
				return;
			}
			worldMovableCharacter.CancelStateSkill(this.skillUseInfo.stateData.GroupData.skillId, this.skillUseInfo.caster.ObjectId, cancel);
		}

		
		public void SkillPause()
		{
			this.isSkillPause++;
		}

		
		public void SkillResume()
		{
			this.isSkillPause--;
		}

		
		public virtual bool CancelMove()
		{
			return this.StateGroupData.stateType.CancelMove(this.self.GetPosition());
		}

		
		protected WorldCharacter self;

		
		protected WorldCharacter caster;

		
		protected float createdTime;

		
		protected float durationPauseEndTime;

		
		private float duration;

		
		private float originalDuration;

		
		protected int stackCount;

		
		private List<StatParameter> externalStats = new List<StatParameter>();

		
		private float externalNonCalculateStatValue;

		
		private CharacterStateData stateData;

		
		private int reserveCount;

		
		private StateEffectStatCalculator statCalculator;

		
		protected Dictionary<StatType, float> stats = new Dictionary<StatType, float>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);

		
		public CharacterState.CalculateStat OnCalculateStat = delegate()
		{
		};

		
		protected SkillUseInfo skillUseInfo;

		
		private int isSkillPause;

		
		private float lastStatCalculationTick;

		
		private const float StatCalculationTick = 0.2f;

		
		public delegate void CalculateStat();
	}
}
