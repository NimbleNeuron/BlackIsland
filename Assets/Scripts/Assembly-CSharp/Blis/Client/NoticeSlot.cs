using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class NoticeSlot : BaseUI, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		private readonly Color defaultColor = new Color(0.78f, 0.78f, 0.78f);


		private readonly Color hoverColor = new Color(1f, 0.7f, 0.3f);


		private Button button;


		private Text content;


		private Image icon;


		private long id;


		private Image link;


		private Text startTime;


		private Text title;


		private string url;


		public void OnPointerEnter(PointerEventData eventData)
		{
			button.interactable = true;
			link.color = hoverColor;
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			button.interactable = false;
			link.color = defaultColor;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<Button>(gameObject, ref button);
			button.onClick.AddListener(OnClick);
			icon = GameUtil.Bind<Image>(gameObject, "Icon");
			link = GameUtil.Bind<Image>(gameObject, "OutLink");
			title = GameUtil.Bind<Text>(gameObject, "Title");
			content = GameUtil.Bind<Text>(gameObject, "Content");
			startTime = GameUtil.Bind<Text>(gameObject, "Date");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
		}


		public void SetSlot(LobbyApi.LobbyNotice lobbyNotice)
		{
			if (lobbyNotice == null)
			{
				return;
			}

			id = lobbyNotice.id;
			url = lobbyNotice.targetUrl;
			icon.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Symbol");
			link.enabled = !string.IsNullOrEmpty(lobbyNotice.targetUrl);
			title.text = lobbyNotice.title;
			content.text = lobbyNotice.message;
			startTime.text = LnUtil.GetLobbyNoticeSlotTimeText(lobbyNotice.startDtm);
		}


		private void OnClick()
		{
			if (string.IsNullOrEmpty(url))
			{
				return;
			}

			Application.OpenURL(url);
		}
	}
}