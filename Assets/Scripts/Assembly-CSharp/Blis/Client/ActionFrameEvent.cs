using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blis.Common;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class ActionFrameEvent : MonoBehaviour
	{
		private const string FootStep = "FootStep";
		private readonly Dictionary<string, Transform> foundObjects = new Dictionary<string, Transform>();
		private readonly RaycastHit[] hits = new RaycastHit[10];
		private readonly List<LocalAudioSource> localAudioSources = new List<LocalAudioSource>();
		private readonly List<LocalEffectObject> localEffects = new List<LocalEffectObject>();
		private readonly Vector3 rayCastDirection = new Vector3(0f, -1f, 0f);
		private readonly Vector3 rayCastPositionAdjust = new Vector3(0f, 1f, 0f);
		private IAnimateObject animateObject;
		private Animator animator;
		private bool destroySoundCharacterExist;
		private LocalMovableCharacter movableCharacter;
		private bool movableCharacterExist;
		
		public bool SetDestroySoundCharacterExist {
			set => destroySoundCharacterExist = value;
		}

		private void Awake()
		{
			animator = GetComponent<Animator>();
			movableCharacter = GetComponentInParent<LocalMovableCharacter>();
			movableCharacterExist = movableCharacter != null;
		}

		private void OnDestroy()
		{
			if (destroySoundCharacterExist)
			{
				for (int i = 0; i < localAudioSources.Count; i++)
				{
					if (localAudioSources[i].tag.Equals("Lobby"))
					{
						localAudioSources[i].audioSource.Stop();
					}
				}

				localAudioSources.Clear();
			}
		}
		
		public event Action<GameObject, bool> onAnimationEventCreated = delegate { };


		public void Init(IAnimateObject animateObject)
		{
			this.animateObject = animateObject;
		}


		public static List<MethodInfo> GetAnimationEventMethods<T>() where T : ActionFrameEvent
		{
			List<MethodInfo> list = new List<MethodInfo>();
			MethodInfo[] methods = typeof(T).GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				if (((AnimationEventMethod[]) methods[i].GetCustomAttributes(typeof(AnimationEventMethod), true))
					.Length != 0)
				{
					list.Add(methods[i]);
				}
			}

			return list;
		}


		public bool IsEnableEvent(AnimationParam animationParam)
		{
			if (animator.layerCount == 1)
			{
				return true;
			}

			if (animationParam.syncLayerIndices != null && animationParam.syncLayerIndices.Length != 0)
			{
				for (int i = 0; i < animationParam.syncLayerIndices.Length; i++)
				{
					float layerWeight = animator.GetLayerWeight(animationParam.syncLayerIndices[i]);
					if (0f < layerWeight)
					{
						return false;
					}
				}

				return true;
			}

			return 0f < animator.GetLayerWeight(animationParam.layer);
		}


		private object GetAnimationParam(int key)
		{
			AnimationParam animationParamData = GetAnimationParamData(key);
			if (animationParamData == null)
			{
				return null;
			}

			if (IsEnableEvent(animationParamData))
			{
				return animationParamData.param;
			}

			return null;
		}


		private AnimationParam GetAnimationParamData(int key)
		{
			if (animateObject == null)
			{
				return null;
			}

			return SingletonMonoBehaviour<AnimationEventService>.inst.AnimationCollection.GetAnimationParam(key,
				gameObject);
		}


		private GameObject SpawnEffect(AttachObjectInfo attachObjectInfo)
		{
			Transform transform = FindAttachTransform(attachObjectInfo.childObjectPath);
			if (transform == null)
			{
				return null;
			}

			Object @object = animateObject.LoadEffect(attachObjectInfo.resourceName);
			if (@object)
			{
				Quaternion rotation = (@object as GameObject).transform.rotation;
				GameObject gameObject = Instantiate(@object, transform.position, transform.rotation) as GameObject;
				gameObject.transform.rotation *= rotation * Quaternion.Euler(attachObjectInfo.rotation);
				AddEffect(attachObjectInfo.tag, gameObject);
				return gameObject;
			}

			return null;
		}


		private GameObject SpawnEffectPoint(AttachPointInfo attachPointInfo)
		{
			Vector3 vector = attachPointInfo.positionOffset;
			vector = transform.rotation * vector;
			GameObject gameObject = animateObject.LoadEffect(attachPointInfo.resourceName);
			if (gameObject)
			{
				Quaternion rotation = gameObject.transform.rotation;
				GameObject gameObject2 = Instantiate<GameObject>(gameObject, animateObject.GetPosition() + vector,
					transform.rotation);
				gameObject2.transform.rotation *= rotation * Quaternion.Euler(attachPointInfo.rotation);
				AddEffect(attachPointInfo.tag, gameObject2);
				return gameObject2;
			}

			return null;
		}


		private GameObject SpawnEffectChild(AttachObjectInfo attachObjectInfo)
		{
			Transform transform = FindAttachTransform(attachObjectInfo.childObjectPath);
			if (transform == null)
			{
				return null;
			}

			GameObject gameObject = animateObject.LoadEffect(attachObjectInfo.resourceName);
			if (gameObject)
			{
				Quaternion rotation = gameObject.transform.rotation;
				GameObject gameObject2 = Instantiate<GameObject>(gameObject, transform.position, transform.rotation);
				gameObject2.transform.parent = transform;
				gameObject2.transform.rotation *= rotation * Quaternion.Euler(attachObjectInfo.rotation);
				AddEffect(attachObjectInfo.tag, gameObject2);
				return gameObject2;
			}

			return null;
		}


		[AnimationEventMethod(category = "Active", label = "대상, 활성화", type = typeof(ObjectActiveInfo),
			desc = "대상 게임 오브젝트를 활성화한다.")]
		public void ActiveModel(int key)
		{
			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				ObjectActiveInfo objectActiveInfo = (ObjectActiveInfo) animationParam;
				Transform transform = FindAttachTransform(objectActiveInfo.objectPath);
				if (transform == null)
				{
					return;
				}

				if (transform.gameObject.activeSelf != objectActiveInfo.active)
				{
					transform.gameObject.SetActive(objectActiveInfo.active);
				}
			}
		}


		[AnimationEventMethod(category = "Effect", label = "위치,회전,이펙트,태그", type = typeof(AttachObjectInfo),
			desc = "지정된 본에 이펙트를 출력한다.")]
		public void PlayEffect(int key)
		{
			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				AttachObjectInfo attachObjectInfo = (AttachObjectInfo) animationParam;
				if (!animateObject.CanPlayEvent())
				{
					return;
				}

				onAnimationEventCreated(SpawnEffect(attachObjectInfo), false);
			}
		}


		[AnimationEventMethod(category = "Effect", label = "위치,회전,이펙트,태그", type = typeof(AttachPointInfo),
			desc = "지정된 위치에 이펙트를 출력한다")]
		public void PlayEffectPoint(int key)
		{
			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				AttachPointInfo attachPointInfo = (AttachPointInfo) animationParam;
				if (!animateObject.CanPlayEvent())
				{
					return;
				}

				onAnimationEventCreated(SpawnEffectPoint(attachPointInfo), false);
			}
		}


		[AnimationEventMethod(category = "Effect", label = "위치,회전,이펙트,태그", type = typeof(AttachObjectInfo),
			desc = "지정된 본에 이펙트를 출력한다. PlayEffectChild로 지정된 이펙트는 DestroyEffect가 가능하다")]
		public void PlayEffectChild(int key)
		{
			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				AttachObjectInfo attachObjectInfo = (AttachObjectInfo) animationParam;
				if (!animateObject.CanPlayEvent())
				{
					return;
				}

				onAnimationEventCreated(SpawnEffectChild(attachObjectInfo), true);
			}
		}


		[AnimationEventMethod(category = "Effect", label = "태그", type = typeof(StringTag), desc = "태그가 같은 이펙트를 제거한다.")]
		public void StopEffectByTagKey(int key)
		{
			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				StringTag stringTag = (StringTag) animationParam;
				if (!animateObject.CanPlayEvent())
				{
					return;
				}

				StopEffectByTag(stringTag.tag);
			}
		}


		[AnimationEventMethod(category = "Effect", label = "태그", type = typeof(StringTag),
			desc = "태그가 같은 이펙트를 SetParent(null)처리 해준다.")]
		public void SetNoParentEffectByTagKey(int key)
		{
			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				StringTag stringTag = (StringTag) animationParam;
				if (!animateObject.CanPlayEvent())
				{
					return;
				}

				SetNoParentEffectByTag(stringTag.tag);
			}
		}


		[AnimationEventMethod(category = "Sound", label = "사운드명,MaxDistance,태그", type = typeof(StringPairInfo),
			desc = "사운드를 해당 믹서그룹에 출력한다.")]
		public void PlaySoundEffectInMixer(int key)
		{
			if (!animateObject.CanPlayEvent())
			{
				return;
			}

			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				StringPairInfo stringPairInfo = (StringPairInfo) animationParam;
				int maxDistance = 0;
				int.TryParse(stringPairInfo.value2, out maxDistance);
				AudioClip audioClip = animateObject.LoadFXSound(stringPairInfo.value1);
				SfxAudioSource audioSource = Singleton<SoundControl>.inst.PlayFXSound(audioClip, stringPairInfo.tag,
					maxDistance, animateObject.GetPosition(), false);
				AddSound(stringPairInfo.tag, audioSource);
			}
		}


		[AnimationEventMethod(category = "Sound", label = "캐릭터명,사운드명,Count,MaxDistance,태그",
			type = typeof(SkillVoiceInfo), desc = "음성을 각 나라 언어에 맞게 출력한다.(랜덤X)")]
		public void PlayVoiceInMixer(int key)
		{
			if (!animateObject.CanPlayEvent())
			{
				return;
			}

			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				SkillVoiceInfo skillVoiceInfo = (SkillVoiceInfo) animationParam;
				AudioClip audioClip = animateObject.LoadVoice(skillVoiceInfo.charName, skillVoiceInfo.soundName, 1);
				Singleton<SoundControl>.inst.PlayCharacterVoiceSound(animateObject.GetTransform(), audioClip,
					animateObject.GetObjectId(), skillVoiceInfo.tag, skillVoiceInfo.maxDistance,
					animateObject.GetPosition(), true, true);
			}
		}


		[AnimationEventMethod(category = "Sound", label = "캐릭터명,사운드명,Count,MaxDistance,태그",
			type = typeof(SkillVoiceInfo), desc = "스킬 사용시 기합 소리 등을 각 나라 언어에 맞게 랜덤하게 출력한다.")]
		public void PlayRandomVoiceInMixer(int key)
		{
			if (!animateObject.CanPlayEvent())
			{
				return;
			}

			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				SkillVoiceInfo skillVoiceInfo = (SkillVoiceInfo) animationParam;
				int randomCount = Random.Range(1, skillVoiceInfo.count + 1);
				AudioClip audioClip =
					animateObject.LoadVoice(skillVoiceInfo.charName, skillVoiceInfo.soundName, randomCount);
				Singleton<SoundControl>.inst.PlayCharacterVoiceSound(animateObject.GetTransform(), audioClip,
					animateObject.GetObjectId(), skillVoiceInfo.tag, skillVoiceInfo.maxDistance,
					animateObject.GetPosition(), true, true);
			}
		}


		[AnimationEventMethod(category = "Sound", label = "", type = typeof(EmptyParam), desc = "발소리를 출력한다.")]
		public void PlayFootStepSound(int key)
		{
			if (!animateObject.CanPlayEvent())
			{
				return;
			}

			if (GetAnimationParam(key) != null)
			{
				int num = Physics.RaycastNonAlloc(animateObject.GetPosition() + rayCastPositionAdjust, rayCastDirection,
					hits, 20f, GameConstants.LayerMask.GROUND_LAYER);
				if (num <= 0)
				{
					return;
				}

				RaycastHit raycastHit = default;
				float num2 = float.MaxValue;
				for (int i = 0; i < num; i++)
				{
					if (hits[i].collider is MeshCollider)
					{
						if (raycastHit.collider == null)
						{
							raycastHit = hits[i];
							num2 = hits[i].distance;
						}
						else if (num2 > hits[i].distance)
						{
							raycastHit = hits[i];
							num2 = hits[i].distance;
						}
					}
				}

				MeshCollider meshCollider = raycastHit.collider as MeshCollider;
				if (meshCollider == null)
				{
					return;
				}

				Mesh sharedMesh = meshCollider.sharedMesh;
				int num3 = raycastHit.triangleIndex * 3;
				int j;
				for (j = 0; j < sharedMesh.subMeshCount; j++)
				{
					int indexCount = sharedMesh.GetSubMesh(j).indexCount;
					if (indexCount > num3)
					{
						break;
					}

					num3 -= indexCount;
				}

				Material[] sharedMaterials = raycastHit.collider.GetComponent<MeshRenderer>().sharedMaterials;
				if (sharedMaterials.Length <= j)
				{
					Log.E(string.Format("Can't Find FootStep Data. SubMesh : {0}", j));
					return;
				}

				Material material = sharedMaterials[j];
				FootstepData footStepData = GameDB.level.GetFootStepData(material.name);
				if (footStepData == null)
				{
					Log.E("Can't Find FootStep Data. Material Name : " + material.name);
					return;
				}

				List<SoundGroupData> soundGroupData = GameDB.level.GetSoundGroupData(footStepData.groupName);
				if (soundGroupData.Count == 0)
				{
					return;
				}

				int randomElementalIndex = GameUtil.GetRandomElementalIndex((from s in soundGroupData
					select s.rate).ToList<int>());
				if (randomElementalIndex == -1)
				{
					return;
				}

				AudioClip audioClip = animateObject.LoadFXSound(soundGroupData[randomElementalIndex].fileName);
				Singleton<SoundControl>.inst.PlayFXSound(audioClip, "FootStep", footStepData.maxDistance,
					animateObject.GetPosition(), false);
			}
		}


		[AnimationEventMethod(category = "Animation", label = "키값,bool값", type = typeof(StringBool),
			desc = "에니메이터의 bool parameters의 값을 바꾼다.")]
		public void SetAnimatorBool(int key)
		{
			object animationParam = GetAnimationParam(key);
			if (animationParam != null)
			{
				StringBool stringBool = (StringBool) animationParam;
				bool flag = false;
				AnimatorControllerParameter[] parameters = animator.parameters;
				for (int i = 0; i < parameters.Length; i++)
				{
					if (parameters[i].name == stringBool.key)
					{
						flag = true;
						break;
					}
				}

				if (flag)
				{
					if (!movableCharacterExist)
					{
						animator.SetBool(stringBool.key, stringBool.value);
						return;
					}

					movableCharacter.SetAnimation(Animator.StringToHash(stringBool.key), stringBool.value);
				}
				else
				{
					Log.E("SetAnimatorBool Animator has no key : {0}", stringBool.key);
				}
			}
		}


		public void SetNoParentEffectByTag(string tag)
		{
			for (int i = localEffects.Count - 1; i >= 0; i--)
			{
				if (localEffects[i].tag == tag && localEffects[i].effect != null)
				{
					localEffects[i].effect.transform.SetParent(null);
				}
			}
		}


		public void StopEffectByTag(string tag)
		{
			this.StartThrowingCoroutine(Internal_StopEffectByTag(tag),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][StopEffectByTag] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void StopSoundByTag(string tag)
		{
			this.StartThrowingCoroutine(Internal_StopSoundByTag(tag),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][StopSoundByTag] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private void AddEffect(string tag, GameObject effectObject)
		{
			CleanUpLocalEffects();
			if (string.IsNullOrEmpty(tag))
			{
				return;
			}

			LocalEffectObject item = new LocalEffectObject
			{
				tag = tag,
				effect = effectObject
			};
			localEffects.Add(item);
		}


		private void CleanUpLocalEffects()
		{
			for (int i = localEffects.Count - 1; i >= 0; i--)
			{
				if (localEffects[i].effect == null)
				{
					localEffects.RemoveAt(i);
				}
			}
		}


		private void AddSound(string tag, SfxAudioSource audioSource)
		{
			CleanUpLocalAudioSources();
			if (string.IsNullOrEmpty(tag))
			{
				return;
			}

			if (audioSource == null)
			{
				return;
			}

			LocalAudioSource item = new LocalAudioSource
			{
				tag = tag,
				audioSource = audioSource
			};
			localAudioSources.Add(item);
		}


		private void CleanUpLocalAudioSources()
		{
			for (int i = localAudioSources.Count - 1; i >= 0; i--)
			{
				if (!localAudioSources[i].audioSource.IsPlaying)
				{
					localAudioSources.RemoveAt(i);
				}
			}
		}


		private IEnumerator Internal_StopEffectByTag(string tag)
		{
			float deltaTime = 0.25f;
			while (0f < deltaTime)
			{
				deltaTime -= Time.deltaTime;
				bool flag = false;
				for (int i = localEffects.Count - 1; i >= 0; i--)
				{
					if (localEffects[i].tag == tag)
					{
						if (localEffects[i].effect != null)
						{
							Destroy(localEffects[i].effect);
						}

						localEffects.RemoveAt(i);
						flag = true;
					}
				}

				if (flag)
				{
					yield break;
				}

				yield return null;
			}
		}


		private IEnumerator Internal_StopSoundByTag(string tag)
		{
			float deltaTime = 0.25f;
			while (0f < deltaTime)
			{
				deltaTime -= Time.deltaTime;
				bool flag = false;
				for (int i = localAudioSources.Count - 1; i >= 0; i--)
				{
					if (localAudioSources[i].tag.Equals(tag))
					{
						if (localAudioSources[i].audioSource.IsPlaying)
						{
							localAudioSources[i].audioSource.Stop();
						}

						localAudioSources.RemoveAt(i);
						flag = true;
					}
				}

				if (flag)
				{
					yield break;
				}

				yield return null;
			}
		}


		private Transform FindAttachTransform(string realPath)
		{
			Transform transform;
			foundObjects.TryGetValue(realPath, out transform);
			if (transform == null)
			{
				transform = this.transform.Find(realPath);
				foundObjects[realPath] = transform;
			}

			return transform;
		}


		public class LocalEffectObject
		{
			public GameObject effect;

			public string tag;
		}


		public class LocalAudioSource
		{
			public SfxAudioSource audioSource;

			public string tag;
		}
	}
}