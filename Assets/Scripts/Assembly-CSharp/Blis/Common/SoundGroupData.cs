using Newtonsoft.Json;

namespace Blis.Common
{
	public class SoundGroupData
	{
		public readonly int code;


		public readonly string fileName;


		public readonly string groupName;


		public readonly int rate;

		[JsonConstructor]
		public SoundGroupData(int code, string groupName, string fileName, int rate)
		{
			this.code = code;
			this.groupName = groupName;
			this.fileName = fileName;
			this.rate = rate;
		}
	}
}