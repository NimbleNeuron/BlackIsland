using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public Transform target;


	public Vector3 offset;


	public float currentZoom = 2f;


	public float pitch = 1.75f;


	public float zoomSpeed = 0.85f;


	public float minZoom = 1.5f;


	public float maxZoom = 3f;


	public float yawSpeed = 100f;


	private readonly float currentYaw = default;


	private void LateUpdate()
	{
		transform.position = target.position - offset * currentZoom;
		transform.LookAt(target.position + Vector3.up * pitch);
		transform.RotateAround(target.position, Vector3.up, currentYaw);
	}


	public void SetZoom(float zoom)
	{
		currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
	}
}