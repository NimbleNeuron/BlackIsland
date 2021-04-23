using UnityEngine;

namespace Blis.Common
{
	public class WaitForTrigger : CustomYieldInstruction
	{
		private bool triggered;


		public override bool keepWaiting => !triggered;


		public void ActiveTrigger()
		{
			triggered = true;
		}
	}
}