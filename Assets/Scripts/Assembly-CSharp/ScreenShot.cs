using UnityEngine;


public class ScreenShot : MonoBehaviour
{
	
	private void Update()
	{
		if (Input.GetKeyDown(this.ActivateKey) && this.ScreenShotActive)
		{
			Debug.Log("Screen Shot Captured.");
			ScreenCapture.CaptureScreenshot(this.CaptureName);
		}
	}

	
	[SerializeField]
	private bool ScreenShotActive = default;

	
	[SerializeField]
	private KeyCode ActivateKey = KeyCode.Space;

	
	[SerializeField]
	private string CaptureName = "ScreenShot.png";
}
