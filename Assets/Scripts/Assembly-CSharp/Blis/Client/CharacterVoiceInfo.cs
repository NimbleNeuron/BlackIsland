namespace Blis.Client
{
	public class CharacterVoiceInfo
	{
		public CharacterVoiceType charVoiceType;


		public int playVoiceCount;


		
		public float StartTime { get; set; }


		
		public float CoolTime { get; set; }


		public void AddCoolTime(float addTime)
		{
			CoolTime += addTime;
		}
	}
}