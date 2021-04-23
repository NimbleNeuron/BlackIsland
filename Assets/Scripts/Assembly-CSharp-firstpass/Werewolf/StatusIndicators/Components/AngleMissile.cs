using UnityEngine;

namespace Werewolf.StatusIndicators.Components
{
	public class AngleMissile : SpellIndicator
	{
		public override ScalingType Scaling => ScalingType.LengthAndHeight;


		public override void Update()
		{
			if (Manager != null)
			{
				Vector3 vector = FlattenVector(Manager.Get3DMousePosition()) - Manager.transform.position;
				if (vector != Vector3.zero)
				{
					Manager.transform.rotation = Quaternion.LookRotation(vector);
				}
			}
		}
	}
}