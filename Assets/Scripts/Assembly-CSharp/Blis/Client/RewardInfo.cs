using System.Collections.Generic;

namespace Blis.Client
{
	public class RewardInfo
	{
		public List<RewardItemInfo> itemInfos;


		public RewardType rewardType;

		public RewardInfo(RewardType rewardType, List<RewardItemInfo> itemInfos)
		{
			this.rewardType = rewardType;
			this.itemInfos = itemInfos;
		}
	}
}