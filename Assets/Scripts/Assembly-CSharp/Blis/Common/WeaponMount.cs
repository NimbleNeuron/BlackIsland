using UnityEngine;

namespace Blis.Common
{
	public class WeaponMount : MonoBehaviour
	{
		[SerializeField] private WeaponMountType weaponMountType = default;


		private Animator animator;


		private GameObject mountObject;


		public WeaponMountType WeaponMountType => weaponMountType;


		public bool isUsed => mountObject != null;


		public Animator Animator => animator;


		public void AddMount(GameObject go, float scale)
		{
			mountObject = go;
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = new Vector3(scale, scale, scale);
			animator = go.GetComponent<Animator>();
			if (animator != null)
			{
				animator.logWarnings = false;
			}
		}


		public void SetAnimatorController(RuntimeAnimatorController controller)
		{
			if (animator != null)
			{
				animator.runtimeAnimatorController = controller;
			}
		}


		public void SetAnimatorCullingMode(AnimatorCullingMode cullingMode)
		{
			if (animator != null)
			{
				animator.cullingMode = cullingMode;
			}
		}


		public void ResetMount()
		{
			animator = null;
			if (mountObject != null)
			{
				Destroy(mountObject);
				mountObject = null;
			}
		}


		public void ResetTrigger(int id)
		{
			if (animator != null)
			{
				animator.ResetTrigger(id);
			}
		}


		public void SetTrigger(int id)
		{
			if (animator != null)
			{
				animator.SetTrigger(id);
			}
		}


		public void ResetTrigger(string trigger)
		{
			if (animator != null)
			{
				animator.ResetTrigger(trigger);
			}
		}


		public void SetTrigger(string trigger)
		{
			if (animator != null)
			{
				animator.SetTrigger(trigger);
			}
		}


		public void SetFloat(int id, float value)
		{
			if (animator != null)
			{
				animator.SetFloat(id, value);
			}
		}


		public void SetBool(int id, bool value)
		{
			if (animator != null)
			{
				animator.SetBool(id, value);
			}
		}


		public void SetInteger(int id, int value)
		{
			if (animator != null)
			{
				animator.SetInteger(id, value);
			}
		}


		public void SetAnimatorParameter(int id)
		{
			ResetTrigger(id);
			SetTrigger(id);
		}


		public void SetAnimatorParameter(int id, float value)
		{
			SetFloat(id, value);
		}


		public void SetAnimatorParameter(int id, bool value)
		{
			SetBool(id, value);
		}


		public void SetAnimatorParameter(int id, int value)
		{
			SetInteger(id, value);
		}
	}
}