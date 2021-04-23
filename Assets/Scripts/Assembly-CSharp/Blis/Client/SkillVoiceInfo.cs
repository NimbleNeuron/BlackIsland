using System;

namespace Blis.Client
{
	[Serializable]
	public class SkillVoiceInfo : BaseParam
	{
		public string charName = "";


		public string soundName = "";


		public int count;


		public int maxDistance;


		public string tag = "";


		public override object Clone()
		{
			return new SkillVoiceInfo
			{
				charName = charName,
				soundName = soundName,
				count = count,
				maxDistance = maxDistance,
				tag = tag
			};
		}
	}
}