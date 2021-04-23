using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class RecommendDB
	{
		private List<RecommendArea> recommendAreaList;


		private List<RecommendItem> recommendItems;


		private List<RecommendStarting> recommendStartingList;


		private List<StartItem> startItems;

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(RecommendStarting))
			{
				recommendStartingList = data.Cast<RecommendStarting>().ToList<RecommendStarting>();
				return;
			}

			if (typeFromHandle == typeof(RecommendArea))
			{
				recommendAreaList = data.Cast<RecommendArea>().ToList<RecommendArea>();
				return;
			}

			if (typeFromHandle == typeof(RecommendItem))
			{
				recommendItems = data.Cast<RecommendItem>().ToList<RecommendItem>();
				return;
			}

			if (typeFromHandle == typeof(StartItem))
			{
				startItems = data.Cast<StartItem>().ToList<StartItem>();
			}
		}


		public RecommendStarting FindStartingData(int characterCode, MasteryType masteryType)
		{
			return recommendStartingList.Find(x => x.characterCode == characterCode && x.mastery == masteryType);
		}


		public RecommendArea FindRecommendAreaData(int characterCode, MasteryType masteryType)
		{
			RecommendArea recommendArea =
				recommendAreaList.Find(x => x.characterCode == characterCode && x.mastery == masteryType);
			if (recommendArea == null)
			{
				Log.E("Failed to find RecommendAreaData by characterCode[{0}] masteryType[{1}]", characterCode,
					masteryType);
			}

			return recommendArea;
		}


		public List<RecommendArea> FindRecommendAreaDatas(int characterCode, MasteryType masteryType)
		{
			return recommendAreaList.FindAll(x => x.characterCode == characterCode && x.mastery == masteryType);
		}


		public RecommendStarting FindRecommendStarting(int code)
		{
			return recommendStartingList.Find(x => x.code == code);
		}


		public List<RecommendItem> FindRecommendItems(int characterCode, MasteryType masteryType,
			RecommendItemType recommendItemType)
		{
			return recommendItems.FindAll(x =>
				x.characterCode == characterCode && x.mastery == masteryType &&
				x.recommendItemType == recommendItemType);
		}


		public List<StartItem> GetStartItem(int groupCode)
		{
			return startItems.FindAll(x => x.groupCode == groupCode);
		}
	}
}