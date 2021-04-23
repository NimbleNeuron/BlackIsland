namespace Blis.Common
{
	public class Skin
	{
		public readonly int skinCode;


		public readonly long userNum;

		public Skin(long userNum, int skinCode)
		{
			this.userNum = userNum;
			this.skinCode = skinCode;
		}
	}
}