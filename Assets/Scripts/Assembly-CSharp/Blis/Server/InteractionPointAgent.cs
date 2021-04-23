using UnityEngine;

namespace Blis.Server
{
	
	public class InteractionPointAgent : MonoBehaviour
	{
		
		public void SetInteractionOffset(Vector3 offset)
		{
			this.interactionOffset = offset;
		}

		
		public Vector3 GetInteractionPoint()
		{
			return base.transform.position + base.transform.TransformVector(this.interactionOffset);
		}

		
		[SerializeField]
		private Vector3 interactionOffset;
	}
}
