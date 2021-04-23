using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class DevLoginWindow : BasePopup
	{
		[SerializeField] private InputField inputField = default;


		[SerializeField] private Button confirmButton = default;

		public override bool IgnoreEscapeInputWindow()
		{
			return true;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			enableBackShadeEvent = false;
			if (PlayerPrefs.HasKey("_dev_id_token"))
			{
				inputField.text = PlayerPrefs.GetString("_dev_id_token");
			}
		}


		public void OnClickConfirm()
		{
			MonoBehaviourInstance<LobbyClient>.inst.doneDevLogin = true;
			string text = inputField.text;
			PlayerPrefs.SetString("_dev_id_token", text);
			Close();
		}

		private void Ref()
		{
			Reference.Use(confirmButton);
		}
	}
}