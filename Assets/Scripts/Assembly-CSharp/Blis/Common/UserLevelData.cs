using Newtonsoft.Json;

namespace Blis.Common
{
	public class UserLevelData
	{
		public readonly int accumulateExp;


		public readonly int level;


		public readonly int needExp;


		public readonly int rewardAcoin;

		[JsonConstructor]
		public UserLevelData(int level, int needExp, int accumulateExp, int rewardAcoin)
		{
			this.level = level;
			this.needExp = needExp;
			this.accumulateExp = accumulateExp;
			this.rewardAcoin = rewardAcoin;
		}
	}
}