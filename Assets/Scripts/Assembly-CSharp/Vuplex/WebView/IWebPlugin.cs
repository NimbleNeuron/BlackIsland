using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	internal interface IWebPlugin
	{
		
		void ClearAllData();

		
		void CreateTexture(float width, float height, Action<Texture2D> callback);

		
		void CreateMaterial(Action<Material> callback);

		
		void CreateVideoMaterial(Action<Material> callback);

		
		IWebView CreateWebView();

		
		void SetIgnoreCertificateErrors(bool ignore);

		
		void SetStorageEnabled(bool enabled);

		
		void SetUserAgent(bool mobile);

		
		void SetUserAgent(string userAgent);
	}
}
