using UnityEngine;

namespace AraSamples
{
	public class ObjectDragger : MonoBehaviour
	{
		private Vector3 offset;


		private Vector3 screenPoint;


		private void OnMouseDown()
		{
			screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			offset = gameObject.transform.position -
			         Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
				         screenPoint.z));
		}


		private void OnMouseDrag()
		{
			Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			transform.position = Camera.main.ScreenToWorldPoint(position) + offset;
		}
	}
}