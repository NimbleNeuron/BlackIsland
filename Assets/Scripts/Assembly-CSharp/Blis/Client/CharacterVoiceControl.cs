using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class CharacterVoiceControl
	{
		private const string SoundTag = "InGameVoice";


		private readonly int characterCode;


		private readonly Dictionary<CharacterVoiceType, CharacterVoiceInfo> dicCharInfos =
			new Dictionary<CharacterVoiceType, CharacterVoiceInfo>();


		private readonly LocalPlayerCharacter localPlayerCharacter;


		private readonly int objectId;


		private readonly Transform parent;


		private readonly List<CharacterVoiceType> playList = new List<CharacterVoiceType>();


		private readonly int skinIndex;

		public CharacterVoiceControl(LocalPlayerCharacter localPlayerCharacter)
		{
			this.localPlayerCharacter = localPlayerCharacter;
			parent = localPlayerCharacter.transform;
			objectId = localPlayerCharacter.ObjectId;
			characterCode = localPlayerCharacter.CharacterCode;
			skinIndex = localPlayerCharacter.SkinIndex;
		}


		public void PlayCharacterVoice(string customSoundName, int maxDistance, Vector3 position, bool listenerEveryone,
			bool spatial3D = true, bool immediatelyPlay = false)
		{
			if (!CheckListener(MonoBehaviourInstance<ClientService>.inst.MyObjectId, listenerEveryone))
			{
				return;
			}

			if (localPlayerCharacter.IsOutSight)
			{
				return;
			}

			string resource = GameDB.character.GetCharacterData(characterCode).resource;
			AudioClip audioClip =
				SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(characterCode, skinIndex, resource,
					customSoundName);
			Singleton<SoundControl>.inst.PlayCharacterVoiceSound(parent, audioClip, objectId, "InGameVoice",
				maxDistance, position, spatial3D, immediatelyPlay);
		}


		public void PlayCharacterVoice(CharacterVoiceType charVoiceType, int maxDistance, Vector3 position,
			string customSoundName = null, SkillSlotSet skillSet = SkillSlotSet.None)
		{
			if (charVoiceType == CharacterVoiceType.None)
			{
				return;
			}

			CharacterVoiceData characterVoiceData = GameDB.characterVoice.GetCharacterVoiceData(charVoiceType);
			if (characterVoiceData == null)
			{
				return;
			}

			CharacterVoiceRandomCountData characterVoiceRandomCountData =
				GameDB.characterVoice.GetCharacterVoiceRandomCountData(charVoiceType, characterCode, skinIndex,
					skillSet);
			if (characterVoiceRandomCountData == null)
			{
				return;
			}

			if (characterVoiceRandomCountData.randomCount == 0)
			{
				return;
			}

			if (characterVoiceData.immediatelyPlay)
			{
				Play(characterVoiceData, characterVoiceRandomCountData.randomCount, charVoiceType, maxDistance,
					position, customSoundName);
				return;
			}

			if (!IsBattlePlaying(characterVoiceData.battlePlaying))
			{
				return;
			}

			if (IsCoolTime(charVoiceType))
			{
				return;
			}

			if (IsRepeatOnceIgnore(charVoiceType, characterVoiceData.onceRepeatIgnore))
			{
				int useCaseMinCoolTime = characterVoiceData.useCaseMinCoolTime;
				int useCaseMaxCoolTime = characterVoiceData.useCaseMaxCoolTime;
				int globalCoolTime = characterVoiceData.globalCoolTime;
				int b = Random.Range(useCaseMinCoolTime, useCaseMaxCoolTime + 1);
				int num = Mathf.Max(globalCoolTime, b);
				dicCharInfos[charVoiceType].AddCoolTime(num);
				return;
			}

			if (IsLimit(charVoiceType, characterVoiceData.maxCount))
			{
				return;
			}

			if (!CheckListener(MonoBehaviourInstance<ClientService>.inst.MyObjectId,
				characterVoiceData.listener == ListenerType.Everyone))
			{
				return;
			}

			Play(characterVoiceData, characterVoiceRandomCountData.randomCount, charVoiceType, maxDistance, position,
				customSoundName);
		}


		private void Play(CharacterVoiceData charVoiceData, int maxCount, CharacterVoiceType charVoiceType,
			int maxDistance, Vector3 position, string customSoundName)
		{
			if (localPlayerCharacter.IsOutSight)
			{
				return;
			}

			string resource = GameDB.character.GetCharacterData(characterCode).resource;
			int randomCount = Random.Range(1, maxCount + 1);
			AudioClip audioClip = SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(characterCode, skinIndex,
				resource, customSoundName ?? charVoiceData.soundName, randomCount);
			bool spatial3D = charVoiceData.spatial3D;
			Singleton<SoundControl>.inst.PlayCharacterVoiceSound(parent, audioClip, objectId, "InGameVoice",
				maxDistance, position, spatial3D, charVoiceData.immediatelyPlay);
			if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == objectId)
			{
				int useCaseMinCoolTime = charVoiceData.useCaseMinCoolTime;
				int useCaseMaxCoolTime = charVoiceData.useCaseMaxCoolTime;
				int globalCoolTime = charVoiceData.globalCoolTime;
				int b = Random.Range(useCaseMinCoolTime, useCaseMaxCoolTime + 1);
				AddPlayInfo(charVoiceType, Time.time, Mathf.Max(globalCoolTime, b));
			}
		}


		private bool IsBattlePlaying(bool battlePlaying)
		{
			return !MonoBehaviourInstance<ClientService>.inst.World.Find<LocalPlayerCharacter>(objectId).IsInCombat ||
			       battlePlaying;
		}


		private bool IsCoolTime(CharacterVoiceType charVoiceType)
		{
			if (!dicCharInfos.ContainsKey(charVoiceType))
			{
				return false;
			}

			float startTime = dicCharInfos[charVoiceType].StartTime;
			float coolTime = dicCharInfos[charVoiceType].CoolTime;
			return Time.time <= startTime + coolTime;
		}


		private bool IsRepeatOnceIgnore(CharacterVoiceType charVoiceType, bool repeatOnceIgnore)
		{
			if (playList.Contains(charVoiceType) && repeatOnceIgnore)
			{
				playList.Remove(charVoiceType);
				return true;
			}

			return false;
		}


		private bool IsLimit(CharacterVoiceType charVoiceType, int maxCount)
		{
			return dicCharInfos.ContainsKey(charVoiceType) && dicCharInfos[charVoiceType].playVoiceCount >= maxCount;
		}


		private void AddPlayInfo(CharacterVoiceType charVoiceType, float startTime, float coolTime)
		{
			playList.Add(charVoiceType);
			if (dicCharInfos.ContainsKey(charVoiceType))
			{
				dicCharInfos[charVoiceType].playVoiceCount++;
				dicCharInfos[charVoiceType].StartTime = startTime;
				dicCharInfos[charVoiceType].CoolTime = 0f;
				dicCharInfos[charVoiceType].AddCoolTime(coolTime);
				return;
			}

			CharacterVoiceInfo characterVoiceInfo = new CharacterVoiceInfo();
			characterVoiceInfo.charVoiceType = charVoiceType;
			characterVoiceInfo.playVoiceCount = 1;
			characterVoiceInfo.StartTime = startTime;
			characterVoiceInfo.AddCoolTime(coolTime);
			dicCharInfos.Add(charVoiceType, characterVoiceInfo);
		}


		private bool CheckListener(int myObjectId, bool listenerEveryone)
		{
			return listenerEveryone || myObjectId == objectId;
		}


		public IEnumerator DelaySoundPlay(Action delayAction, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			delayAction();
		}
	}
}