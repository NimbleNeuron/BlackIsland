using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	internal class StandaloneWebPlugin : MonoBehaviour
	{
		
		public void ClearAllData()
		{
			StandaloneWebView.ClearAllData();
		}

		
		public void CreateTexture(float width, float height, Action<Texture2D> callback)
		{
			BaseWebView.CreateTexture(width, height, callback);
		}

		
		public void CreateMaterial(Action<Material> callback)
		{
			this.CreateTexture(1f, 1f, delegate(Texture2D texture)
			{
				Material material = Utils.CreateDefaultMaterial();
				material.mainTexture = texture;
				callback(material);
			});
		}

		
		public void CreateVideoMaterial(Action<Material> callback)
		{
			callback(null);
		}

		
		public void SetIgnoreCertificateErrors(bool ignore)
		{
			StandaloneWebView.SetIgnoreCertificateErrors(ignore);
		}

		
		public void SetStorageEnabled(bool enabled)
		{
			StandaloneWebView.SetStorageEnabled(enabled);
		}

		
		public void SetUserAgent(bool mobile)
		{
			BaseWebView.GloballySetUserAgent(mobile);
		}

		
		public void SetUserAgent(string userAgent)
		{
			BaseWebView.GloballySetUserAgent(userAgent);
		}

		
		private void Start()
		{
			Application.quitting += delegate()
			{
				StandaloneWebView.TerminatePlugin();
			};
		}
	}
}
