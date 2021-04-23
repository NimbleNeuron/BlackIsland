using System;
using System.Collections;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ToastMessageUI : BaseUI
	{
		private CanvasAlphaTweener alphaTweener;


		private CanvasGroup canvasGroup;


		private Text message;


		private Coroutine showMessage;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			alphaTweener = GetComponent<CanvasAlphaTweener>();
			canvasGroup.alpha = 0f;
			message = GetComponentInChildren<Text>();
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			gameObject.SetActive(true);
		}


		public void ShowMessage(string toastMessage, float duration = 3f)
		{
			if (showMessage != null)
			{
				StopCoroutine(showMessage);
			}

			message.text = toastMessage;
			canvasGroup.alpha = 1f;
			showMessage = this.StartThrowingCoroutine(ShowMessage(duration),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][ShowMessage] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			if (alphaTweener != null)
			{
				alphaTweener.StopAnimation();
				alphaTweener.PlayAnimation();
			}
		}


		private IEnumerator ShowMessage(float duration)
		{
			float time = 0f;
			for (;;)
			{
				time += Time.deltaTime;
				if (time > duration)
				{
					break;
				}

				yield return null;
			}

			canvasGroup.alpha = 0f;
			showMessage = null;
		}


		public bool isShowing()
		{
			return canvasGroup.alpha == 1f;
		}
	}
}