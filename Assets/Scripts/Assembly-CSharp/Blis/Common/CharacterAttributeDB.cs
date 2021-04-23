using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	
	public class CharacterAttributeDB
	{
		
		private List<CharacterAttributeData> characterAttributeList;

		
		public List<CharacterAttributeData> GetCharacterAttributeDatas(int characterCode)
		{
			return characterAttributeList.FindAll(data => data.characterCode == characterCode);
		}

		
		public void SetData<T>(List<T> data)
		{
			if (typeof(T) == typeof(CharacterAttributeData))
			{
				characterAttributeList = data.Cast<CharacterAttributeData>().ToList<CharacterAttributeData>();
			}
		}
	}
}