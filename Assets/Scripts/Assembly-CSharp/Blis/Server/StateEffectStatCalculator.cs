using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	public abstract class StateEffectStatCalculator
	{
		
		
		protected WorldCharacter Self
		{
			get
			{
				return this.state.Self;
			}
		}

		
		
		protected float CreatedTime
		{
			get
			{
				return this.state.CreatedTime;
			}
		}

		
		
		protected float CalculatorParameter
		{
			get
			{
				return this.state.StateData.calculatorParameter;
			}
		}

		
		
		protected StatType NonCalculateStatStatType
		{
			get
			{
				return this.state.StateData.nonCalculateStatType;
			}
		}

		
		
		protected float NonCalculateStatValue
		{
			get
			{
				return this.state.StateData.nonCalculateStatValue;
			}
		}

		
		
		protected StatType StatType1
		{
			get
			{
				return this.state.StateData.statType1;
			}
		}

		
		
		protected float StatValue1
		{
			get
			{
				return this.state.StateData.statValue1;
			}
		}

		
		
		protected StatType StatType2
		{
			get
			{
				return this.state.StateData.statType2;
			}
		}

		
		
		protected float StatValue2
		{
			get
			{
				return this.state.StateData.statValue2;
			}
		}

		
		
		protected StatType CoefStatType1
		{
			get
			{
				return this.state.StateData.coefStatType1;
			}
		}

		
		
		protected StatType CoefStatType2
		{
			get
			{
				return this.state.StateData.coefStatType2;
			}
		}

		
		
		protected float CoefStatValue1
		{
			get
			{
				return this.state.StateData.coefStatValue1;
			}
		}

		
		
		protected float CoefStatValue2
		{
			get
			{
				return this.state.StateData.coefStatValue2;
			}
		}

		
		protected StateEffectStatCalculator(CharacterState state)
		{
			this.state = state;
		}

		
		public bool Calculation(int stackCount, float addFixedStatValue, List<StatParameter> externalStats, ref Dictionary<StatType, float> stats)
		{
			this.calcResultStats.Clear();
			if (this.NonCalculateStatStatType != StatType.None)
			{
				this.calcResultStats.Add(new KeyValuePair<StatType, float>(this.NonCalculateStatStatType, (this.NonCalculateStatValue + addFixedStatValue) * (float)stackCount));
			}
			if (this.StatType1 != StatType.None)
			{
				this.calcResultStats.Add(new KeyValuePair<StatType, float>(this.StatType1, this.CalcStat(this.StatType1, this.StatValue1, this.CoefStatType1, this.CoefStatValue1) * (float)stackCount));
			}
			if (this.StatType2 != StatType.None)
			{
				this.calcResultStats.Add(new KeyValuePair<StatType, float>(this.StatType2, this.CalcStat(this.StatType2, this.StatValue2, this.CoefStatType2, this.CoefStatValue2) * (float)stackCount));
			}
			for (int i = 0; i < externalStats.Count; i++)
			{
				StatParameter statParameter = externalStats[i];
				this.calcResultStats.Add(new KeyValuePair<StatType, float>(statParameter.statType, this.CalcStat(statParameter.statType, statParameter.statValue, statParameter.coefStatType, statParameter.coefStatValue) * (float)stackCount));
			}
			Dictionary<StatType, float> dictionary = new Dictionary<StatType, float>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);
			for (int j = 0; j < this.calcResultStats.Count; j++)
			{
				this.MergeStats(this.calcResultStats[j].Key, this.calcResultStats[j].Value, ref dictionary);
			}
			bool flag = !this.Equals<StatType, float>(stats, dictionary);
			if (flag)
			{
				stats = dictionary;
			}
			return flag;
		}

		
		protected virtual float CalcStat(StatType statType, float value, StatType coefType, float coefValue)
		{
			return value;
		}

		
		private void MergeStats(StatType statType, float result, ref Dictionary<StatType, float> newStats)
		{
			if (statType == StatType.None)
			{
				return;
			}
			if (!newStats.ContainsKey(statType))
			{
				newStats.Add(statType, result);
				return;
			}
			Dictionary<StatType, float> dictionary = newStats;
			dictionary[statType] += result;
		}

		
		protected bool Equals<TKey, TValue>(IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y)
		{
			if (y == null)
			{
				return x == null;
			}
			if (x == null)
			{
				return false;
			}
			if (x == y)
			{
				return true;
			}
			if (x.Count != y.Count)
			{
				return false;
			}
			foreach (TKey key in x.Keys)
			{
				if (!y.ContainsKey(key))
				{
					return false;
				}
			}
			foreach (TKey key2 in x.Keys)
			{
				TValue tvalue = x[key2];
				if (!tvalue.Equals(y[key2]))
				{
					return false;
				}
			}
			return true;
		}

		
		private CharacterState state;

		
		protected readonly List<KeyValuePair<StatType, float>> calcResultStats = new List<KeyValuePair<StatType, float>>();
	}
}
