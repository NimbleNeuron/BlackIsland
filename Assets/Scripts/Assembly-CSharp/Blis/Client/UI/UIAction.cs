using System;

namespace Blis.Client.UI
{
	public abstract class UIAction
	{
		public void IfTypeIs<T>(Action<T> callback) where T : UIAction
		{
			if (this is T)
			{
				callback((T) this);
			}
		}
	}
}