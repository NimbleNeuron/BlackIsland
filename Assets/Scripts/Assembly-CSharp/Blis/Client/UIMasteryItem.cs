using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIMasteryItem : BaseControl
	{
		public delegate void OnClickHelpButton(LocalMastery masteryInfo);


		[SerializeField] private UIProgressBlock masteryProgress = default;


		[SerializeField] private Image glow = default;


		private int exp;


		private int level;


		private LnText masteryLevel;


		private LnText masteryName;


		private MasteryType masteryType;


		private int maxExp;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			masteryName = GameUtil.Bind<LnText>(gameObject, "Label");
			masteryLevel = GameUtil.Bind<LnText>(gameObject, "Lv/Level");
		}


		public MasteryType GetMasteryType()
		{
			return masteryType;
		}


		public void ShowMastery(MasteryType masteryType, int level)
		{
			this.masteryType = masteryType;
			this.level = level;
			masteryName.text = LnUtil.GetMasteryName(masteryType);
			masteryLevel.text = string.Format("{0}", level);
			gameObject.SetActive(true);
		}


		public void HideMastery()
		{
			gameObject.SetActive(false);
		}


		public void SetMasteryData(MasteryType masteryType, int level, int exp, int maxExp)
		{
			this.masteryType = masteryType;
			this.level = level;
			this.exp = exp;
			this.maxExp = maxExp;
			if (masteryType != MasteryType.None)
			{
				masteryName.text = LnUtil.GetMasteryName(masteryType);
				masteryLevel.text = string.Format("{0}", level);
				if (masteryProgress != null)
				{
					masteryProgress.FillBlock(level - 1, exp / (float) maxExp);
				}
			}
			else
			{
				masteryName.text = null;
				masteryLevel.text = null;
				if (masteryProgress != null)
				{
					masteryProgress.ClearBlock();
				}
			}

			if (IsHover)
			{
				ShowTooltip();
			}

			gameObject.SetActive(true);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			glow.gameObject.SetActive(true);
			ShowTooltip();
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			glow.gameObject.SetActive(false);
			MonoBehaviourInstance<Tooltip>.inst.Hide(GetParentWindow());
		}


		private void ShowTooltip()
		{
			if (masteryType != MasteryType.None)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetMastery(masteryType, level, exp, maxExp);
				MonoBehaviourInstance<Tooltip>.inst.ShowTracking(GetParentWindow());
			}
		}
	}
}