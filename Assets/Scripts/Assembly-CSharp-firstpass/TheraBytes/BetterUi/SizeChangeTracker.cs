using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Helpers/Size Change Tracker", 30)]
	public class SizeChangeTracker : UIBehaviour, ILayoutSelfController, ILayoutController
	{
		public RectTransform[] AffectedObjects;


		private bool isInRecursion;


		protected override void OnEnable()
		{
			base.OnEnable();
			CallForAffectedObjects(delegate(ILayoutChildDependency dp) { dp.ChildAddedOrEnabled(transform); });
		}


		protected override void OnDisable()
		{
			base.OnDisable();
			CallForAffectedObjects(delegate(ILayoutChildDependency dp) { dp.ChildRemovedOrDisabled(transform); });
		}


		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			CallForAffectedObjects(delegate(ILayoutChildDependency dp) { dp.ChildSizeChanged(transform); });
		}


		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			CallForAffectedObjects(delegate(ILayoutChildDependency dp) { dp.ChildRemovedOrDisabled(transform); });
		}


		public void SetLayoutHorizontal()
		{
			CallForAffectedObjects(delegate(ILayoutChildDependency dp) { dp.ChildSizeChanged(transform); });
		}


		public void SetLayoutVertical()
		{
			CallForAffectedObjects(delegate(ILayoutChildDependency dp) { dp.ChildSizeChanged(transform); });
		}


		private void CallForAffectedObjects(Action<ILayoutChildDependency> function)
		{
			if (isInRecursion)
			{
				return;
			}

			if (function == null)
			{
				throw new ArgumentNullException("function must not be null");
			}

			isInRecursion = true;
			try
			{
				foreach (RectTransform rectTransform in AffectedObjects)
				{
					if (!(rectTransform == null))
					{
						foreach (ILayoutChildDependency layoutChildDependency in from o in rectTransform
								.GetComponents<MonoBehaviour>()
							where o is ILayoutChildDependency
							select o as ILayoutChildDependency)
						{
							if (layoutChildDependency != null)
							{
								function(layoutChildDependency);
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				isInRecursion = false;
			}
		}
	}
}