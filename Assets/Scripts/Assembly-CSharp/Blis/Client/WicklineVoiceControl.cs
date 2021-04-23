using System;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class WicklineVoiceControl
	{
		private readonly LocalMonster monster;
		private WicklineActionType actionType;


		private bool isPlaying;


		private Coroutine loopRoutine;


		private Coroutine voiceCoroutine;


		private WicklineVoiceType voiceType;


		public WicklineVoiceControl(LocalMonster monster)
		{
			this.monster = monster;
		}


		public bool IsPlaying => isPlaying;


		public WicklineActionType ActionType => actionType;


		public WicklineVoiceType VoiceType => voiceType;


		public void PlayWicklineVoice(WicklineActionType actionType, WicklineVoiceType voiceType)
		{
			this.actionType = actionType;
			this.voiceType = voiceType;
			isPlaying = true;
			string wicklineVoiceTypeConvertToSoundName = GetWicklineVoiceTypeConvertToSoundName(voiceType);
			int wicklineVoiceTypeToRandomNumber = GetWicklineVoiceTypeToRandomNumber(voiceType);
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.Append(wicklineVoiceTypeConvertToSoundName);
			stringBuilder.Append("_");
			stringBuilder.Append(wicklineVoiceTypeToRandomNumber);
			AudioClip audioClip =
				SingletonMonoBehaviour<ResourceManager>.inst.LoadCommonVoice(stringBuilder.ToString());
			Singleton<SoundControl>.inst.PlayCharacterVoiceSound(monster.transform, audioClip, monster.ObjectId,
				"InGameVoice", 15, monster.GetPosition(), true, false);
		}


		public void PlayLoopWicklineVoice(WicklineActionType actionType)
		{
			WicklineVoiceType wicklineVoiceType = WicklineVoiceType.None;
			if (actionType == WicklineActionType.StartMove)
			{
				if (MonoBehaviourInstance<ClientService>.inst.DayNight == DayNight.Day)
				{
					wicklineVoiceType = WicklineVoiceType.NormalDay;
				}
				else if (MonoBehaviourInstance<ClientService>.inst.DayNight == DayNight.Night)
				{
					wicklineVoiceType = WicklineVoiceType.NormalNight;
				}
			}

			PlayWicklineVoice(actionType, wicklineVoiceType);
			float wicklineVoiceLength = GetWicklineVoiceLength();
			loopRoutine = MonoBehaviourInstance<GameClient>.inst.StartThrowingCoroutine(
				CoroutineUtil.DelayedAction(wicklineVoiceLength, delegate { PlayLoopWicklineVoice(actionType); }),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][VOICE] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		public void StopWicklineVoice()
		{
			actionType = WicklineActionType.None;
			voiceType = WicklineVoiceType.None;
			isPlaying = false;
			Singleton<SoundControl>.inst.StopCharacterVoice(monster.ObjectId);
			if (loopRoutine != null)
			{
				MonoBehaviourInstance<GameClient>.inst.StopCoroutine(loopRoutine);
				loopRoutine = null;
			}
		}


		private string GetWicklineVoiceTypeConvertToSoundName(WicklineVoiceType wicklineVoiceType)
		{
			string result = string.Empty;
			switch (wicklineVoiceType)
			{
				case WicklineVoiceType.NormalDay:
					result = "wicklineDay";
					break;
				case WicklineVoiceType.NormalNight:
					result = "wicklineNight";
					break;
				case WicklineVoiceType.CombatStart:
					result = "wicklineToBattle";
					break;
				case WicklineVoiceType.CombatEnd:
					result = "wicklineEndBattle";
					break;
			}

			return result;
		}


		private int GetWicklineVoiceTypeToRandomNumber(WicklineVoiceType wicklineVoiceType)
		{
			int result = 1;
			switch (wicklineVoiceType)
			{
				case WicklineVoiceType.NormalDay:
					result = Random.Range(1, 5);
					break;
				case WicklineVoiceType.NormalNight:
					result = Random.Range(1, 6);
					break;
				case WicklineVoiceType.CombatStart:
					result = Random.Range(1, 7);
					break;
				case WicklineVoiceType.CombatEnd:
					result = Random.Range(1, 6);
					break;
			}

			return result;
		}


		private float GetWicklineVoiceLength()
		{
			return Singleton<SoundControl>.inst.GetCharacterVoiceSoundLength(monster.ObjectId);
		}
	}
}