using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public abstract class LocalObject : ObjectBase
	{
		protected static ISerializer serializer = Serializer.Default;
		private readonly Dictionary<string, int> childEffectCounter = new Dictionary<string, int>();
		private readonly Dictionary<string, GameObject> childEffects = new Dictionary<string, GameObject>();
		private readonly List<ParticleSystem> destroyEffectParticleSystems = new List<ParticleSystem>();
		[NonSerialized] public List<LocalSightAgent> attachedSights;
		protected LocalSkillPlayer localSkillPlayer;
		public LocalSkillPlayer LocalSkillPlayer => localSkillPlayer;
		public bool IsOutSight => GetIsOutSight();

		protected abstract bool GetIsOutSight();

		public abstract void Init(byte[] snapshotData);

		public virtual void DestroySelf() { }

		public virtual GameObject ReleaseChildren()
		{
			return null;
		}

		public virtual void OnEffectCreated(GameObject obj, bool addCharacterRenderer) { }

		public virtual string GetLocalizedName(bool includeColor)
		{
			return "";
		}

		public virtual void LookAt(Quaternion lookFrom, Quaternion lookTo, float angularSpeed)
		{
			SetRotation(lookTo);
		}


		protected Pickable AttachPickable(GameObject parent)
		{
			if (parent == null)
			{
				parent = new GameObject(typeof(Pickable).Name);
				parent.transform.parent = transform;
				parent.transform.localPosition = Vector3.zero;
				parent.transform.localRotation = Quaternion.identity;
			}

			bool isDebugBuild = BSERVersion.isDebugBuild;
			Pickable result = null;
			GameUtil.BindOrAdd<Pickable>(parent, ref result);
			return result;
		}


		public void PlayLocalEffectWorldPoint(int effectAndSoundCode, Vector3 worldPosition,
			Quaternion? effectRotation = null)
		{
			PlayLocalDamagedEffectWorldPoint(this, effectAndSoundCode, worldPosition, effectRotation);
		}


		public void PlayLocalEffect(int effectAndSoundCode, Quaternion? effectRotation = null)
		{
			PlayLocalDamagedEffect(this, effectAndSoundCode, effectRotation);
		}


		public void PlayLocalDamagedEffectWorldPoint(LocalObject effectOwner, int effectAndSoundCode,
			Vector3 worldPosition, Quaternion? effectRotation = null)
		{
			if (effectAndSoundCode <= 0)
			{
				return;
			}

			if (IsOutSight)
			{
				return;
			}

			if (effectOwner == null)
			{
				return;
			}

			EffectAndSoundData effectSoundData = GameDB.effectAndSound.GetEffectSoundData(effectAndSoundCode);
			if (effectSoundData != null)
			{
				GameObject gameObject = effectOwner.LoadEffect(effectSoundData.effectPrefabName);
				AudioClip audioClip = effectOwner.LoadFXSound(effectSoundData.soundName);
				if (gameObject != null)
				{
					PlayLocalEffectWorldPoint(gameObject, worldPosition, effectRotation);
				}

				if (audioClip != null)
				{
					PlayLocalSound(audioClip, effectSoundData, effectOwner);
				}
			}
		}


		public void PlayLocalDamagedEffect(LocalObject effectOwner, int effectAndSoundCode,
			Quaternion? effectRotation = null)
		{
			if (effectAndSoundCode <= 0)
			{
				return;
			}

			if (IsOutSight)
			{
				return;
			}

			if (effectOwner == null)
			{
				return;
			}

			EffectAndSoundData effectSoundData = GameDB.effectAndSound.GetEffectSoundData(effectAndSoundCode);
			if (effectSoundData != null)
			{
				GameObject gameObject = effectOwner.LoadEffect(effectSoundData.effectPrefabName);
				AudioClip audioClip = effectOwner.LoadFXSound(effectSoundData.soundName);
				if (gameObject != null)
				{
					PlayLocalEffect(gameObject, effectSoundData, effectRotation);
				}

				if (audioClip != null)
				{
					PlayLocalSound(audioClip, effectSoundData, effectOwner);
				}
			}
		}


		private void PlayLocalEffect(GameObject resource, EffectAndSoundData data, Quaternion? effectRotation = null)
		{
			if (data.attachParent)
			{
				PlayLocalEffectChild(resource, data.effectParentName);
				return;
			}

			PlayLocalEffectPoint(resource, data.effectParentName, effectRotation);
		}


		private void PlayLocalSound(AudioClip audioClip, EffectAndSoundData data, LocalObject effectOwner)
		{
			if (!string.IsNullOrEmpty(data.soundMixer) && !string.IsNullOrEmpty(data.soundName))
			{
				if (data.childSound)
				{
					Singleton<SoundControl>.inst.PlayFXSoundChild(audioClip, data.soundTag, data.soundMaxDistance,
						data.loop, effectOwner.transform, true);
					return;
				}

				PlayLocalSoundPoint(audioClip, data.soundTag, data.soundMaxDistance, data.effectParentName);
			}
		}


		protected virtual void PlayLocalSoundPoint(string soundName, string tag, int maxDistance, Vector3 pos)
		{
			if (IsOutSight)
			{
				return;
			}

			Singleton<SoundControl>.inst.PlayFXSound(soundName, tag, maxDistance, pos, false);
		}


		private void PlayLocalSoundPoint(string soundName, string tag, int maxDistance, string effectParentName)
		{
			if (IsOutSight)
			{
				return;
			}

			Transform transform = FindParent(effectParentName);
			Singleton<SoundControl>.inst.PlayFXSound(soundName, tag, maxDistance, transform.position, false);
		}


		private void PlayLocalSoundPoint(AudioClip audioClip, string tag, int maxDistance, string effectParentName)
		{
			if (IsOutSight)
			{
				return;
			}

			Transform transform = FindParent(effectParentName);
			Singleton<SoundControl>.inst.PlayFXSound(audioClip, tag, maxDistance, transform.position, false);
		}


		public GameObject PlayLocalEffectPoint(string effectName, object param, Quaternion? effectRotation = null)
		{
			if (IsOutSight)
			{
				return null;
			}

			GameObject gameObject = LoadEffect(effectName);
			if (gameObject == null)
			{
				Log.E("[PlayLocalEffectPoint] Resource does not exist : " + effectName);
				return null;
			}

			return PlayLocalEffectPoint(gameObject, param, effectRotation);
		}


		public GameObject PlayLocalEffectChild(string effectName, string parentName)
		{
			if (string.IsNullOrEmpty(effectName))
			{
				Log.W("OnPlayEffectChild : EffectName is empty");
				return null;
			}

			GameObject gameObject = LoadEffect(effectName);
			if (gameObject == null)
			{
				Log.E("[PlayLocalEffectPoint] Resource does not exist : " + effectName);
				return null;
			}

			return PlayLocalEffectChild(gameObject, parentName);
		}


		public GameObject PlayLocalEffectChild(GameObject resource, string parentName)
		{
			Transform parent = transform;
			if (!string.IsNullOrEmpty(parentName))
			{
				parent = transform.FindRecursively(parentName);
			}

			GameObject gameObject = Instantiate<GameObject>(resource, parent);
			OnEffectCreated(gameObject, true);
			return gameObject;
		}


		private Transform FindParent(string effectParentName)
		{
			Transform result = this.transform;
			if (!string.IsNullOrEmpty(effectParentName))
			{
				Transform transform = this.transform.FindRecursively(effectParentName);
				if (transform != null)
				{
					result = transform;
				}
			}

			return result;
		}


		private GameObject PlayLocalEffectWorldPoint(GameObject resource, Vector3 worldPosition,
			Quaternion? effectRotation = null)
		{
			if (IsOutSight)
			{
				return null;
			}

			GameObject gameObject = Instantiate<GameObject>(resource, worldPosition, Quaternion.identity);
			gameObject.transform.up = Vector3.up;
			gameObject.transform.forward = transform.forward;
			if (effectRotation != null)
			{
				gameObject.transform.rotation = effectRotation.Value;
			}

			OnEffectCreated(gameObject, false);
			return gameObject;
		}


		public GameObject PlayLocalEffectPoint(GameObject resource, object param, Quaternion? effectRotation = null)
		{
			if (IsOutSight)
			{
				return null;
			}

			if (param != null)
			{
				Vector3 position;
				if (param is Vector3)
				{
					Vector3 vector = (Vector3) param;
					Vector3 b = vector;
					position = GetPosition() + b;
				}
				else
				{
					string text;
					if ((text = param as string) == null)
					{
						goto IL_81;
					}

					string text2 = text;
					Transform transform = this.transform;
					if (!string.IsNullOrEmpty(text2))
					{
						Transform transform2 = this.transform.FindRecursively(text2);
						if (transform2 != null)
						{
							transform = transform2;
						}
					}

					position = transform.position;
				}

				GameObject gameObject = Instantiate<GameObject>(resource, position, Quaternion.identity);
				gameObject.transform.up = Vector3.up;
				gameObject.transform.forward = this.transform.forward;
				if (effectRotation != null)
				{
					gameObject.transform.rotation = effectRotation.Value;
				}

				OnEffectCreated(gameObject, false);
				return gameObject;
			}

			IL_81:
			return null;
		}


		public void PlayLocalEffectChildManual(string effectKey, GameObject resource, string parentName,
			bool includeInactive = true)
		{
			if (childEffectCounter.ContainsKey(effectKey))
			{
				Dictionary<string, int> dictionary = childEffectCounter;
				int num = dictionary[effectKey];
				dictionary[effectKey] = num + 1;
				return;
			}

			Transform parent = transform;
			if (!string.IsNullOrEmpty(parentName))
			{
				parent = transform.FindRecursively(parentName, includeInactive);
			}

			GameObject instancedEffectObject = Instantiate<GameObject>(resource, parent);
			FxDestroy component = instancedEffectObject.GetComponent<FxDestroy>();
			if (component != null)
			{
				component.enabled = false;
			}

			childEffectCounter.Add(effectKey, 1);
			childEffects.Add(effectKey, instancedEffectObject);
			OnEffectCreated(instancedEffectObject, true);
		}


		public void StopLocalEffectChildManual(string effectKey, bool destroyImmediate)
		{
			if (childEffectCounter.ContainsKey(effectKey))
			{
				Dictionary<string, int> dictionary = childEffectCounter;
				int num = dictionary[effectKey];
				dictionary[effectKey] = num - 1;
				if (childEffectCounter[effectKey] == 0)
				{
					if (childEffects.ContainsKey(effectKey))
					{
						if (childEffects[effectKey] != null)
						{
							if (destroyImmediate)
							{
								Destroy(childEffects[effectKey]);
							}
							else
							{
								DestroyEffect(childEffects[effectKey]);
							}
						}

						childEffects.Remove(effectKey);
					}

					childEffectCounter.Remove(effectKey);
				}
			}
		}


		public void SetNoParentEffectManual(string effectKey)
		{
			if (!childEffectCounter.ContainsKey(effectKey))
			{
				return;
			}

			if (!childEffects.ContainsKey(effectKey))
			{
				return;
			}

			if (childEffects[effectKey] == null)
			{
				return;
			}

			childEffects[effectKey].transform.SetParent(null);
			childEffectCounter.Remove(effectKey);
			childEffects.Remove(effectKey);
		}


		public virtual void SetNoParentEffectByTag(string tag) { }


		private void DestroyEffect(GameObject effectObj)
		{
			effectObj.GetComponentsInChildren<ParticleSystem>(destroyEffectParticleSystems);
			foreach (ParticleSystem particleSystem in destroyEffectParticleSystems)
			{
				ParticleSystem.MainModule main = particleSystem.main;
				if (main.loop)
				{
					main.loop = false;
					main.stopAction = ParticleSystemStopAction.Destroy;
				}
				else
				{
					Renderer component = particleSystem.GetComponent<Renderer>();
					if (component != null)
					{
						component.enabled = false;
					}
				}
			}

			effectObj.transform.parent = null;
			Destroy(effectObj, 1f);
		}


		public virtual void StopLocalEffectByTag(string tag) { }


		public virtual void StopLocalSoundByTag(string tag) { }


		public bool HasEffectKey(string effectKey)
		{
			return childEffectCounter.ContainsKey(effectKey);
		}


		public virtual void StartSkill(SkillId skillId, SkillData skillData, int evolutionLevel, int targetObjectId)
		{
			LocalSkillPlayer localSkillPlayer = LocalSkillPlayer;
			if (localSkillPlayer == null)
			{
				return;
			}

			localSkillPlayer.Start(skillId, skillData, evolutionLevel, targetObjectId);
		}


		public virtual void StartPassiveSkill(SkillId skillId, SkillData skillData, int evolutionLevel,
			int targetObjectId)
		{
			LocalSkillPlayer localSkillPlayer = LocalSkillPlayer;
			if (localSkillPlayer == null)
			{
				return;
			}

			localSkillPlayer.StartPassiveSkill(skillId, skillData, evolutionLevel, targetObjectId);
		}


		public virtual void StartStateSkill(SkillId skillId, SkillData skillData, int evolutionLevel, int casterId)
		{
			LocalSkillPlayer localSkillPlayer = LocalSkillPlayer;
			if (localSkillPlayer == null)
			{
				return;
			}

			localSkillPlayer.StartStateSkill(skillId, skillData, evolutionLevel, casterId);
		}


		public virtual void PlaySkill(SkillId skillId, int actionNo, LocalObject target, Vector3? targetPosition)
		{
			LocalSkillPlayer localSkillPlayer = LocalSkillPlayer;
			if (localSkillPlayer == null)
			{
				return;
			}

			localSkillPlayer.Play(skillId, actionNo, target, targetPosition);
		}


		public virtual void FinishSkill(SkillId skillId, bool cancel, SkillSlotSet skillSlotSet)
		{
			LocalSkillPlayer localSkillPlayer = LocalSkillPlayer;
			if (localSkillPlayer == null)
			{
				return;
			}

			localSkillPlayer.Finish(skillId, cancel);
		}


		public virtual void FinishPassiveSkill(SkillId skillId, bool cancel)
		{
			LocalSkillPlayer localSkillPlayer = LocalSkillPlayer;
			if (localSkillPlayer == null)
			{
				return;
			}

			localSkillPlayer.FinishPassiveSkill(skillId, cancel);
		}


		public virtual void FinishStateSkill(SkillId skillId, bool cancel)
		{
			LocalSkillPlayer localSkillPlayer = LocalSkillPlayer;
			if (localSkillPlayer == null)
			{
				return;
			}

			localSkillPlayer.FinishStateSkill(skillId, cancel);
		}


		public void AddAttachedSight(LocalSightAgent sightAgent)
		{
			if (attachedSights == null)
			{
				attachedSights = new List<LocalSightAgent>();
			}

			attachedSights.Add(sightAgent);
		}


		public void RemoveAttachedSight(LocalSightAgent sightAgent)
		{
			if (attachedSights == null)
			{
				attachedSights = new List<LocalSightAgent>();
			}

			attachedSights.Remove(sightAgent);
		}


		public override void RemoveAllAttachedSight()
		{
			if (attachedSights == null)
			{
				return;
			}

			for (int i = 0; i < attachedSights.Count; i++)
			{
				if (!(attachedSights[i] == null))
				{
					SightAgent owner = attachedSights[i].GetOwner();
					if (owner != null)
					{
						owner.RemoveAttachSight(attachedSights[i]);
					}

					DestroyImmediate(attachedSights[i]);
				}
			}

			attachedSights.Clear();
		}


		public virtual AreaData GetCurrentAreaData(LevelData currentLevel)
		{
			return AreaUtil.GetCurrentAreaDataByMask(currentLevel, 2147483640, GetCurrentAreaMask());
		}


		public virtual int GetCurrentAreaMask()
		{
			return AreaUtil.GetCurrentAreaMask(GetPosition());
		}


		public void ActiveObject(string childName, bool isActive)
		{
			Transform transform = this.transform.FindRecursively(childName);
			if (transform == null)
			{
				return;
			}

			if (transform.gameObject.activeSelf != isActive)
			{
				transform.gameObject.SetActive(isActive);
			}
		}


		public virtual ObjectOrder GetObjectOrder()
		{
			return ObjectOrder.ItemBox;
		}


		public virtual bool IsMouseHitPossible(LocalSightAgent targetSightAgent, bool isInvisible)
		{
			SightAgent component = GetComponent<SightAgent>();
			return component == null || targetSightAgent.IsInAllySight(component, GetPosition(), 0f, isInvisible);
		}


		public virtual bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.None);
			return true;
		}


		public GameObject LoadCommonProjectile(string projectileName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadProjectile(projectileName);
		}


		public virtual GameObject LoadProjectile(string projectileName)
		{
			return LoadCommonProjectile(projectileName);
		}


		public GameObject LoadCommonObject(string objectName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadObject(objectName);
		}


		public virtual GameObject LoadObject(string objectName)
		{
			return LoadCommonObject(objectName);
		}


		public GameObject LoadCommonEffect(string effectName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect(effectName);
		}


		public virtual GameObject LoadEffect(string effectName)
		{
			return LoadCommonEffect(effectName);
		}


		public AudioClip LoadCommonFXSound(string soundName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadFXSound(soundName);
		}


		public virtual AudioClip LoadFXSound(string soundName)
		{
			return LoadCommonFXSound(soundName);
		}


		public AudioClip LoadCommonVoice(string voiceName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadCommonVoice(voiceName);
		}


		public virtual AudioClip LoadVoice(string characterResource, string voiceName, int randomCount = 0)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(voiceName);
		}
	}
}