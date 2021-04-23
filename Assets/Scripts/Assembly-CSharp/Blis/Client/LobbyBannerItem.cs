using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyBannerItem : BaseUI, IPointerClickHandler, IEventSystemHandler
	{
		private RawImage bannerImage;


		private Texture defaultImage;


		private Text desc;


		private string linkedUrl;


		private Text title;


		public void OnPointerClick(PointerEventData eventData)
		{
			if (!string.IsNullOrEmpty(linkedUrl))
			{
				Application.OpenURL(linkedUrl);
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			bannerImage = GameUtil.Bind<RawImage>(gameObject, "Banner");
			title = GameUtil.Bind<Text>(gameObject, "Title");
			desc = GameUtil.Bind<Text>(gameObject, "Desc");
			defaultImage = bannerImage.texture;
		}


		public void Init(string title, string desc, Texture2D bannerImage, string linkedUrl)
		{
			this.title.text = title;
			this.desc.text = desc;
			this.bannerImage.texture = bannerImage != null ? bannerImage : defaultImage;
			this.linkedUrl = linkedUrl;
		}
	}
}