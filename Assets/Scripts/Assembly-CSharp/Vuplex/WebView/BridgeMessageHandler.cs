using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public class BridgeMessageHandler
	{
		
		
		
		public event EventHandler<EventArgs<StringWithIdBridgeMessage>> JavaScriptResultReceived;

		
		
		
		public event EventHandler<EventArgs<string>> MessageEmitted;

		
		
		
		public event EventHandler<EventArgs<string>> TitleChanged;

		
		
		
		public event EventHandler<UrlChangedEventArgs> UrlChanged;

		
		
		
		public event EventHandler<EventArgs<Rect>> VideoRectChanged;

		
		
		public Rect VideoRect
		{
			get
			{
				return this._videoRect;
			}
		}

		
		public void HandleMessage(string serializedMessage)
		{
			string a = serializedMessage.Contains("vuplex.webview") ? BridgeMessage.ParseType(serializedMessage) : null;
			if (!(a == "vuplex.webview.javaScriptResult"))
			{
				if (!(a == "vuplex.webview.titleChanged"))
				{
					if (!(a == "vuplex.webview.urlChanged"))
					{
						if (a == "vuplex.webview.videoRectChanged")
						{
							this._handleVideoRectChangedMessage(serializedMessage);
							return;
						}
						if (this.MessageEmitted != null)
						{
							this.MessageEmitted(this, new EventArgs<string>(serializedMessage));
						}
					}
					else
					{
						UrlAction urlAction = JsonUtility.FromJson<UrlChangedMessage>(serializedMessage).urlAction;
						if (this.UrlChanged != null)
						{
							this.UrlChanged(this, new UrlChangedEventArgs(urlAction.Url, urlAction.Title, urlAction.Type));
						}
					}
				}
				else
				{
					string val = StringBridgeMessage.ParseValue(serializedMessage);
					if (this.TitleChanged != null)
					{
						this.TitleChanged(this, new EventArgs<string>(val));
					}
				}
			}
			else
			{
				StringWithIdBridgeMessage val2 = JsonUtility.FromJson<StringWithIdBridgeMessage>(serializedMessage);
				if (this.JavaScriptResultReceived != null)
				{
					this.JavaScriptResultReceived(this, new EventArgs<StringWithIdBridgeMessage>(val2));
				}
			}
		}

		
		private void _handleVideoRectChangedMessage(string serializedMessage)
		{
			Rect rect = JsonUtility.FromJson<VideoRectChangedMessage>(serializedMessage).value.rect.toRect();
			if (this._videoRect != rect)
			{
				this._videoRect = rect;
				if (this.VideoRectChanged != null)
				{
					this.VideoRectChanged(this, new EventArgs<Rect>(rect));
				}
			}
		}

		
		private Rect _videoRect = new Rect(0f, 0f, 0f, 0f);
	}
}
