using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class TierInfoWindow : BaseWindow
	{
		private Transform tierInfo;


		private TierInfoSlot[] tierInfoSlots;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			tierInfo = transform.Find("Frame/Content");
			tierInfoSlots = tierInfo.GetComponentsInChildren<TierInfoSlot>(true);
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			Dictionary<RankingTierType, List<RankingTier>> dictionary =
				new Dictionary<RankingTierType, List<RankingTier>>();
			for (int i = 0; i < InfoService.GetRankingTiersCount(); i++)
			{
				RankingTier rankingTier = InfoService.GetRankingTier(i);
				if (!dictionary.ContainsKey(rankingTier.tierType))
				{
					dictionary.Add(rankingTier.tierType, new List<RankingTier>());
				}

				dictionary[rankingTier.tierType].Add(rankingTier);
			}

			int num = 0;
			foreach (object obj in Enum.GetValues(typeof(RankingTierType)))
			{
				RankingTierType key = (RankingTierType) obj;
				if (dictionary.ContainsKey(key))
				{
					tierInfoSlots[num].SetInfo(dictionary[key]);
					num++;
				}
			}

			tierInfo.gameObject.SetActive(true);
		}


		protected override void OnClose()
		{
			base.OnClose();
			tierInfo.gameObject.SetActive(false);
		}
	}
}