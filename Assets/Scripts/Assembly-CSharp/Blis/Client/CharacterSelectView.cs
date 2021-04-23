using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public abstract class CharacterSelectView : BaseUI
	{
		private Action onFinishTimer;


		private LnText phaseText;


		private LnText remainTime;


		private UIProgress remainTimeBar;


		private Coroutine timer;


		private Coroutine timerSound;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			remainTimeBar = GameUtil.Bind<UIProgress>(gameObject, "Notification/Progress");
			phaseText = GameUtil.Bind<LnText>(gameObject, "Notification/Timer/TextGroup/Left");
			remainTime = GameUtil.Bind<LnText>(gameObject, "Notification/Timer/TextGroup/Time");
		}


		public virtual void Open()
		{
			gameObject.SetActive(true);
			remainTimeBar.SetValue(0f);
			remainTime.text = null;
			SetTimer(GameConstants.MatchingCharacterSelectTime, 0);
			SetTimerSound(GameConstants.MatchingCharacterSelectTime);
		}


		public virtual void Close()
		{
			StopTimer();
			StopTimerSound();
			gameObject.SetActive(false);
		}


		public void SetFinishTimerAction(Action action)
		{
			onFinishTimer = action;
		}


		public void SetPhase(string phaseName, int standByTime, int breakTime)
		{
			SetTimer(standByTime, breakTime);
			StopTimerSound();
			SetPhaseText(phaseName);
		}


		public virtual void CharacterSelect(int characterCode, int startingDataCode, bool mySelect) { }


		public virtual void SkinSelect(int characterCode, int skinCode, bool mySelect) { }


		public virtual void WeaponSelect(int startingDataCode) { }


		public virtual void PickCharacter(MatchingService.MatchingUser userInfo) { }


		public virtual void UpdateMyTeam(MatchingService.MatchingUser userInfo) { }


		public virtual void PickMyCharacter(int characterCode, int skinCode, bool isSinglePlay) { }


		public virtual void CharacterCancelMyPick(int characterCode) { }


		public virtual void CharacterCancelPick(int characterCode) { }


		public virtual void CharacterSelect(int teamNumber, long userNum, int characterCode, int startingDataCode) { }


		public virtual void SkinSelect(int teamNumber, MatchingService.MatchingUser userInfo) { }


		public virtual void WeaponSelect(int teamNumber, long userNum, int startingDataCode) { }


		public virtual void PickCharacter(int teamNumber, MatchingService.MatchingUser userInfo) { }


		public virtual void UpdateMyTeam(int teamNumber, MatchingService.MatchingUser userInfo) { }


		public virtual void CharacterCancelPick(int teamNumber, MatchingService.MatchingUser userInfo) { }


		private void SetTimer(int seconds, int breakTime)
		{
			StopTimer();
			timer = this.StartThrowingCoroutine(StartTimer(seconds, breakTime),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][StartTimer] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private void StopTimer()
		{
			if (timer != null)
			{
				StopCoroutine(timer);
				timer = null;
			}
		}


		private void SetTimerSound(int seconds)
		{
			StopTimerSound();
			timerSound = this.StartThrowingCoroutine(PlaySoundCountDown(seconds),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][StartTimerSound] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private void StopTimerSound()
		{
			if (timerSound != null)
			{
				StopCoroutine(timerSound);
				timerSound = null;
			}
		}


		private IEnumerator PlaySoundCountDown(float TotalTime)
		{
			int countDown = 11;
			float seconds = TotalTime - countDown;
			yield return new WaitForSeconds(seconds);
			int time = 0;
			do
			{
				Singleton<SoundControl>.inst.PlayUISound("StrategyMapCount");
				yield return new WaitForSeconds(1f);
				time++;
			} while (time != countDown);
		}


		private IEnumerator StartTimer(float standbySecond, float breakTime)
		{
			remainTime.gameObject.SetActive(true);
			DateTime startTime = DateTime.Now.AddSeconds(standbySecond - breakTime);
			TimeSpan timeSpan;
			do
			{
				yield return null;
				timeSpan = startTime - DateTime.Now;
				remainTimeBar.SetValue(1f - (float) timeSpan.TotalSeconds / standbySecond);
				remainTime.text = string.Format("{0}", Mathf.CeilToInt((float) timeSpan.TotalSeconds));
			} while (timeSpan.TotalSeconds > 0.0);

			remainTime.gameObject.SetActive(false);
			SetPhaseText(Ln.Get("잠시 뒤 실험이 시작됩니다."));
			Action action = onFinishTimer;
			if (action != null)
			{
				action();
			}
		}


		private void SetPhaseText(string phase)
		{
			phaseText.text = phase;
		}


		public virtual void SetOnClickExit(Action action) { }


		public virtual void StandBy()
		{
			SetPhase(Ln.Get("실험 시작까지 남은 시간"), GameConstants.GameStartStandByTime, 0);
		}
	}
}