using Newtonsoft.Json;

namespace Blis.Common
{
	public class FootstepData
	{
		public readonly int code;
		public readonly string groupName;
		public readonly string material;
		public readonly int maxDistance;

		[JsonConstructor]
		public FootstepData(int code, string material, string groupName, int maxDistance)
		{
			this.code = code;
			this.material = material;
			this.groupName = groupName;
			this.maxDistance = maxDistance;
		}
	}
}