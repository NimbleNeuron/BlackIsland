using UnityEngine;

namespace UnityStandardAssets.SceneUtils
{
	public class PlaceTargetWithMouse : MonoBehaviour
	{
		public float surfaceOffset = 1.5f;


		public GameObject setTargetOn;


		private void Update()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
			{
				setTargetOn.transform.position = raycastHit.point + raycastHit.normal * surfaceOffset;
				setTargetOn.transform.forward = -raycastHit.normal;
			}
		}
	}
}