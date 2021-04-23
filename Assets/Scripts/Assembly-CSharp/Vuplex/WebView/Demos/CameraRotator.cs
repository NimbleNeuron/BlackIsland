using UnityEngine;
using UnityEngine.XR;

namespace Vuplex.WebView.Demos
{
	
	internal class CameraRotator : MonoBehaviour
	{
		
		private void Start()
		{
			if (!XRSettings.enabled)
			{
				Input.gyro.enabled = true;
			}
			if (Application.isEditor && this.InstructionMessage != null)
			{
				this.InstructionMessage.SetActive(true);
				return;
			}
			this.InstructionMessage = null;
		}

		
		private void Update()
		{
			if (this.InstructionMessage != null && Input.GetMouseButtonDown(0))
			{
				this.InstructionMessage.SetActive(false);
				this.InstructionMessage = null;
			}
			if (XRSettings.enabled)
			{
				return;
			}
			if (SystemInfo.supportsGyroscope)
			{
				Camera.main.transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, Input.gyro.rotationRateUnbiased.z);
				return;
			}
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				float num = 10f;
				float num2 = 80f;
				this._rotationFromMouse.x = this._rotationFromMouse.x + Input.GetAxis("Mouse X") * num;
				this._rotationFromMouse.y = this._rotationFromMouse.y - Input.GetAxis("Mouse Y") * num;
				this._rotationFromMouse.x = Mathf.Repeat(this._rotationFromMouse.x, 360f);
				this._rotationFromMouse.y = Mathf.Clamp(this._rotationFromMouse.y, -num2, num2);
				Camera.main.transform.rotation = Quaternion.Euler(this._rotationFromMouse.y, this._rotationFromMouse.x, 0f);
			}
		}

		
		public GameObject InstructionMessage;

		
		private Vector2 _rotationFromMouse;
	}
}
