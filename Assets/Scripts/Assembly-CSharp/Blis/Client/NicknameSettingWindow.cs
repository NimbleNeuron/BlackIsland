using System;
using System.Collections.Generic;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class NicknameSettingWindow : BasePopup
	{
		[SerializeField] private InputFieldExtension inputField = default;


		[SerializeField] private Button confirmButton = default;


		private Action<string> onNicknameConfirm;


		private LnText recommendText;


		private LnText warningText;

		public override bool IgnoreEscapeInputWindow()
		{
			return true;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			enableBackShadeEvent = false;
			inputField.onValueChanged.AddListener(OnInputValueChange);
			warningText = GameUtil.Bind<LnText>(gameObject, "Warning");
			recommendText = GameUtil.Bind<LnText>(gameObject, "Recommend");
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			confirmButton.interactable = false;
			inputField.text = "";
			warningText.transform.localScale = Vector3.zero;
			recommendText.transform.localScale = Vector3.zero;
		}


		protected override void OnClose()
		{
			base.OnClose();
			onNicknameConfirm = null;
		}


		private void OnInputValueChange(string value)
		{
			confirmButton.interactable = ValidateNicknameLength(value);
		}


		private bool ValidateNicknameLength(string nickname)
		{
			return ArchStringUtil.IsOverSizeANSI(nickname, 2) && !ArchStringUtil.IsOverSizeANSI(nickname, 16);
		}


		public void OnUnavailableNickname(string nickname)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < 3; i++)
			{
				string recommend = nickname + Random.Range(0, 99);
				recommend = ArchStringUtil.CutOverSizeANSI(recommend, 16);
				if (!list.Exists(r => r.Equals(recommend)))
				{
					list.Add(recommend);
				}
			}

			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			int count = list.Count;
			for (int j = 0; j < count; j++)
			{
				stringBuilder.Append(list[j]);
				if (j + 1 < count)
				{
					stringBuilder.Append(" / ");
				}
			}

			if (list.Count > 0)
			{
				warningText.transform.localScale = Vector3.one;
				recommendText.transform.localScale = Vector3.one;
				recommendText.text = stringBuilder.ToString();
			}
		}


		public void OnClickConfirmNickname()
		{
			if (!ValidateNicknameLength(inputField.text))
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임 길이 에러"));
			}
			else if (!StringUtil.IsVaildStr(inputField.text))
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임에 특수 문자 사용 불가"));
			}
			else if (SingletonMonoBehaviour<SwearWordManager>.inst.IsSwearWordNickName(inputField.text))
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임 사용 불가 단어"));
			}
			else if (GameDB.bot.IsBotName(inputField.text))
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get(string.Format("{0}", ErrorType.UnavailableNickname)));
			}
			else
			{
				string nickname = inputField.text;
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("{0}(으)로 하시겠습니까?", nickname), new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("확인"),
					callback = (Action) (() => MonoBehaviourInstance<LobbyService>.inst.ChangeNickname(nickname,
						(restErrorType, message, res) =>
						{
							if (restErrorType != RestErrorType.SUCCESS)
							{
								OnUnavailableNickname(nickname);
							}
							else
							{
								SingletonMonoBehaviour<GameAnalytics>.inst.CustomEvent("Nickname Create",
									new Dictionary<string, object>
									{
										{
											"server",
											"release"
										}
									});
								Close();
							}
						}))
				}, new Popup.Button
				{
					type = Popup.ButtonType.Cancel,
					text = Ln.Get("취소")
				});
			}

			// co: dotPeek
			// if (!this.ValidateNicknameLength(this.inputField.text))
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임 길이 에러"), null);
			// 	return;
			// }
			// if (!StringUtil.IsVaildStr(this.inputField.text))
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임에 특수 문자 사용 불가"), null);
			// 	return;
			// }
			// if (SingletonMonoBehaviour<SwearWordManager>.inst.IsSwearWordNickName(this.inputField.text))
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임 사용 불가 단어"), null);
			// 	return;
			// }
			// if (GameDB.bot.IsBotName(this.inputField.text))
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Error(Ln.Get(string.Format("{0}", ErrorType.UnavailableNickname)), null);
			// 	return;
			// }
			// string nickname = this.inputField.text;
			// Action<RestErrorType, string, string> <>9__1;
			// MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("{0}(으)로 하시겠습니까?", nickname), new Popup.Button[]
			// {
			// 	new Popup.Button
			// 	{
			// 		type = Popup.ButtonType.Confirm,
			// 		text = Ln.Get("확인"),
			// 		callback = delegate()
			// 		{
			// 			LobbyService inst = MonoBehaviourInstance<LobbyService>.inst;
			// 			string nickname = nickname;
			// 			Action<RestErrorType, string, string> callback;
			// 			if ((callback = <>9__1) == null)
			// 			{
			// 				callback = (<>9__1 = delegate(RestErrorType restErrorType, string message, string res)
			// 				{
			// 					if (restErrorType != RestErrorType.SUCCESS)
			// 					{
			// 						this.OnUnavailableNickname(nickname);
			// 						return;
			// 					}
			// 					SingletonMonoBehaviour<GameAnalytics>.inst.CustomEvent("Nickname Create", new Dictionary<string, object>
			// 					{
			// 						{
			// 							"server",
			// 							"release"
			// 						}
			// 					});
			// 					this.Close();
			// 				});
			// 			}
			// 			inst.ChangeNickname(nickname, callback);
			// 		}
			// 	},
			// 	new Popup.Button
			// 	{
			// 		type = Popup.ButtonType.Cancel,
			// 		text = Ln.Get("취소")
			// 	}
			// });
		}

		private void Ref()
		{
			Reference.Use(onNicknameConfirm);
		}
	}
}