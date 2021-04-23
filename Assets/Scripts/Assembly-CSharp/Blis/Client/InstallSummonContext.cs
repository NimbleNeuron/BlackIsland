using Blis.Common;

namespace Blis.Client
{
	public class InstallSummonContext
	{
		public readonly float createRange;


		public readonly int itemId;


		public readonly SummonData summonData;

		public InstallSummonContext(SummonData summonData, int itemId)
		{
			this.summonData = summonData;
			createRange = summonData.createRange;
			this.itemId = itemId;
		}
	}
}