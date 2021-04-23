namespace Blis.Client
{
	public class RewardItemInfo
	{
		public RewardItemType itemType;


		public int itemValue;

		public RewardItemInfo(RewardItemType itemType, int itemValue)
		{
			this.itemType = itemType;
			this.itemValue = itemValue;
		}
	}
}