using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleEvent : MonoBehaviour
	{
		public GameObject targetObject;


		private Toggle toggle;

		private void Awake()
		{
			toggle = GetComponent<Toggle>();
		}


		private void OnEnable()
		{
			OnSwitchToggle(toggle.isOn);
			toggle.onValueChanged.AddListener(OnSwitchToggle);
		}


		private void OnDisable()
		{
			toggle.onValueChanged.RemoveListener(OnSwitchToggle);
		}


		private void OnSwitchToggle(bool isOn)
		{
			if (targetObject != null)
			{
				targetObject.SetActive(isOn);
			}
		}
	}
}