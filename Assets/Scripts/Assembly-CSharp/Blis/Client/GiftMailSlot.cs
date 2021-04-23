using System;
using Blis.Common;
using UnityEngine.UI;

namespace Blis.Client
{
	public class GiftMailSlot : BaseUI
	{
		private Button button;


		private Image check;


		private Text content;


		private Text expireTime;


		private Image icon;


		private long id;


		private Text title;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<Button>(gameObject, ref button);
			button.onClick.AddListener(OnClick);
			icon = GameUtil.Bind<Image>(gameObject, "Icon");
			check = GameUtil.Bind<Image>(gameObject, "Check");
			title = GameUtil.Bind<Text>(gameObject, "Title");
			content = GameUtil.Bind<Text>(gameObject, "Content");
			expireTime = GameUtil.Bind<Text>(gameObject, "Date");
			check.enabled = false;
		}


		public void SetSlot(LobbyApi.UserGiftMail giftMail)
		{
			if (giftMail == null)
			{
				return;
			}

			id = giftMail.id;
			button.interactable = giftMail.type != LobbyApi.GiftMailType.NOTICE;
			icon.sprite = giftMail.type != LobbyApi.GiftMailType.NOTICE
				? SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Giftmail_Basic")
				: SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Symbol");
			title.text = giftMail.title;
			content.text = giftMail.message;
			expireTime.text = LnUtil.GetGiftMailExpireTimeText(DateTime.Now.ToUniversalTime(), giftMail.expireDtm);
		}


		private void OnClick()
		{
			NoticeService.RequestGift(id);
		}
	}
}