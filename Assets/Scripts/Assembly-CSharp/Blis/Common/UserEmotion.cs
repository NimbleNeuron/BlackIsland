namespace Blis.Common
{
	public class UserEmotion
	{
		public readonly int emotionCode;


		public readonly long userNum;

		public UserEmotion(long userNum, int emotionCode)
		{
			this.userNum = userNum;
			this.emotionCode = emotionCode;
		}
	}
}