namespace Vuplex.WebView.Demos
{
	
	public class KeyboardInputEventArgs : EventArgs<string>
	{
		
		public KeyboardInputEventArgs(string value, KeyModifier modifiers) : base(value)
		{
			this.Modifiers = modifiers;
		}

		
		public readonly KeyModifier Modifiers;
	}
}
