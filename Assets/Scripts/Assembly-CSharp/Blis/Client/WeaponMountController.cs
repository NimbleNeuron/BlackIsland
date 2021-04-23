using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class WeaponMountController : MonoBehaviour
	{
		private static readonly int MoveVelocity = Animator.StringToHash("moveVelocity");


		private static readonly int MoveSpeed = Animator.StringToHash("moveSpeed");


		private static readonly int AttackSpeed = Animator.StringToHash("attackSpeed");


		private readonly Dictionary<int, float> animatorLayerSwitcher = new Dictionary<int, float>();


		private readonly float layerSwitchingSpeed = 6f;


		private readonly List<int> resetWeaponLayers = new List<int>();


		private readonly List<WeaponMount> weaponMounts = new List<WeaponMount>();


		private Animator animator;


		private int characterCode;


		private int skinIndex;


		private WeaponType weaponType;

		public void Init(int characterCode, int skinIndex, Animator animator)
		{
			if (this.characterCode != characterCode || this.skinIndex != skinIndex || this.animator != animator)
			{
				this.characterCode = characterCode;
				this.skinIndex = skinIndex;
				this.animator = animator;
				ResetAllLayers();
				weaponMounts.Clear();
				weaponMounts.AddRange(transform.GetComponentsInChildren<WeaponMount>(true));
			}
		}


		private void ResetAllLayers()
		{
			if (animator != null)
			{
				for (int i = 0; i < animator.layerCount; i++)
				{
					animator.SetLayerWeight(i, 0f);
				}
			}
		}


		public void UpdateAnimator()
		{
			Animator animator = this.animator;
			float value = animator != null ? animator.GetFloat(MoveVelocity) : 0f;
			Animator animator2 = this.animator;
			float value2 = animator2 != null ? animator2.GetFloat(MoveSpeed) : 0f;
			Animator animator3 = this.animator;
			float value3 = animator3 != null ? animator3.GetFloat(AttackSpeed) : 0f;
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				WeaponMount weaponMount = weaponMounts[i];
				weaponMount.SetFloat(MoveSpeed, value2);
				weaponMount.SetFloat(MoveVelocity, value);
				weaponMount.SetFloat(AttackSpeed, value3);
			}

			if (this.animator != null && animatorLayerSwitcher.Count > 0)
			{
				bool flag = false;
				foreach (KeyValuePair<int, float> keyValuePair in animatorLayerSwitcher)
				{
					float num = this.animator.GetLayerWeight(keyValuePair.Key);
					if (num != keyValuePair.Value)
					{
						flag = true;
						if (num < keyValuePair.Value)
						{
							num += Time.deltaTime * layerSwitchingSpeed;
							if (keyValuePair.Value < num)
							{
								num = keyValuePair.Value;
							}
						}
						else if (keyValuePair.Value < num)
						{
							num -= Time.deltaTime * layerSwitchingSpeed;
							if (num < keyValuePair.Value)
							{
								num = keyValuePair.Value;
							}
						}

						this.animator.SetLayerWeight(keyValuePair.Key, num);
					}
				}

				if (!flag)
				{
					animatorLayerSwitcher.Clear();
				}
			}
		}


		public void UpdateWeaponAnimation(WeaponType paramWeaponType)
		{
			ResetWeaponLayers(characterCode, paramWeaponType);
			List<WeaponMountData> mountDataList =
				GameDB.character.GetMountDataList(characterCode, skinIndex, paramWeaponType);
			if (mountDataList == null)
			{
				ResetAllMount();
			}
			else
			{
				List<WeaponMountData> mountDataList2 =
					GameDB.character.GetMountDataList(characterCode, skinIndex, weaponType);
				if (mountDataList2 == null)
				{
					ResetAllMount();
					for (int i = 0; i < mountDataList.Count; i++)
					{
						WeaponMountData weaponMountData = mountDataList[i];
						UpdateMount(weaponMountData, null);
					}
				}
				else
				{
					foreach (WeaponMountData weaponMountData2 in mountDataList2)
					{
						bool flag = false;
						foreach (WeaponMountData weaponMountData3 in mountDataList)
						{
							if (weaponMountData3.prefab == weaponMountData2.prefab &&
							    weaponMountData3.animationController == weaponMountData2.animationController &&
							    weaponMountData3.mountType == weaponMountData2.mountType &&
							    weaponMountData3.scale == weaponMountData2.scale)
							{
								flag = true;
								break;
							}
						}

						if (!flag)
						{
							foreach (WeaponMount weaponMount in weaponMounts)
							{
								if (weaponMount.WeaponMountType == weaponMountData2.mountType)
								{
									weaponMount.ResetMount();
									break;
								}
							}
						}
					}

					for (int j = 0; j < mountDataList.Count; j++)
					{
						WeaponMountData mountData = mountDataList[j];
						WeaponMount weaponMount2 = weaponMounts.Find(x => x.WeaponMountType == mountData.mountType);
						if (!weaponMount2.isUsed)
						{
							UpdateMount(mountData, weaponMount2);
						}
					}
				}
			}

			weaponType = paramWeaponType;
			List<WeaponAnimatorLayersData> animatorLayers =
				GameDB.character.GetAnimatorLayers(characterCode, weaponType);
			for (int k = 0; k < animatorLayers.Count; k++)
			{
				WeaponAnimatorLayersData weaponAnimatorLayersData = animatorLayers[k];
				int layerIndex = animator.GetLayerIndex(weaponAnimatorLayersData.layerName);
				if (layerIndex == -1)
				{
					Log.E("WeaponAnimatorLayers table data Invalid. layerDataCode : " + weaponAnimatorLayersData.code);
				}
				else
				{
					animatorLayerSwitcher[layerIndex] = weaponAnimatorLayersData.layerWeight;
				}
			}
		}


		private void ResetAllMount()
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].ResetMount();
			}
		}


		public void ResetWeaponLayers(int characterCode, WeaponType exceptType)
		{
			resetWeaponLayers.Clear();
			GetAnimatorWeaponLayers(characterCode, exceptType, resetWeaponLayers);
			for (int i = 0; i < resetWeaponLayers.Count; i++)
			{
				animatorLayerSwitcher[resetWeaponLayers[i]] = 0f;
			}
		}


		private void UpdateMount(WeaponMountData weaponMountData, WeaponMount mount)
		{
			if (mount == null)
			{
				mount = weaponMounts.Find(x => x.WeaponMountType == weaponMountData.mountType);
			}

			if (mount == null)
			{
				Log.E(string.Format("WeaponMount Error : {0}", weaponMountData.mountType));
				return;
			}

			GameObject go = Instantiate<GameObject>(
				SingletonMonoBehaviour<ResourceManager>.inst.LoadWeaponMount(weaponMountData.characterCode,
					weaponMountData.skinIndex, weaponMountData.prefab));
			mount.AddMount(go, weaponMountData.scale);
			if (!string.IsNullOrEmpty(weaponMountData.animationController))
			{
				RuntimeAnimatorController animatorController =
					Instantiate<RuntimeAnimatorController>(
						SingletonMonoBehaviour<ResourceManager>.inst.LoadWeaponMountAnimator(
							weaponMountData.characterCode, weaponMountData.skinIndex,
							weaponMountData.animationController));
				mount.SetAnimatorController(animatorController);
			}
		}


		private void GetAnimatorWeaponLayers(int characterCode, WeaponType exceptType, List<int> result)
		{
			if (animator != null)
			{
				CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(characterCode);
				GetAnimatorWeaponLayers(characterCode, exceptType, characterMasteryData.weapon1.GetWeaponType(),
					result);
				GetAnimatorWeaponLayers(characterCode, exceptType, characterMasteryData.weapon2.GetWeaponType(),
					result);
				GetAnimatorWeaponLayers(characterCode, exceptType, characterMasteryData.weapon3.GetWeaponType(),
					result);
				GetAnimatorWeaponLayers(characterCode, exceptType, characterMasteryData.weapon4.GetWeaponType(),
					result);
			}
		}


		private void GetAnimatorWeaponLayers(int characterCode, WeaponType exceptType, WeaponType weaponType,
			List<int> weaponLayers)
		{
			if (weaponType == WeaponType.None)
			{
				return;
			}

			if (weaponType.Equals(exceptType))
			{
				return;
			}

			List<WeaponAnimatorLayersData> animatorLayers =
				GameDB.character.GetAnimatorLayers(characterCode, weaponType);
			for (int i = 0; i < animatorLayers.Count; i++)
			{
				WeaponAnimatorLayersData weaponAnimatorLayersData = animatorLayers[i];
				weaponLayers.Add(animator.GetLayerIndex(weaponAnimatorLayersData.layerName));
			}
		}


		public void ActiveWeaponObject(WeaponMountType mountType, bool isActive)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				if (weaponMounts[i].WeaponMountType == mountType && weaponMounts[i].gameObject.activeSelf != isActive)
				{
					weaponMounts[i].gameObject.SetActive(isActive);
				}
			}
		}


		public void SetAnimation(WeaponMountType type, int id)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				if (weaponMounts[i].WeaponMountType == type)
				{
					weaponMounts[i].SetAnimatorParameter(id);
				}
			}
		}


		public void SetAnimation(WeaponMountType type, int id, float value)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				if (weaponMounts[i].WeaponMountType == type)
				{
					weaponMounts[i].SetAnimatorParameter(id, value);
				}
			}
		}


		public void SetAnimation(WeaponMountType type, int id, bool value)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				if (weaponMounts[i].WeaponMountType == type)
				{
					weaponMounts[i].SetAnimatorParameter(id, value);
				}
			}
		}


		public void SetAnimation(WeaponMountType type, int id, int value)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				if (weaponMounts[i].WeaponMountType == type)
				{
					weaponMounts[i].SetAnimatorParameter(id, value);
				}
			}
		}


		public void SetAnimation(int id)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].SetAnimatorParameter(id);
			}
		}


		public void SetAnimation(int id, float value)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].SetAnimatorParameter(id, value);
			}
		}


		public void SetAnimation(int id, bool value)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].SetAnimatorParameter(id, value);
			}
		}


		public void SetAnimation(int id, int value)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].SetAnimatorParameter(id, value);
			}
		}


		public void StopAnimation()
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].ResetTrigger(LocalMovableCharacter.TriggerCanNotControl);
				weaponMounts[i].SetTrigger(LocalMovableCharacter.TriggerCanNotControl);
			}
		}


		public void ResetTrigger(int id)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].ResetTrigger(id);
			}
		}


		public void SetWeaponAnimatorTrigger(string trigger, bool reset = true)
		{
			if (weaponMounts == null || weaponMounts.Count == 0)
			{
				return;
			}

			if (reset)
			{
				ResetTrigger(GameConstants.AnimationKey.ANIMATION_CANCEL_TRIGGER_KEY);
			}

			SetTrigger(trigger);
		}


		public void SetTrigger(string trigger)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].SetTrigger(trigger);
			}
		}


		public void ResetTrigger(string trigger)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].ResetTrigger(trigger);
			}
		}


		public Transform GetTransform(WeaponMountType weaponMountType)
		{
			WeaponMount weaponMount = weaponMounts.Find(x => x.WeaponMountType == weaponMountType);
			if (weaponMount == null)
			{
				Log.E(string.Format("GetTransform WeaponMount Error : {0}", weaponMountType));
				return null;
			}

			return weaponMount.transform;
		}


		public void SetAnimatorCullingMode(AnimatorCullingMode cullingMode)
		{
			for (int i = 0; i < weaponMounts.Count; i++)
			{
				weaponMounts[i].SetAnimatorCullingMode(cullingMode);
			}
		}
	}
}