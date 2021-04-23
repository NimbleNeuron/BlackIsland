namespace Blis.Common
{
	public class AreaData
	{
		public readonly int code;


		public readonly int maskCode;


		public readonly string name;

		public AreaData(int code, string name, int maskCode)
		{
			this.code = code;
			this.name = name;
			this.maskCode = 1 << maskCode;
		}
	}
}