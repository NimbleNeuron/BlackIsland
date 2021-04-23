using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	public class SimpleCharacterStat : CharacterStatBase
	{
		
		private Dictionary<StatType, ServerCharacterStatValue> values =
			new Dictionary<StatType, ServerCharacterStatValue>(SingletonComparerEnum<StatTypeComparer, StatType>
				.Instance);

		
		protected override void CheckNull(StatType statType)
		{
			if (!values.ContainsKey(statType))
			{
				values.Add(statType, new ServerCharacterStatValue(statType, 0));
			}
		}

		
		public override float GetValue(StatType statType)
		{
			CheckNull(statType);
			return values[statType].GetValue();
		}

		
		public override float GetValue(StatType statType, bool shuffle)
		{
			return GetValue(statType);
		}

		
		protected override int GetIntValue(StatType statType)
		{
			CheckNull(statType);
			return values[statType].GetIntValue();
		}

		
		protected override int GetIntValue(StatType statType, bool shuffle)
		{
			return GetIntValue(statType);
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

		
		public void CopyStats(CharacterStat characterStat)
		{
			characterStat.CopyValues(ref values);
		}
	}
}