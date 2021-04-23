using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIMasteryInfo : UITooltipComponent
	{
		[SerializeField] private Text masteryName = default;


		[SerializeField] private Text level = default;


		[SerializeField] private UIProgress progress = default;


		[SerializeField] private Text desc = default;


		[SerializeField] private RectTransform masteryRectTransform = default;


		[SerializeField] private List<UIMasteryExp> masteryExps = default;

		public void SetMasteryInfo(MasteryType masteryType, int level, int exp, int maxExp)
		{
			this.level.text = level.ToString();
			masteryName.text = LnUtil.GetMasteryName(masteryType);
			progress.SetValue(exp, maxExp);
			desc.text = LnUtil.GetMasteryDesc(masteryType, level);
			transform.localScale = Vector3.one;
		}


		public void SetAcquisitionMethod(MasteryType masteryType)
		{
			masteryRectTransform.gameObject.SetActive(masteryType > MasteryType.None);
			if (masteryType != MasteryType.None)
			{
				List<MasteryExpData> list = GameDB.mastery.FindMasteryExpData(masteryType);
				Dictionary<MasteryConditionType, MasteryGain> dictionary =
					new Dictionary<MasteryConditionType, MasteryGain>(
						SingletonComparerEnum<MasteryConditionTypeComparer, MasteryConditionType>.Instance);
				for (int i = 0; i < list.Count; i++)
				{
					MasteryConditionType conditionType = list[i].conditionType;
					if (!dictionary.ContainsKey(conditionType))
					{
						dictionary.Add(conditionType, new MasteryGain(conditionType));
					}

					dictionary[conditionType].SetValue(GetMasteryExp(masteryType, list[i]));
				}

				float num = 30f;
				int num2 = 0;
				masteryExps.ForEach(delegate(UIMasteryExp x) { x.gameObject.SetActive(false); });
				foreach (KeyValuePair<MasteryConditionType, MasteryGain> keyValuePair in dictionary)
				{
					if (num2 < masteryExps.Count)
					{
						masteryExps[num2].SetCategory(LnUtil.GetMasteryCondition(keyValuePair.Key));
						masteryExps[num2].SetValue(keyValuePair.Value.GetValueString());
						masteryExps[num2].gameObject.SetActive(true);
						RectTransform rectTransform = (RectTransform) masteryExps[num2].transform;
						num += rectTransform.rect.height;
					}

					num2++;
				}

				masteryRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num);
			}
		}


		private int GetMasteryExp(MasteryType masteryType, MasteryExpData masteryExpData)
		{
			if (masteryType.IsWeaponMastery())
			{
				if (masteryExpData.masteryType1 == MasteryType.None && masteryExpData.value1 > 0)
				{
					return masteryExpData.value1;
				}

				if (masteryExpData.masteryType2 == MasteryType.None && masteryExpData.value2 > 0)
				{
					return masteryExpData.value2;
				}

				if (masteryExpData.masteryType3 == MasteryType.None && masteryExpData.value3 > 0)
				{
					return masteryExpData.value3;
				}
			}

			if (masteryExpData.masteryType1 == masteryType)
			{
				return masteryExpData.value1;
			}

			if (masteryExpData.masteryType2 == masteryType)
			{
				return masteryExpData.value2;
			}

			if (masteryExpData.masteryType3 == masteryType)
			{
				return masteryExpData.value3;
			}

			return 0;
		}


		public override void Clear()
		{
			base.Clear();
			transform.localScale = Vector3.zero;
		}


		private class MasteryGain
		{
			private int max = int.MinValue;


			private int min = int.MaxValue;


			public MasteryGain(MasteryConditionType masteryGainType)
			{
				MasteryGainType = masteryGainType;
			}


			public MasteryConditionType MasteryGainType { get; }


			public void SetValue(int value)
			{
				if (min >= value)
				{
					min = value;
				}

				if (max <= value)
				{
					max = value;
				}
			}


			public string GetValueString()
			{
				if (min != max)
				{
					return string.Format("{0} ~ {1}", min, max);
				}

				return string.Format("{0}", min);
			}
		}
	}
}