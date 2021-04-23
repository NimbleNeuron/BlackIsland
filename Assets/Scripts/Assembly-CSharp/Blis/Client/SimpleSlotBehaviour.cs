using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public abstract class SimpleSlotBehaviour
	{
		protected SimpleSlot simpleSlot;

		private SimpleSlotBehaviour() { }


		protected SimpleSlotBehaviour(SimpleSlot simpleSlot)
		{
			this.simpleSlot = simpleSlot;
		}


		public abstract Sprite GetIcon();


		public abstract Sprite GetBackground();


		public abstract void ShowTooltip();


		public virtual void HideTooltip()
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}
	}
}