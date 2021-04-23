using Newtonsoft.Json;

namespace Blis.Common
{
	public class CriticalChanceData
	{
		public readonly float actualUse;


		public readonly int probability;

		[JsonConstructor]
		public CriticalChanceData(int probability, float actualUse)
		{
			this.probability = probability;
			this.actualUse = actualUse;
		}
	}
}