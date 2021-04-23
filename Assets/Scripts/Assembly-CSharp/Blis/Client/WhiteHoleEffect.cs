using UnityEngine;

namespace Blis.Client
{
	public class WhiteHoleEffect : EnvironmentEffect
	{
		public const string DOOR_WORK = "tHyperLoopDoor_Work";


		public const string CANCEL = "Cancel";

		public override void PlayAnimation(string eventKey)
		{
			Animator animator = this.animator;
			if (animator != null)
			{
				animator.ResetTrigger(eventKey);
			}

			Animator animator2 = this.animator;
			if (animator2 == null)
			{
				return;
			}

			animator2.SetTrigger(eventKey);
		}
	}
}