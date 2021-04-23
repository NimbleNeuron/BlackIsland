using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	[ExecuteInEditMode]
	public class MobileControlRig : MonoBehaviour
	{
		private void Start()
		{
			if (FindObjectOfType<EventSystem>() == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
			}
		}


		private void OnEnable()
		{
			CheckEnableControlRig();
		}


		private void CheckEnableControlRig()
		{
			EnableControlRig(false);
		}


		private void EnableControlRig(bool enabled)
		{
			foreach (object obj in transform)
			{
				((Transform) obj).gameObject.SetActive(enabled);
			}
		}
	}
}