using System.Collections.Generic;
using System.Linq;
using Blis.Client;

namespace Blis.Common
{
	public class EmotionDB
	{
		private List<EmotionIconData> emotionList;

		public EmotionIconData GetEmotionIconData(int code)
		{
			return emotionList.Find(data => data.code == code);
		}


		public List<EmotionIconData> GetEmotionIconData(bool isCheckHasEmotion)
		{
			List<EmotionIconData> list = new List<EmotionIconData>();
			for (int i = 0; i < emotionList.Count; i++)
			{
				if (isCheckHasEmotion || emotionList[i].purchaseType == EmotionPurchaseType.FREE ||
				    Lobby.inst.IsHasEmotion(emotionList[i].code))
				{
					list.Add(emotionList[i]);
				}
			}

			return (from x in list
				orderby x.purchaseType, Lobby.inst.IsHasEmotion(x.code), x.code
				select x).ToList<EmotionIconData>();
		}


		public void SetData<T>(List<T> data)
		{
			if (typeof(T) == typeof(EmotionIconData))
			{
				emotionList = data.Cast<EmotionIconData>().ToList<EmotionIconData>();
			}
		}
	}
}