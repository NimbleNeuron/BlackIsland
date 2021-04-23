using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	
	public class CharacterSkillVideoDB
	{
		
		private readonly Dictionary<int, CharacterSkillVideoData> videoDatas =
			new Dictionary<int, CharacterSkillVideoData>();

		
		public void SetData<T>(List<T> data)
		{
			if (typeof(T) == typeof(CharacterSkillVideoData))
			{
				List<CharacterSkillVideoData> list = data.Cast<CharacterSkillVideoData>()
					.ToList<CharacterSkillVideoData>();
				for (int i = 0; i < list.Count; i++)
				{
					videoDatas[list[i].code] = list[i];
				}
			}
		}

		
		public CharacterSkillVideoData GetData(int code)
		{
			if (!videoDatas.ContainsKey(code))
			{
				return null;
			}

			return videoDatas[code];
		}
	}
}