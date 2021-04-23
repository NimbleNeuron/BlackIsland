using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client
{
	public class LocalCharacterStat : CharacterStatBase
	{
		private readonly Dictionary<StatType, LocalCharacterStatValue> values =
			new Dictionary<StatType, LocalCharacterStatValue>(
				SingletonComparerEnum<StatTypeComparer, StatType>.Instance);


		public override float SightRange => GetValue(StatType.SightRange);


		public override float SightRangeRatio => GetValue(StatType.SightRangeRatio);


		public override int SightAngle => GetIntValue(StatType.SightAngle);


		public override float AttackRange => GetValue(StatType.AttackRange);


		protected override void CheckNull(StatType statType)
		{
			if (!values.ContainsKey(statType))
			{
				values.Add(statType, new LocalCharacterStatValue(statType, 0));
			}
		}


		public override float GetValue(StatType statType)
		{
			CheckNull(statType);
			return values[statType].GetValue();
		}


		public override float GetValue(StatType statType, bool shuffle)
		{
			CheckNull(statType);
			return values[statType].GetValue(shuffle);
		}


		protected override int GetIntValue(StatType statType)
		{
			CheckNull(statType);
			return values[statType].GetIntValue();
		}


		protected override int GetIntValue(StatType statType, bool shuffle)
		{
			CheckNull(statType);
			return values[statType].GetIntValue(shuffle);
		}


		protected override void SetValue(StatType statType, float value)
		{
			CheckNull(statType);
			values[statType].Set(value);
		}


		protected override void SetIntValue(StatType statType, int value)
		{
			CheckNull(statType);
			values[statType].Set(value);
		}


		public override CharacterStatValue GetStatValue(StatType statType)
		{
			CheckNull(statType);
			return values[statType];
		}
	}
}