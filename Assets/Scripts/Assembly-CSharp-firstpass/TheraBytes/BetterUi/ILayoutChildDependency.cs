using UnityEngine;

namespace TheraBytes.BetterUi
{
	public interface ILayoutChildDependency
	{
		void ChildSizeChanged(Transform child);


		void ChildAddedOrEnabled(Transform child);


		void ChildRemovedOrDisabled(Transform child);
	}
}