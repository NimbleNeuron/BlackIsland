using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class JoinCustomGameRoomWindow : BaseWindow
	{
		private const int enableLength = 6;


		private Button btnOk;


		private Image imgBtnOkBG;


		private InputField inputField;


		private GameObject txtOkDisable;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			inputField = GameUtil.Bind<InputField>(gameObject, "InputField");
			inputField.onValueChanged.AddListener(OnValueChanged);
			btnOk = GameUtil.Bind<Button>(gameObject, "BTN_Ok");
			txtOkDisable = transform.FindRecursively("TXT_Ok_Disable").gameObject;
			imgBtnOkBG = GameUtil.Bind<Image>(btnOk.gameObject, ref imgBtnOkBG);
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			inputField.text = string.Empty;
			btnOk.enabled = false;
			txtOkDisable.SetActive(true);
			CanvasGroup.interactable = true;
			inputField.ActivateInputField();
		}


		private void OnValueChanged(string str)
		{
			if (str.Length < 6)
			{
				imgBtnOkBG.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Btn_Basic");
				btnOk.enabled = false;
				btnOk.enabled = false;
				txtOkDisable.SetActive(true);
				return;
			}

			imgBtnOkBG.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Btn_Hero");
			btnOk.enabled = true;
			txtOkDisable.SetActive(false);
		}


		public void ClickOK()
		{
			if (inputField.text == string.Empty)
			{
				return;
			}

			MonoBehaviourInstance<MatchingService>.inst.EnterCustomGame(inputField.text);
			Close();
		}


		public void ClickClose()
		{
			Close();
		}
	}
}