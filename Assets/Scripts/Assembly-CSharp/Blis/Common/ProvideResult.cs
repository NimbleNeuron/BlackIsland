using Newtonsoft.Json;

namespace Blis.Common
{
	public class ProvideResult
	{
		[JsonProperty("cha")] public readonly Character character;


		[JsonProperty("gs")] public readonly Goods goods;

		[JsonConstructor]
		public ProvideResult(Goods goods, Character character)
		{
			this.goods = goods;
			this.character = character;
		}
	}
}