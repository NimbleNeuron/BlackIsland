using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class ClientCharacter : MonoBehaviour, IAnimateObject
	{
		public bool isCharacter;
		public int characterCode;
		public int skinIndex;
		public bool isLobbyCharacter;
		private ActionFrameEvent actionFrameEvent;
		private bool inSight = true;
		private LocalObject localObject;
		private int objectId;
		private bool stealth;
		private bool visible;

		public void Awake()
		{
			if (this.gameObject.GetComponent<Animator>() != null)
			{
				GameUtil.BindOrAdd<ActionFrameEvent>(gameObject, ref actionFrameEvent);
			}
			else
			{
				GameObject gameObject = transform.GetChild(0).gameObject;
				if (gameObject != null && gameObject.GetComponent<Animator>() != null)
				{
					GameUtil.BindOrAdd<ActionFrameEvent>(gameObject, ref actionFrameEvent);
				}
			}

			actionFrameEvent.Init(this);
		}


		public Vector3 GetPosition()
		{
			return transform.position;
		}


		public Transform GetTransform()
		{
			return transform;
		}


		public int GetObjectId()
		{
			return objectId;
		}


		public bool IsCharacter()
		{
			return isCharacter;
		}


		public int CharacterCode()
		{
			return characterCode;
		}


		public int SkinIndex()
		{
			return skinIndex;
		}


		public bool IsLobbyObject()
		{
			return isLobbyCharacter;
		}


		public bool CanPlayEvent()
		{
			return isLobbyCharacter || inSight && !stealth || visible;
		}


		public GameObject LoadEffect(string effectName)
		{
			if (localObject != null)
			{
				return localObject.LoadEffect(effectName);
			}

			if (characterCode != 0)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect(characterCode, skinIndex, effectName);
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect(effectName);
		}


		public AudioClip LoadFXSound(string soundName)
		{
			if (localObject != null)
			{
				return localObject.LoadFXSound(soundName);
			}

			if (characterCode != 0)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadFXSound(characterCode, skinIndex, soundName);
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.LoadFXSound(soundName);
		}


		public AudioClip LoadVoice(string characterResource, string voiceName, int randomCount = 0)
		{
			if (localObject != null)
			{
				return localObject.LoadVoice(characterResource, voiceName, randomCount);
			}

			if (characterCode != 0)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(characterCode, skinIndex,
					characterResource, voiceName, randomCount);
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(voiceName);
		}


		Coroutine IAnimateObject.StartCoroutine(IEnumerator enumerator)
		{
			return base.StartCoroutine(enumerator);
		}

		public void Init(LocalObject localObject, int objectId)
		{
			this.localObject = localObject;
			this.objectId = objectId;
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				stealth = false;
				inSight = true;
				visible = true;
			}
		}


		public void Init(int characterCode, int skinIndex)
		{
			this.characterCode = characterCode;
			this.skinIndex = skinIndex;
		}


		public void AddOnEffectCreatedEvent(Action<GameObject, bool> e)
		{
			actionFrameEvent.onAnimationEventCreated -= e;
			actionFrameEvent.onAnimationEventCreated += e;
		}


		public void SetStealth(bool stealth)
		{
			this.stealth = stealth;
		}


		public void SetInSight(bool inSight)
		{
			this.inSight = inSight;
		}


		public void SetVisible(bool visible)
		{
			this.visible = visible;
		}


		public void SetNoParentEffectByTag(string tag)
		{
			actionFrameEvent.SetNoParentEffectByTag(tag);
		}


		public void StopEffectByTag(string tag)
		{
			actionFrameEvent.StopEffectByTag(tag);
		}


		public void StopSoundByTag(string tag)
		{
			actionFrameEvent.StopSoundByTag(tag);
		}
	}
}