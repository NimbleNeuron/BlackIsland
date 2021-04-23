using Blis.Client;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Common
{
	public class UIChattingSlot : BaseUI
	{
		[SerializeField] private Text content = default;


		private ContentSizeFitter contentSizeFillter;


		public ContentSizeFitter ContentSizeFilter {
			get
			{
				if (contentSizeFillter == null)
				{
					contentSizeFillter = GetComponent<ContentSizeFitter>();
				}

				return contentSizeFillter;
			}
		}


		public void Set(ChattingUI.ChattingInfo info)
		{
			Set(info.NickName, info.CharacterName, info.Content, info.IsAll, info.IsNotice, info.noticeColor);
		}


		public void Set(string nickName, string characterName, string chatContent, bool isAll, bool isNotice,
			bool isNoticeColor)
		{
			if (string.IsNullOrEmpty(nickName))
			{
				if (isNotice)
				{
					if (isNoticeColor)
					{
						chatContent = "<color=#FFA500>" + chatContent + "</color>";
					}
					else
					{
						chatContent = "<color=#00ffff>" + chatContent + "</color>";
					}
				}
			}
			else if (string.IsNullOrEmpty(characterName))
			{
				if (isAll)
				{
					chatContent = string.Concat("<color=#00ffff>[", Ln.Get("전체"), "] ", nickName, " : </color>",
						chatContent);
				}
				else
				{
					chatContent = "<color=#00ffff>" + nickName + " : </color>" + chatContent;
				}
			}
			else if (isAll)
			{
				chatContent = string.Concat("<color=#00ffff>[", Ln.Get("전체"), "] ", nickName, "(", characterName,
					") : </color>", chatContent);
			}
			else
			{
				chatContent = string.Concat("<color=#00ffff>", nickName, "(", characterName, ") : </color>",
					chatContent);
			}

			content.text = chatContent;
		}
	}
}