using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LnText : Text, ILnEventHander
	{
		private const char PREFIX = '#';


		private string initialText;

		protected override void Awake()
		{
			base.Awake();
			if (Application.isPlaying)
			{
				initialText = text ?? string.Empty;
				ApplyLocalization();
			}
		}


		protected override void OnEnable()
		{
			base.OnEnable();
			ApplyLocalization();
		}


		public void OnLnDataChange()
		{
			ApplyLocalization();
		}


		private void ApplyLocalization()
		{
			if (Application.isPlaying && Ln.IsLoaded)
			{
				if (!string.IsNullOrEmpty(initialText) && initialText.StartsWith("#"))
				{
					text = Ln.Get(initialText.Substring(1));
				}

				RefreshFont();
			}
		}


		private void RefreshFont()
		{
			string fontName = Ln.GetCurrentLanguage().GetFontName();
			if (font != null && !string.Equals(font.name, fontName))
			{
				font = SingletonMonoBehaviour<ResourceManager>.inst.GetFont(fontName);
			}
		}
	}
}