using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TutorialDialogue : MonoBehaviour
	{
		private const int split = 20;


		private const float defaultcommetLineDelayTime = 2f;


		private const float defaultcommetLineDelayLongTime = 5f;


		private const float defaultCommentDelayTime = 0.05f;


		[SerializeField] private GameObject content = default;


		[SerializeField] private LnText txtName = default;


		[SerializeField] private Image imgCharacter = default;


		[SerializeField] private Image imgDialogue = default;


		[SerializeField] private LnText txtComment = default;


		[SerializeField] private GameObject skipButton = default;


		private readonly List<string> colorList = new List<string>
		{
			"yellow",
			"orange",
			"lime",
			"silver"
		};


		private readonly List<ColorString> colorStrings = new List<ColorString>();


		private readonly List<int> longDelayList = new List<int>
		{
			1,
			2,
			4,
			18,
			21,
			23
		};


		private CanvasGroup canvasGroup = default;


		private bool clickedSkipButton;


		private int code;


		private float commentDelayTime;


		private float commentLineDelayLongTime;


		private float commentLineDelayTime;


		private bool usingComment;

		private void Awake()
		{
			canvasGroup = content.GetComponent<CanvasGroup>();
		}


		public void SetCommentDelayTime(float commentDelayTime, float commentLineDelayTime,
			float commentLineDelayLongTime)
		{
			this.commentDelayTime = commentDelayTime;
			this.commentLineDelayTime = commentLineDelayTime;
			this.commentLineDelayLongTime = commentLineDelayLongTime;
		}


		public IEnumerator Show(int code)
		{
			if (usingComment)
			{
				yield return new WaitUntil(() => !usingComment);
			}

			this.code = code;
			content.SetActive(true);
			TutorialDialogueData tutorialDialogueData = GameDB.tutorial.GetTutorialDialogueData(code);
			SetCharacterName(tutorialDialogueData.characterCode);
			SetCharacterImage(tutorialDialogueData.characterCode);
			SetTutorialImage(tutorialDialogueData.img);
			yield return this.StartThrowingCoroutine(SetComment(Ln.Get(tutorialDialogueData.comment ?? "")), null);
		}


		public void Hide()
		{
			content.SetActive(false);
			canvasGroup.alpha = 0f;
		}


		private void SetCharacterName(int characterCode)
		{
			txtName.text = Ln.Get(LnType.Character_Name, characterCode.ToString());
		}


		private void SetCharacterImage(int characterCode)
		{
			imgCharacter.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterResultSprite(characterCode);
		}


		private void SetTutorialImage(string imgName)
		{
			if (imgName.Equals(string.Empty))
			{
				imgDialogue.gameObject.SetActive(false);
				return;
			}

			imgDialogue.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(imgName);
			imgDialogue.SetNativeSize();
			imgDialogue.gameObject.SetActive(true);
		}


		private string[] SplitCommentBySlush(string comment)
		{
			return comment.Split('@');
		}


		private List<string> SplitCommentByLengthCount(string comment)
		{
			List<string> list = new List<string>();
			int num = 0;
			int num2 = 21;
			int length = comment.Length;
			int num3 = comment.Length / 20;
			for (int i = 0; i < num3 + 1; i++)
			{
				int num4;
				if (length < num + num2)
				{
					num4 = length - num;
				}
				else
				{
					num4 = comment.Substring(num, num2).LastIndexOf("");
				}

				string item = comment.Substring(num, num4);
				list.Add(item);
				num += num4;
			}

			return list;
		}


		private IEnumerator SetComment(string comment)
		{
			string[] commentArray = SplitCommentBySlush(comment);
			yield return this.StartThrowingCoroutine(StartComment(commentArray), null);
		}


		private IEnumerator StartComment(string[] commentArray)
		{
			usingComment = true;
			foreach (string str in commentArray)
			{
				txtComment.text = "";
				colorStrings.Clear();
				foreach (string str2 in SplitColorString(str))
				{
					ColorGroup(str2);
				}

				SetCommentDelayTime(0.05f, 2f, 5f);
				clickedSkipButton = false;
				GameObject gameObject = skipButton;
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}

				foreach (ColorString colorString in colorStrings)
				{
					foreach (char c in colorString.str)
					{
						if (colorString.color == "white")
						{
							LnText lnText = txtComment;
							lnText.text += c.ToString();
						}
						else
						{
							LnText lnText2 = txtComment;
							lnText2.text += string.Format("<color={0}>{1}</color>", colorString.color, c);
						}

						if (commentDelayTime > 0f)
						{
							yield return new WaitForSeconds(commentDelayTime);
						}
					}

					// char[] array3 = null;
					// colorString = null;
				}

				if (clickedSkipButton)
				{
					yield return new WaitForSeconds(longDelayList.Contains(code)
						? commentLineDelayLongTime
						: commentLineDelayTime);
				}
				else
				{
					float time = 0f;
					float delayTime = longDelayList.Contains(code) ? commentLineDelayLongTime : commentLineDelayTime;
					for (;;)
					{
						time += Time.deltaTime;
						if (time > delayTime || clickedSkipButton)
						{
							break;
						}

						yield return null;
					}
				}
			}

			usingComment = false;
		}


		private void ColorGroup(string str)
		{
			if (!str.Contains("<color"))
			{
				colorStrings.Add(new ColorString("white", str));
				return;
			}

			int num = str.IndexOfAny(new[]
			{
				'<'
			});
			int num2 = str.IndexOfAny(new[]
			{
				'>'
			});
			if (num == 0)
			{
				using (List<string>.Enumerator enumerator = colorList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						if (str.Contains(text))
						{
							int num3 = num2 + 1;
							int length = str.Length - num3;
							string str2 = str.Substring(num3, length);
							colorStrings.Add(new ColorString(text, str2));
							break;
						}
					}

					return;
				}
			}

			string str3 = str.Substring(0, num);
			colorStrings.Add(new ColorString("white", str3));
			int startIndex = num;
			int length2 = str.Length - num;
			string str4 = str.Substring(startIndex, length2);
			ColorGroup(str4);
		}


		private string[] SplitColorString(string str)
		{
			return str.Split(new[]
			{
				"</color>"
			}, StringSplitOptions.None);
		}


		public void ClickedSkip()
		{
			clickedSkipButton = true;
			GameObject gameObject = skipButton;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}

			SetCommentDelayTime(0f, 1f, 1f);
		}


		public class ColorString
		{
			public string color;


			public string str;

			public ColorString(string color, string str)
			{
				this.color = color;
				this.str = str;
			}
		}
	}
}