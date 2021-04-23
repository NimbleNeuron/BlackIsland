using UnityEngine;


public class YT_RotateCamera : MonoBehaviour
{
	
	private void Update()
	{
		this.yaw += this.speedH * Input.GetAxis("Mouse X");
		this.pitch -= this.speedV * Input.GetAxis("Mouse Y");
		base.transform.eulerAngles = new Vector3(this.pitch, this.yaw, 0f);
	}

	
	public float speedH = 2f;

	
	public float speedV = 2f;

	
	private float yaw;

	
	private float pitch;
}
