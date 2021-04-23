using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ReportReasonList : BaseUI
	{
		public delegate void OnclickReasonButton(ReportType reportType);


		private const int REPORT_TEAM_REASON_COUNT = 2;


		private readonly List<Button> reasonButtons = new List<Button>();


		private readonly List<Text> reasonTexts = new List<Text>();


		private RectTransform buttonsObject;


		private CanvasGroup canvasGroup;


		public OnclickReasonButton ClickReasonButton;


		private int reasonCount;

		private void Update()
		{
			if (Input.GetMouseButtonUp(0) && IsShow())
			{
				this.StartThrowingCoroutine(CoroutineUtil.FrameDelayedAction(1, Hide),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][ReportHide] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
			}
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			buttonsObject = GameUtil.Bind<RectTransform>(gameObject, "Buttons");
			GameUtil.Bind<Transform>(gameObject, "Buttons").GetComponentsInChildren<Button>(true, reasonButtons);
			reasonCount = Enum.GetValues(typeof(ReportType)).Length;
			int index = 0;
			foreach (ReportType reportType1 in Enum.GetValues(typeof(ReportType)))
			{
				ReportType reportType = reportType1;
				if (index < reasonButtons.Count)
				{
					reasonButtons[index].onClick.AddListener(() =>
					{
						OnClick(reportType);
						Hide();
					});
					reasonTexts.Add(reasonButtons[index].transform.GetComponentInChildren<Text>());
					reasonTexts[index].text = Ln.Get(reportType.ToString());
					++index;
				}
				else
				{
					break;
				}
			}

			Hide();

			// co: dotPeek
			// base.OnAwakeUI();
			// GameUtil.Bind<CanvasGroup>(base.gameObject, ref this.canvasGroup);
			// this.buttonsObject = GameUtil.Bind<RectTransform>(base.gameObject, "Buttons");
			// GameUtil.Bind<Transform>(base.gameObject, "Buttons").GetComponentsInChildren<Button>(true, this.reasonButtons);
			// this.reasonCount = Enum.GetValues(typeof(ReportType)).Length;
			// int num = 0;
			// using (IEnumerator enumerator = Enum.GetValues(typeof(ReportType)).GetEnumerator())
			// {
			// 	while (enumerator.MoveNext())
			// 	{
			// 		ReportType reportType = (ReportType)enumerator.Current;
			// 		if (num >= this.reasonButtons.Count)
			// 		{
			// 			break;
			// 		}
			// 		this.reasonButtons[num].onClick.AddListener(delegate()
			// 		{
			// 			this.OnClick(reportType);
			// 			this.Hide();
			// 		});
			// 		this.reasonTexts.Add(this.reasonButtons[num].transform.GetComponentInChildren<Text>());
			// 		this.reasonTexts[num].text = Ln.Get(reportType.ToString());
			// 		num++;
			// 	}
			// }
			// this.Hide();
		}


		public void Show(int slotIndex, bool isMyTeam)
		{
			gameObject.SetActive(true);
			for (int i = reasonCount - 2; i < reasonCount; i++)
			{
				reasonButtons[i].transform.parent.gameObject.SetActive(isMyTeam);
			}

			transform.position = Input.mousePosition;
			if (slotIndex < 16)
			{
				ChangeButtonsPivot(Pivot.LeftTop);
				return;
			}

			ChangeButtonsPivot(Pivot.LeftBottom);
		}


		private void OnClick(ReportType reportType)
		{
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("PlayerReportConfirm", Ln.Get(reportType.ToString())),
				new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("확인"),
					callback = delegate { ClickReasonButton(reportType); }
				}, new Popup.Button
				{
					type = Popup.ButtonType.Cancel,
					text = Ln.Get("취소")
				});
		}


		private void ChangeButtonsPivot(Pivot pivot)
		{
			if (buttonsObject == null)
			{
				return;
			}

			Vector2 vector2 = new Vector2((int) (pivot & Pivot.RightBottom) >> 1, (float) (pivot & Pivot.LeftTop));
			if (!(buttonsObject.pivot != vector2))
			{
				return;
			}

			buttonsObject.pivot = vector2;

			// co: dotPeek
			// if (this.buttonsObject == null)
			// {
			// 	return;
			// }
			// int num = (int)((pivot & ReportReasonList.Pivot.RightBottom) >> 1);
			// int num2 = (int)(pivot & ReportReasonList.Pivot.LeftTop);
			// Vector2 vector = new Vector2((float)num, (float)num2);
			// if (this.buttonsObject.pivot != vector)
			// {
			// 	this.buttonsObject.pivot = vector;
			// }
		}


		public bool IsShow()
		{
			return gameObject.activeSelf;
		}


		public void Hide()
		{
			gameObject.SetActive(false);
		}


		private enum Pivot
		{
			LeftBottom,

			LeftTop,

			RightBottom,

			RightTop
		}
	}
}