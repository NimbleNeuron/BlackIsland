using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class TooltipMastery : BaseUI
	{
		[SerializeField] private List<UIMasteryItem> masteryItems = default;


		[SerializeField] private List<GameObject> lines = default;

		public void ShowTooltip(Dictionary<MasteryType, int> masterys, Vector3 position)
		{
			if (masterys == null || masterys.Count == 0)
			{
				return;
			}

			int num = 0;
			foreach (KeyValuePair<MasteryType, int> keyValuePair in masterys)
			{
				masteryItems[num].ShowMastery(keyValuePair.Key, keyValuePair.Value);
				num++;
			}

			SetLines(num);
			transform.position = position;
			gameObject.SetActive(true);
		}


		public void HideTooltip()
		{
			foreach (UIMasteryItem uimasteryItem in masteryItems)
			{
				uimasteryItem.HideMastery();
			}

			foreach (GameObject g in lines)
			{
				g.SetActive(false);
			}

			gameObject.SetActive(false);
		}


		private void SetLines(int count)
		{
			for (int i = 0; i <= count; i++)
			{
				lines[i].SetActive(true);
			}
		}
	}
}