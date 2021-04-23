using UnityEngine;

namespace Blis.Client
{
	public class PolymorphController : MonoBehaviour
	{
		private const float MeterPerSecond = 3.5f;


		private static readonly int FloatMoveVelocity = Animator.StringToHash("moveVelocity");


		private static readonly int FloatMoveSpeed = Animator.StringToHash("moveSpeed");


		private Animator animator;

		private void ResetAllLayers()
		{
			for (int i = 0; i < animator.layerCount; i++)
			{
				animator.SetLayerWeight(i, 0f);
			}
		}


		public void UpdateAnimator(float moveVelocity)
		{
			Animator animator = this.animator;
			if (animator != null)
			{
				animator.SetFloat(FloatMoveVelocity, moveVelocity);
			}

			Animator animator2 = this.animator;
			if (animator2 == null)
			{
				return;
			}

			animator2.SetFloat(FloatMoveSpeed, moveVelocity / 3.5f);
		}


		public void SetAnimator(GameObject polymorphObject)
		{
			animator = polymorphObject.GetComponent<Animator>();
			if (animator == null)
			{
				animator = polymorphObject.GetComponentInChildren<Animator>();
			}

			ResetAllLayers();
		}
	}
}