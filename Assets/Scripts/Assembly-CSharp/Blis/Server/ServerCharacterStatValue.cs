using Blis.Common;
using MessagePack;
using UnityEngine;

namespace Blis.Server
{
	
	[MessagePackObject()]
	public class ServerCharacterStatValue : CharacterStatValue
	{
		
		[IgnoreMember] private int value;

		
		[SerializationConstructor]
		public ServerCharacterStatValue(StatType statType, int value) : base(statType)
		{
			this.value = value;
		}

		
		
		[Key(1)] public int InternalValue => value;

		
		public override void Set(float v)
		{
			value = Mathf.RoundToInt(v * 100f);
		}

		
		public override void Set(int v)
		{
			value = Mathf.RoundToInt(v * 100f);
		}

		
		public override int GetIntValue()
		{
			return Mathf.RoundToInt(value / 100f);
		}

		
		public override int GetIntValue(bool shuffle)
		{
			return GetIntValue();
		}

		
		public override float GetValue()
		{
			return value / 100f;
		}

		
		public override float GetValue(bool shuffle)
		{
			return GetValue();
		}

		
		public static float GetBiasedValue(float v)
		{
			return Mathf.RoundToInt(v * 100f) / 100f;
		}
	}
}