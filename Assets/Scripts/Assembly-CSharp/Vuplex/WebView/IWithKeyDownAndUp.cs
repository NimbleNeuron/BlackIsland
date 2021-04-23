namespace Vuplex.WebView
{
	
	public interface IWithKeyDownAndUp
	{
		
		void KeyDown(string key, KeyModifier modifiers);

		
		void KeyUp(string key, KeyModifier modifiers);
	}
}
