using UnityEngine;

namespace Blis.Client
{
	public class RotationRebuilder : MonoBehaviour
	{
		[ContextMenu("CopyRotation")]
		public void Rebuild()
		{
			Vector3 position = transform.position;
			transform.parent.rotation = transform.parent.transform.rotation * transform.localRotation;
			transform.localRotation = Quaternion.identity;
			transform.position = position;
		}
	}
}