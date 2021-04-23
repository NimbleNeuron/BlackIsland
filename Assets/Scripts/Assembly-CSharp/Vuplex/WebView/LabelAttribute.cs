using UnityEngine;

namespace Vuplex.WebView
{
	
	public class LabelAttribute : PropertyAttribute
	{
		
		
		
		public string Label { get; private set; }

		
		public LabelAttribute(string label)
		{
			this.Label = label;
		}
	}
}
