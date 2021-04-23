using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class MissionData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MissionCheck check;


		public readonly int code;


		public readonly int conditionItemCode;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MissionConditionType conditionType;


		public readonly int count;


		public readonly int key;


		public readonly int probability;


		public readonly int reqMaxLevel;


		public readonly int reqMinLevel;


		public readonly int rewardAcoin;


		public readonly int rewardExp;


		public readonly int seq;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MissionType type;

		[JsonConstructor]
		public MissionData(int key, int code, int seq, MissionType type, int reqMinLevel, int reqMaxLevel,
			MissionConditionType conditionType, int conditionItemCode, MissionCheck check, int count, int rewardAcoin,
			int rewardExp, int probability)
		{
			this.key = key;
			this.code = code;
			this.seq = seq;
			this.type = type;
			this.reqMinLevel = reqMinLevel;
			this.reqMaxLevel = reqMaxLevel;
			this.conditionType = conditionType;
			this.conditionItemCode = conditionItemCode;
			this.check = check;
			this.count = count;
			this.rewardAcoin = rewardAcoin;
			this.rewardExp = rewardExp;
			this.probability = probability;
		}
	}
}