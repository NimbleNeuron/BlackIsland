using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject(false)]
	public class CharacterStateValue
	{
		
		
		[Key(2)]
		public float Duration
		{
			get
			{
				return this.duration;
			}
		}

		
		
		[Key(3)]
		public int StackCount
		{
			get
			{
				return this.stackCount;
			}
		}

		
		
		[Key(4)]
		public int ReserveCount
		{
			get
			{
				return this.reserveCount;
			}
		}

		
		
		[Key(5)]
		public int CasterId
		{
			get
			{
				return this.casterId;
			}
		}

		
		
		[Key(6)]
		public float OriginalDuration
		{
			get
			{
				return this.originalDuration;
			}
		}

		
		
		[IgnoreMember]
		public float DurationPauseEndTime
		{
			get
			{
				return this.durationPauseEndTime;
			}
		}

		
		
		[IgnoreMember]
		public int Group
		{
			get
			{
				return this.group;
			}
		}

		
		
		[IgnoreMember]
		public int MaxStack
		{
			get
			{
				return this.maxStack;
			}
		}

		
		
		[IgnoreMember]
		public EffectType EffectType
		{
			get
			{
				return this.effectType;
			}
		}

		
		
		[IgnoreMember]
		public StateType StateType
		{
			get
			{
				return this.stateType;
			}
		}

		
		[SerializationConstructor]
		public CharacterStateValue(int code, float createdTime, float duration, int stackCount, int reserveCount, int casterId, float originalDuration)
		{
			this.code = code;
			this.stackCount = stackCount;
			this.reserveCount = reserveCount;
			this.duration = duration;
			this.createdTime = createdTime;
			this.casterId = casterId;
			this.originalDuration = originalDuration;
			CharacterStateData data = GameDB.characterState.GetData(code);
			this.group = data.group;
			this.maxStack = data.maxStack;
			this.effectType = data.GroupData.effectType;
			this.stateType = GameDB.characterState.GetGroupData(data.group).stateType;
			this.durationPauseEndTime = 0f;
		}

		
		public long GetUniqueCode()
		{
			return ((long)this.group << 32) + (long)this.casterId;
		}

		
		public void SetCreateTime(float createdTime)
		{
			this.createdTime = createdTime;
		}

		
		public void SetStackCount(int stack)
		{
			this.stackCount = stack;
		}

		
		public void SetReserveCount(int reserve)
		{
			this.reserveCount = reserve;
		}

		
		public void SetDuration(float duration)
		{
			this.duration = duration;
		}

		
		public void SetDurationPauseEndTime(float durationPauseEndTime)
		{
			this.durationPauseEndTime = durationPauseEndTime;
		}

		
		[Key(0)]
		public readonly int code;

		
		[Key(1)]
		public float createdTime;

		
		private float duration;

		
		private int stackCount;

		
		private int reserveCount;

		
		private readonly int casterId;

		
		private readonly float originalDuration;

		
		private float durationPauseEndTime;

		
		private readonly int group;

		
		private readonly int maxStack;

		
		private readonly EffectType effectType;

		
		private readonly StateType stateType;
	}
}
