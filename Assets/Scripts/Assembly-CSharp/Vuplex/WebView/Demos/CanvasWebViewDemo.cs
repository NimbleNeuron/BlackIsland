using UnityEngine;

namespace Vuplex.WebView.Demos
{
	
	internal class CanvasWebViewDemo : MonoBehaviour
	{
		
		private void Start()
		{
			Web.SetTouchScreenKeyboardEnabled(true);
			this._canvasWebViewPrefab = GameObject.Find("CanvasWebViewPrefab").GetComponent<CanvasWebViewPrefab>();
			this._setUpHardwareKeyboard();
		}

		
		private void _setUpHardwareKeyboard()
		{
			this._hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
			this._hardwareKeyboardListener.KeyDownReceived += delegate(object sender, KeyboardInputEventArgs eventArgs)
			{
				IWithKeyDownAndUp withKeyDownAndUp = this._canvasWebViewPrefab.WebView as IWithKeyDownAndUp;
				if (withKeyDownAndUp == null)
				{
					this._canvasWebViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
					return;
				}
				withKeyDownAndUp.KeyDown(eventArgs.Value, eventArgs.Modifiers);
			};
			this._hardwareKeyboardListener.KeyUpReceived += delegate(object sender, KeyboardInputEventArgs eventArgs)
			{
				IWithKeyDownAndUp withKeyDownAndUp = this._canvasWebViewPrefab.WebView as IWithKeyDownAndUp;
				if (withKeyDownAndUp != null)
				{
					withKeyDownAndUp.KeyUp(eventArgs.Value, eventArgs.Modifiers);
				}
			};
		}

		
		private CanvasWebViewPrefab _canvasWebViewPrefab;

		
		private HardwareKeyboardListener _hardwareKeyboardListener;
	}
}
