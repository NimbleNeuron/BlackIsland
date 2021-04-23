using UnityEngine;

namespace Blis.Client
{
	public abstract class EnvironmentEffect : MonoBehaviour
	{
		[SerializeField] protected Animator animator;

		public abstract void PlayAnimation(string eventKey);
	}
}