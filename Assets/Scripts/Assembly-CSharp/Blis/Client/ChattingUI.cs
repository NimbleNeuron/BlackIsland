using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Blis.Common;
using Blis.Common.Utils;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ChattingUI : BaseUI
	{
		[SerializeField] protected InputFieldExtension chatInputField;


		[SerializeField] protected CanvasGroup inputGroup;


		[SerializeField] protected CanvasGroup scrollGroup;


		[SerializeField] protected CanvasGroup scrollBarGroup;


		[SerializeField] protected LoopListView2 listView;


		[SerializeField] protected Image scrollImage;


		protected readonly Color BgClearColor = new Color(0f, 0f, 0f, 0f);


		protected readonly Color BgColor = new Color(0f, 0f, 0f, 0.5f);


		protected readonly float maxHeight = 612f;


		protected readonly float minHeight = 85f;


		protected readonly float normalHeight = 230f;


		private WaitForSeconds afterInactiveWait = new WaitForSeconds(10f);


		private WaitForSeconds beforeInactiveWait = new WaitForSeconds(10f);


		protected WaitForSeconds inactiveWait;


		protected List<ChattingInfo> infos = new List<ChattingInfo>();


		private bool initializedListView;


		protected bool isFocus;


		private bool isLobbyChat;


		protected Func<bool> isLockInput;


		private float LastTime;


		protected Vector2 maxDeltaSize;


		protected Vector2 minDeltaSize;


		private bool needToUpdateChat;


		protected Vector2 normalDeltaSize;


		protected Action<string> sendChatAction;


		private bool useWaitInactive = true;


		protected Coroutine waitInactiveCoroutine;


		private LoopListView2 ListView {
			get
			{
				if (!initializedListView)
				{
					InitListView();
				}

				return listView;
			}
		}


		public bool IsFocus => isFocus;


		private void Update()
		{
			if (IsFocus != chatInputField.isFocused && chatInputField.isFocused)
			{
				Active(true);
			}

			if (isLockInput != null && !isLockInput())
			{
				if (Input.GetKeyDown(KeyCode.Return))
				{
					return;
				}

				if (Input.GetKeyDown(KeyCode.Escape))
				{
					return;
				}

				if (Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					return;
				}
			}

			isFocus = chatInputField.isFocused;
		}


		private void LateUpdate()
		{
			if (needToUpdateChat)
			{
				needToUpdateChat = false;
				if (IsFocus)
				{
					if (string.IsNullOrEmpty(GetChatString()))
					{
						InactveUi(false);
						return;
					}

					SendEdit();
				}
				else
				{
					Active(true);
				}
			}
		}


		public void SetSendEvent(Action<string> action)
		{
			sendChatAction = action;
		}


		public void SetIsLockInput(Func<bool> func)
		{
			isLockInput = func;
		}


		public void SetWaitInactive(bool waitInactive)
		{
			useWaitInactive = waitInactive;
		}


		public void SetLobbyChat(bool lobbyChat)
		{
			isLobbyChat = lobbyChat;
		}


		public void ClearInput()
		{
			chatInputField.text = string.Empty;
		}


		public void EnterChat(bool isAllChat)
		{
			if (isAllChat)
			{
				AllChattingSet();
			}

			needToUpdateChat = true;
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			minDeltaSize = new Vector2(rectTransform.rect.width, minHeight);
			normalDeltaSize = new Vector2(rectTransform.rect.width, normalHeight);
			maxDeltaSize = new Vector2(rectTransform.rect.width, maxHeight);
			chatInputField.onEndEdit.AddListener(delegate { OnEndEdit(); });
			if (isLobbyChat)
			{
				SetMinChatSize();
			}

			InitListView();
			InactveUi(true);
		}


		private void InitListView()
		{
			if (initializedListView)
			{
				return;
			}

			initializedListView = true;
			listView.InitListView(NoticeService.GetGiftMailCount(), OnGetItemByIndex);
		}


		public void UpdateChatting(List<ChattingInfo> lobbyChat)
		{
			infos = lobbyChat;
			ListView.SetListItemCount(infos.Count, false);
			Active(false);
			WaitInactive(false);
		}


		public void AddChatting(int senderObjectId, string characterName, string content, bool isAll, bool isNotice,
			bool showTime, bool inputReady = false, string chattingSoundName = "")
		{
			string playerNickName = MonoBehaviourInstance<ClientService>.inst.GetPlayerNickName(senderObjectId);
			AddChatting(playerNickName, characterName, content, isAll, isNotice, showTime, inputReady,
				chattingSoundName);
		}


		public void AddChatting(string nickName, string characterName, string content, bool isAll, bool isNotice,
			bool showTime, bool inputReady = false, string chattingSoundName = "")
		{
			content = Regex.Replace(content, "<.*?>", string.Empty);
			if (showTime)
			{
				int playTime = MonoBehaviourInstance<GameUI>.inst.BattleInfoHud.GetPlayTime();
				int value = playTime / 60;
				int value2 = playTime % 60;
				StringBuilder stringBuilder = GameUtil.StringBuilder;
				stringBuilder.Clear();
				stringBuilder.Append("[");
				stringBuilder.Append(GameUtil.IntToString(value, GameUtil.NumberOfDigits.Two));
				stringBuilder.Append(" : ");
				stringBuilder.Append(GameUtil.IntToString(value2, GameUtil.NumberOfDigits.Two));
				stringBuilder.Append("] ");
				stringBuilder.Append(nickName);
				nickName = stringBuilder.ToString();
			}

			ChattingInfo item = new ChattingInfo
			{
				NickName = nickName,
				CharacterName = characterName,
				Content = content,
				IsAll = isAll,
				IsNotice = isNotice
			};
			infos.Insert(0, item);
			ListView.SetListItemCount(infos.Count, false);
			Active(inputReady);
			WaitInactive(false);
			if (!string.IsNullOrEmpty(chattingSoundName))
			{
				Singleton<SoundControl>.inst.Play2DSound(chattingSoundName);
			}
		}


		public void AddSystemChatting(string content, bool showTime = false, bool noticeColor = true)
		{
			content = Regex.Replace(content, "<.*?>", string.Empty);
			if (showTime)
			{
				int playTime = MonoBehaviourInstance<GameUI>.inst.BattleInfoHud.GetPlayTime();
				int value = playTime / 60;
				int value2 = playTime % 60;
				StringBuilder stringBuilder = GameUtil.StringBuilder;
				stringBuilder.Clear();
				stringBuilder.Append("[");
				stringBuilder.Append(GameUtil.IntToString(value, GameUtil.NumberOfDigits.Two));
				stringBuilder.Append(" : ");
				stringBuilder.Append(GameUtil.IntToString(value2, GameUtil.NumberOfDigits.Two));
				stringBuilder.Append("] ");
				stringBuilder.Append(content);
				content = stringBuilder.ToString();
			}

			ChattingInfo item = new ChattingInfo
			{
				NickName = null,
				CharacterName = null,
				Content = content,
				IsAll = false,
				IsNotice = true,
				noticeColor = noticeColor
			};
			infos.Insert(0, item);
			ListView.SetListItemCount(infos.Count, false);
			Active(false);
			WaitInactive(false);
		}


		public void AddStrategySystemChatting(string content)
		{
			AddSystemChatting(content, true, false);
		}


		public new bool IsActive()
		{
			return inputGroup.alpha > 0f;
		}


		public string GetChatString()
		{
			return chatInputField.text;
		}


		public void Active(bool inputReady)
		{
			scrollGroup.alpha = 1f;
			gameObject.SetActive(true);
			ListView.MovePanelToItemIndex(0, 0f);
			if (inputReady)
			{
				if (isLobbyChat)
				{
					SetNormalChatSize();
				}

				scrollBarGroup.alpha = 1f;
				scrollBarGroup.blocksRaycasts = true;
				inputGroup.alpha = 1f;
				inputGroup.blocksRaycasts = true;
				chatInputField.gameObject.SetActive(true);
				chatInputField.ActivateInputField();
				chatInputField.caretColor = Color.white;
				chatInputField.MoveTextEnd(true);
				GameInput inst = MonoBehaviourInstance<GameInput>.inst;
				if (inst != null)
				{
					inst.SetLockInputChat(true);
				}

				isFocus = true;
				ActiveScrollBg(true);
			}

			WaitInactive(false);
		}


		private void ActiveScrollBg(bool active)
		{
			scrollImage.color = active ? BgColor : BgClearColor;
		}


		private void AllChattingSet()
		{
			chatInputField.text = "/all ";
			chatInputField.MoveTextEnd(true);
		}


		public void OnEndEdit()
		{
			GameInput inst = MonoBehaviourInstance<GameInput>.inst;
			if (inst == null)
			{
				return;
			}

			inst.SetLockInputChat(false);
		}


		private void SendEdit()
		{
			GameInput inst = MonoBehaviourInstance<GameInput>.inst;
			if (inst != null)
			{
				inst.SetLockInputChat(false);
			}

			string chatString = GetChatString();
			if (string.IsNullOrEmpty(chatString))
			{
				return;
			}

			Action<string> action = sendChatAction;
			if (action != null)
			{
				action(chatString);
			}

			chatInputField.text = string.Empty;
			InactveUi(false);
		}


		public void SetMinChatSize()
		{
			rectTransform.sizeDelta = minDeltaSize;
			ListView.ResetListView();
		}


		public void SetMaxChatSize()
		{
			rectTransform.sizeDelta = maxDeltaSize;
			ListView.ResetListView();
		}


		public void SetNormalChatSize()
		{
			rectTransform.sizeDelta = normalDeltaSize;
			ListView.ResetListView();
		}


		public void SetWaitInactiveSeconds(float beforeSeconds, float afterSeconds)
		{
			beforeInactiveWait = new WaitForSeconds(beforeSeconds);
			afterInactiveWait = new WaitForSeconds(afterSeconds);
		}


		private void WaitInactive(bool completeInactive)
		{
			if (!useWaitInactive)
			{
				return;
			}

			if (waitInactiveCoroutine != null)
			{
				StopCoroutine(waitInactiveCoroutine);
			}

			inactiveWait = completeInactive ? afterInactiveWait : beforeInactiveWait;
			waitInactiveCoroutine = this.StartThrowingCoroutine(WaitInactiveProc(completeInactive),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][WaitInactiveProc] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator WaitInactiveProc(bool completeInactive)
		{
			yield return inactiveWait;
			while (isFocus)
			{
				yield return inactiveWait;
			}

			InactveUi(completeInactive);
		}


		private void InactveUi(bool completeInactive)
		{
			if (!useWaitInactive)
			{
				return;
			}

			if (waitInactiveCoroutine != null)
			{
				StopCoroutine(waitInactiveCoroutine);
			}

			if (!isLobbyChat)
			{
				ActiveScrollBg(false);
				inputGroup.alpha = 0f;
				inputGroup.blocksRaycasts = false;
				scrollBarGroup.alpha = 0f;
				scrollBarGroup.blocksRaycasts = false;
			}

			DeactivateInput();
			chatInputField.text = string.Empty;
			if (!completeInactive)
			{
				WaitInactive(true);
				return;
			}

			if (isLobbyChat)
			{
				SetMinChatSize();
				return;
			}

			scrollGroup.alpha = 0f;
		}


		public void DeactivateInput()
		{
			chatInputField.DeactivateInputField();
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (index > infos.Count - 1)
			{
				return null;
			}

			ChattingInfo info = infos[index];
			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("ChatSlot");
			UIChattingSlot component = loopListViewItem.GetComponent<UIChattingSlot>();
			component.Set(info);
			component.ContentSizeFilter.SetLayoutVertical();
			loopListViewItem.gameObject.SetActive(true);
			return loopListViewItem;
		}


		public void EraseChatting()
		{
			infos.Clear();
			ListView.SetListItemCount(infos.Count, false);
		}


		public class ChattingInfo
		{
			public string CharacterName;


			public string Content;


			public bool IsAll;


			public bool IsNotice;

			public string NickName;


			public bool noticeColor;
		}
	}
}