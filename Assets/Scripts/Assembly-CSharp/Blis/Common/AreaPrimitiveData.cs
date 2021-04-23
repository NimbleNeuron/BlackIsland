using Newtonsoft.Json;

namespace Blis.Common
{
	public class AreaPrimitiveData
	{
		public readonly int code;


		public readonly int maskCode;


		public readonly string name;


		[JsonConstructor]
		public AreaPrimitiveData(int code, string name, int maskCode)
		{
			this.code = code;
			this.name = name;
			this.maskCode = maskCode;
		}
	}
}