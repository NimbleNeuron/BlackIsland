using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TierInfoSlot : BaseUI
	{
		[SerializeField] private Text[] tierNames = default;


		private Image tierIcon;


		private GameObject tierInfo;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			tierIcon = GameUtil.Bind<Image>(gameObject, "Ico_Tier");
			tierInfo = transform.Find("Info").gameObject;
			for (int i = 0; i < tierInfo.transform.childCount; i++)
			{
				tierNames[i] = GameUtil.Bind<Text>(tierInfo, string.Format("TierGrade_{0}/Name", i));
			}
		}


		public void SetInfo(List<RankingTier> rankingTier)
		{
			if (rankingTier != null && rankingTier.Count > 0)
			{
				tierIcon.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(rankingTier[0].tierType);
				for (int i = 0; i < tierInfo.transform.childCount; i++)
				{
					if (i < rankingTier.Count)
					{
						tierNames[i].gameObject.SetActive(true);
						tierNames[i].text =
							rankingTier[i].tierType != RankingTierType.Unrank &&
							rankingTier[i].tierType != RankingTierType.Normal
								? string.Format(
									rankingTier[i].tierType.GetName() + " " + rankingTier[i].tierGrade.GetName(),
									Array.Empty<object>())
								: string.Format(rankingTier[i].tierType.GetName() ?? "", Array.Empty<object>());
					}
					else
					{
						tierNames[i].gameObject.SetActive(false);
					}
				}
			}
		}
	}
}