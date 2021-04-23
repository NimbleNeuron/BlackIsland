using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class AccessTermsUI : MonoBehaviour
	{
		[SerializeField] private Toggle AccessTermsToggle01 = default;


		[SerializeField] private Toggle AccessTermsToggle02 = default;


		[SerializeField] private Button TermsButton = default;


		[SerializeField] private Image Dimmed = default;


		private string accessKey;


		private bool isAccessTermsAgreement;

		public void Awake()
		{
			TermsButton.onClick.AddListener(delegate
			{
				PlayerPrefs.SetInt(accessKey, 2);
				isAccessTermsAgreement = true;
			});
		}


		public void CloseAccessTermsUI()
		{
			gameObject.SetActive(false);
		}


		public void ShowAccessTermsUI(string accessKey)
		{
			this.accessKey = accessKey;
			gameObject.SetActive(true);
			TermsButton.enabled = false;
			isAccessTermsAgreement = false;
		}


		public void ChangedToggle(bool IsCheck)
		{
			bool flag = AccessTermsToggle01.isOn && AccessTermsToggle02.isOn;
			TermsButton.enabled = flag;
			Dimmed.gameObject.SetActive(!flag);
		}


		public void OnClick_Cancle()
		{
			Application.Quit();
		}


		public bool IsAccessTermsAgreement()
		{
			return isAccessTermsAgreement;
		}
	}
}