using Newtonsoft.Json;

namespace Blis.Common
{
	public class NearByAreaData
	{
		public readonly int areaCode;


		public readonly int code;


		public readonly int nearByAreaCode;

		[JsonConstructor]
		public NearByAreaData(int code, int areaCode, int nearByAreaCode)
		{
			this.code = code;
			this.areaCode = areaCode;
			this.nearByAreaCode = nearByAreaCode;
		}
	}
}