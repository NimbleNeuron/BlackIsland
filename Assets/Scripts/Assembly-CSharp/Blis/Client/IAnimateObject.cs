using System.Collections;
using UnityEngine;

namespace Blis.Client
{
	public interface IAnimateObject
	{
		Coroutine StartCoroutine(IEnumerator enumerator);


		Vector3 GetPosition();


		Transform GetTransform();


		int GetObjectId();


		bool IsCharacter();


		int CharacterCode();


		int SkinIndex();


		bool IsLobbyObject();


		bool CanPlayEvent();


		GameObject LoadEffect(string effectName);


		AudioClip LoadFXSound(string soundName);


		AudioClip LoadVoice(string characterResource, string voiceName, int randomCount = 0);
	}
}