using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LobbyCharacterStation : SingletonMonoBehaviour<LobbyCharacterStation>
	{
		private const int DEFAULT_CULLING_MASK = 32771;


		[SerializeField] private Transform RotationPivot = default;


		[SerializeField] private CameraControl cameraControl = default;


		private Animator[] animators = default;


		private GameObject characterModel = default;


		private Camera mainCamera = default;


		public GameObject CharacterModel => characterModel;


		public CameraControl CameraControl => cameraControl;


		private void Awake()
		{
			mainCamera = GameUtil.Bind<Camera>(gameObject, "Main Camera");
		}


		public void LoadCharacter(int characterCode, int skinIndex)
		{
			ReleaseCharacterModel();
			RotationPivot.localRotation = Quaternion.identity;
			try
			{
				characterModel =
					Instantiate<GameObject>(
						SingletonMonoBehaviour<ResourceManager>.inst.LoadLobbyCharacter(characterCode, skinIndex),
						RotationPivot);
				characterModel.GetComponentInChildren<ClientCharacter>().Init(characterCode, skinIndex);
				animators = characterModel.GetComponentsInChildren<Animator>();
				SetDestroySoundCharacterExist();
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
		}


		public void Rotate(Vector3 euler)
		{
			RotationPivot.Rotate(euler);
		}


		private void ReleaseCharacterModel()
		{
			if (characterModel != null)
			{
				Destroy(characterModel);
				characterModel = null;
			}
		}


		public void EnableAnimators(bool enable)
		{
			if (animators == null || animators.Length == 0)
			{
				return;
			}

			foreach (Animator animator in animators)
			{
				if (animator != null)
				{
					animator.speed = enable ? 1 : 0;
				}
			}
		}


		public bool IsPlayingAppearAnim()
		{
			if (animators == null || animators.Length == 0)
			{
				return false;
			}

			Animator[] array = animators;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetCurrentAnimatorStateInfo(0).loop)
				{
					return false;
				}
			}

			return true;
		}


		public void SetCameraRendering3D(bool flag)
		{
			mainCamera.cullingMask = flag ? 32771 : 0;
		}


		private void SetDestroySoundCharacterExist()
		{
			ActionFrameEvent componentInChildren = GetComponentInChildren<ActionFrameEvent>();
			if (componentInChildren != null)
			{
				componentInChildren.SetDestroySoundCharacterExist = true;
			}
		}
	}
}