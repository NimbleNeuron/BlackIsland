using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class CastingHud : BaseUI
	{
		private CanvasGroup canvasGroup;


		private Coroutine caster;


		private Action<bool> curCallback;


		private UIProgress progress;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			progress = GameUtil.Bind<UIProgress>(gameObject, "Progress");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			gameObject.SetActive(true);
		}


		public void StartCasting(string skillName, float duration, CastingBarType barType, Action<bool> callback)
		{
			if (duration <= 0f)
			{
				return;
			}

			float dir = 1f;
			if (barType != CastingBarType.LeftToRight)
			{
				if (barType == CastingBarType.RightToLeft)
				{
					dir = -1f;
				}
			}
			else
			{
				dir = 1f;
			}

			CancelCasting();
			progress.SetLabel(skillName);
			curCallback = callback;
			caster = this.StartThrowingCoroutine(Casting(duration, dir, delegate { canvasGroup.alpha = 0f; }),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][Casting] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		public void StartConcentration(string skillName, float duration, CastingBarType barType, Action<bool> callback)
		{
			if (duration <= 0f)
			{
				return;
			}

			float dir = 1f;
			if (barType != CastingBarType.LeftToRight)
			{
				if (barType == CastingBarType.RightToLeft)
				{
					dir = -1f;
				}
			}
			else
			{
				dir = 1f;
			}

			CancelCasting();
			progress.SetLabel(skillName);
			curCallback = callback;
			caster = this.StartThrowingCoroutine(Casting(duration, dir, null),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][Casting] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		public void CancelCasting()
		{
			if (caster != null)
			{
				StopCoroutine(caster);
				caster = null;
				Action<bool> action = curCallback;
				if (action != null)
				{
					action(false);
				}

				curCallback = null;
			}

			canvasGroup.alpha = 0f;
		}


		public bool IsCasting()
		{
			return caster != null;
		}


		private IEnumerator Casting(float duration, float dir, Action finish)
		{
			canvasGroup.alpha = 1f;
			float delta = dir / duration;
			float time = 0f;
			float value = 0f < dir ? 0f : 1f;
			for (;;)
			{
				time += Time.deltaTime;
				value += delta * Time.deltaTime;
				progress.SetValue(value);
				if (time > duration)
				{
					break;
				}

				yield return null;
			}

			Action<bool> action = curCallback;
			if (action != null)
			{
				action(true);
			}

			curCallback = null;
			caster = null;
			if (finish != null)
			{
				finish();
			}
		}
	}
}