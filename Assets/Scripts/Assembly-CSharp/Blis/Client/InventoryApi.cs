using System;
using System.Collections.Generic;
using Blis.Common;
using Neptune.Http;

namespace Blis.Client
{
	public static class InventoryApi
	{
		public static Func<HttpRequest> GetInventoryCharacter()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/inventory/character/", Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetInventorySkin()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/inventory/skin/", Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetInventoryEmotion()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/inventory/emotion/", Array.Empty<object>()));
		}


		public static Func<HttpRequest> SetInventoryEmotionSlot(List<UserEmoticonSlot> slotData)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/inventory/emotion/setSlot", Array.Empty<object>()),
				slotData);
		}


		public class UserEmoticonSlot
		{
			public int emotionCode;


			public EmotionPlateType slotType;


			public long userNum;

			public UserEmoticonSlot(EmotionPlateType slotType, int emotionCode)
			{
				this.slotType = slotType;
				this.emotionCode = emotionCode;
			}
		}


		public class EmotionResult
		{
			public List<UserEmoticonSlot> userEmotionSlots;
		}
	}
}