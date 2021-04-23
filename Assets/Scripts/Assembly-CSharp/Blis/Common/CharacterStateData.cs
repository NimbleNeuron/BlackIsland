using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class CharacterStateData
	{
		public readonly float calculatorParameter;


		public readonly bool canReserve;


		public readonly int code;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StatType coefStatType1;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StatType coefStatType2;


		public readonly float coefStatValue1;


		public readonly float coefStatValue2;


		public readonly float duration;


		public readonly float forcedMoveSpeed;


		public readonly int group;


		public readonly int level;


		public readonly int maxStack;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StatType nonCalculateStatType;


		public readonly float nonCalculateStatValue;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StatType statType1;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StatType statType2;


		public readonly float statValue1;


		public readonly float statValue2;


		[JsonIgnore] private CharacterStateGroupData groupData;


		[JsonConstructor]
		public CharacterStateData(int code, int group, int level, float duration, int maxStack,
			float calculatorParameter, StatType nonCalculateStatType, float nonCalculateStatValue, StatType statType1,
			float statValue1, StatType coefStatType1, float coefStatValue1, StatType statType2, float statValue2,
			StatType coefStatType2, float coefStatValue2, float forcedMoveSpeed, bool canReserve)
		{
			this.code = code;
			this.group = group;
			this.level = level;
			this.duration = duration;
			this.maxStack = maxStack;
			this.calculatorParameter = calculatorParameter;
			this.nonCalculateStatType = nonCalculateStatType;
			this.nonCalculateStatValue = nonCalculateStatValue;
			this.statType1 = statType1;
			this.statValue1 = statValue1;
			this.coefStatType1 = coefStatType1;
			this.coefStatValue1 = coefStatValue1;
			this.statType2 = statType2;
			this.statValue2 = statValue2;
			this.coefStatType2 = coefStatType2;
			this.coefStatValue2 = coefStatValue2;
			this.forcedMoveSpeed = forcedMoveSpeed;
			this.canReserve = canReserve;
		}


		[JsonIgnore]
		public CharacterStateGroupData GroupData {
			get
			{
				CharacterStateGroupData result;
				if ((result = groupData) == null)
				{
					result = groupData = GameDB.characterState.GetGroupData(group);
				}

				return result;
			}
		}
	}
}