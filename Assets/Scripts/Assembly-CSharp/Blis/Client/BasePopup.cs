using System;

namespace Blis.Client
{
	public class BasePopup : BaseWindow
	{
		private Action backShadeCallback;


		protected bool enableBackShadeEvent;


		private bool isHover;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			backShade.OnPointerClickEvent += delegate
			{
				if (enableBackShadeEvent)
				{
					Action action = backShadeCallback;
					if (action != null)
					{
						action();
					}

					Close();
				}
			};
		}


		public void SetBackEvent(Action action)
		{
			backShadeCallback = action;
		}


		protected override void OnClose()
		{
			base.OnClose();
			backShadeCallback = null;
		}
	}
}