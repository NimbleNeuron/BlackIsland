using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class CharacterVoiceDB
	{
		private List<CharacterVoiceData> charVoiceDataList;


		private List<CharacterVoiceRandomCountData> charVoiceRandomCountList;

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(CharacterVoiceData))
			{
				charVoiceDataList = data.Cast<CharacterVoiceData>().ToList<CharacterVoiceData>();
				return;
			}

			if (typeFromHandle == typeof(CharacterVoiceRandomCountData))
			{
				charVoiceRandomCountList =
					data.Cast<CharacterVoiceRandomCountData>().ToList<CharacterVoiceRandomCountData>();
			}
		}


		public CharacterVoiceData GetCharacterVoiceData(CharacterVoiceType charVoiceType)
		{
			return charVoiceDataList.FirstOrDefault(x => x.useCase == charVoiceType);
		}


		public CharacterVoiceRandomCountData GetCharacterVoiceRandomCountData(CharacterVoiceType charVoiceType,
			int characterCode, int skinIndex, SkillSlotSet skillSet)
		{
			if (charVoiceType == CharacterVoiceType.PlaySkillActive ||
			    charVoiceType == CharacterVoiceType.PlaySkillPassive)
			{
				return (from x in charVoiceRandomCountList
					where x.useCase == charVoiceType && x.characterCode == characterCode && x.skillSet == skillSet
					select x).LastOrDefault(x => x.skinIndex == skinIndex || x.skinIndex == 0);
			}

			return (from x in charVoiceRandomCountList
				where x.useCase == charVoiceType && x.characterCode == characterCode
				select x).LastOrDefault(x => x.skinIndex == skinIndex || x.skinIndex == 0);
		}
	}
}