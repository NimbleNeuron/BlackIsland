using UnityEngine;

namespace Blis.Client
{
	public class UITooltipComponent : BaseUI
	{
		private Tooltip.TooltipMode mode;


		private Tooltip.Pivot pivot;


		private new RectTransform rectTransform;

		protected override void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
		}


		protected void Update()
		{
			if (mode == Tooltip.TooltipMode.Tracking)
			{
				transform.position = Input.mousePosition;
			}
		}


		private new void OnRectTransformDimensionsChange()
		{
			ChangePivot(pivot);
		}


		public void ChangeMode(Tooltip.TooltipMode mode)
		{
			this.mode = mode;
		}


		public void ChangeParent(Transform parent)
		{
			transform.SetParent(parent);
		}


		public void ChangePosition(Vector2 position)
		{
			transform.position = position;
		}


		public void ResetLocalPosition()
		{
			transform.localPosition = Vector3.zero;
		}


		public void ChangePivot(Tooltip.Pivot pivot)
		{
			if (rectTransform == null)
			{
				return;
			}

			if (mode == Tooltip.TooltipMode.Tracking)
			{
				Rect rect = rectTransform.rect;
				int num1 = (double) rect.width > (double) Screen.width - (double) Input.mousePosition.x ? 1 : 0;
				rect = rectTransform.rect;
				int num2 = (double) rect.height < (double) Input.mousePosition.y ? 1 : 0;
				pivot = (Tooltip.Pivot) ((num1 << 1) | num2);
			}

			this.pivot = pivot;
			int num3 = (int) (pivot & Tooltip.Pivot.RightBottom) >> 1;
			int num4 = (int) (pivot & Tooltip.Pivot.LeftTop);
			float x = num3;
			float y = num4;
			if (num3 > 0)
			{
				x += 0.1f;
			}
			else if (num3 < 0)
			{
				x -= 0.1f;
			}

			if (num4 > 0)
			{
				y += 0.1f;
			}
			else if (num4 < 0)
			{
				y -= 0.1f;
			}

			Vector2 vector2 = new Vector2(x, y);
			if (!(rectTransform.pivot != vector2))
			{
				return;
			}

			rectTransform.pivot = vector2;

			// co: dotPeek
			// if (this.rectTransform == null)
			// {
			// 	return;
			// }
			// int num;
			// int num2;
			// if (this.mode == Tooltip.TooltipMode.Tracking)
			// {
			// 	num = ((this.rectTransform.rect.width > (float)Screen.width - Input.mousePosition.x) ? 1 : 0);
			// 	num2 = ((this.rectTransform.rect.height < Input.mousePosition.y) ? 1 : 0);
			// 	pivot = (Tooltip.Pivot)(num << 1 | num2);
			// }
			// this.pivot = pivot;
			// num = (int)((pivot & Tooltip.Pivot.RightBottom) >> 1);
			// num2 = (int)(pivot & Tooltip.Pivot.LeftTop);
			// float num3 = (float)num;
			// float num4 = (float)num2;
			// if (num > 0)
			// {
			// 	num3 += 0.1f;
			// }
			// else if (num < 0)
			// {
			// 	num3 -= 0.1f;
			// }
			// if (num2 > 0)
			// {
			// 	num4 += 0.1f;
			// }
			// else if (num2 < 0)
			// {
			// 	num4 -= 0.1f;
			// }
			// Vector2 rhs = new Vector2(num3, num4);
			// if (this.rectTransform.pivot != rhs)
			// {
			// 	this.rectTransform.pivot = rhs;
			// }
		}


		public virtual void Clear()
		{
			ChangeMode(Tooltip.TooltipMode.Fixed);
		}
	}
}