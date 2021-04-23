using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[ExecuteInEditMode]
	public abstract class ResolutionSizer<T> : UIBehaviour, ILayoutController, ILayoutSelfController,
		IResolutionDependency
	{
		protected abstract ScreenDependentSize<T> sizer { get; }


		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateSize();
		}


		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			UpdateSize();
		}


		public virtual void SetLayoutHorizontal()
		{
			UpdateSize();
		}


		public virtual void SetLayoutVertical()
		{
			UpdateSize();
		}


		public void OnResolutionChanged()
		{
			UpdateSize();
		}


		protected void SetDirty()
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			UpdateSize();
		}


		private void UpdateSize()
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			T newSize = sizer.CalculateSize(this);
			ApplySize(newSize);
		}


		protected abstract void ApplySize(T newSize);
	}
}