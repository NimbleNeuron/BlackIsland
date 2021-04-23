using UnityEngine;

namespace BIOutline
{
	public class SelectionRenderer : Outliner
	{
		private bool hover;


		private bool selection;


		private bool untouched;


		public void SetSelection(bool selection)
		{
			this.selection = selection;
			UpdateSelection();
			OrganizeDrawCall();
		}


		public void SetHover(bool hover)
		{
			this.hover = hover;
			UpdateSelection();
			OrganizeDrawCall();
		}


		public void SetUntouched(bool untouched)
		{
			this.untouched = untouched;
			UpdateSelection();
			OrganizeDrawCall();
		}


		public void SetColor(Color color)
		{
			this.color = color;
		}


		private void UpdateSelection()
		{
			if (hover)
			{
				outlineType = OutlineType.HoverLine;
				return;
			}

			if (selection)
			{
				outlineType = OutlineType.SelectLine;
				return;
			}

			if (untouched)
			{
				outlineType = OutlineType.InSight;
				return;
			}

			outlineType = OutlineType.None;
		}
	}
}