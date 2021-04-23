using Newtonsoft.Json;

namespace Blis.Common
{
	public class BotAiModelData
	{
		public readonly string id;


		public readonly string model;

		[JsonConstructor]
		public BotAiModelData(string id, string model)
		{
			this.id = id;
			this.model = model;
		}
	}
}