using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public abstract class LobbyTabBaseUI : BaseUI
	{
		private Canvas canvas;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<Canvas>(gameObject, ref canvas);
			EnableCanvas(false);
		}


		protected void EnableCanvas(bool enable)
		{
			if (canvas == null)
			{
				GameUtil.Bind<Canvas>(gameObject, ref canvas);
			}

			if (canvas != null)
			{
				canvas.enabled = enable;
			}
		}
	}
}