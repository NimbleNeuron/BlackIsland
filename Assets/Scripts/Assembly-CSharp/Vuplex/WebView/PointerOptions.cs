namespace Vuplex.WebView
{
	
	public class PointerOptions
	{
		
		public override string ToString()
		{
			return string.Format("Button = {0}, ClickCount = {1}", this.Button, this.ClickCount);
		}

		
		public MouseButton Button;

		
		public int ClickCount = 1;
	}
}
