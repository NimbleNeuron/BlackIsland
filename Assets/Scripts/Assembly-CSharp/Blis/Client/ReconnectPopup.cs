using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ReconnectPopup : BasePopup
	{
		[SerializeField] private Text txtDesc = default;


		[SerializeField] private Text txtReconnect = default;


		[SerializeField] private CanvasGroup indicator = default;

		public override bool IgnoreEscapeInputWindow()
		{
			return true;
		}


		public override void Open()
		{
			indicator.alpha = 0f;
			indicator.interactable = false;
			indicator.blocksRaycasts = false;
			base.Open();
		}


		public void OnClickReconnect()
		{
			ShowIndicator();
			MonoBehaviourInstance<LobbyClient>.inst.Reconnect();
		}


		private void ShowIndicator()
		{
			indicator.alpha = 1f;
			indicator.interactable = true;
			indicator.blocksRaycasts = true;
		}

		private void Ref()
		{
			Reference.Use(txtDesc);
			Reference.Use(txtReconnect);
			Reference.Use(indicator);
		}
	}
}