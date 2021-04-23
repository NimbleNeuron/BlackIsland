using Newtonsoft.Json;


public class TutorialRewardData
{
	
	[JsonConstructor]
	public TutorialRewardData(int tutorialKey, string goodsType, int collectionCode, int amount)
	{
		this.tutorialKey = tutorialKey;
		this.goodsType = goodsType;
		this.collectionCode = collectionCode;
		this.amount = amount;
	}

	
	public readonly int tutorialKey;

	
	public readonly string goodsType;

	
	public readonly int collectionCode;

	
	public readonly int amount;
}
