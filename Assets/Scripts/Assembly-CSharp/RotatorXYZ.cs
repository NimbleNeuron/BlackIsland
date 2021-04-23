using UnityEngine;


public class RotatorXYZ : MonoBehaviour
{
	
	private void Update()
	{
		base.transform.Rotate(new Vector3(this.rotateSpeedX * Time.deltaTime, this.rotateSpeedY * Time.deltaTime, this.rotateSpeedZ * Time.deltaTime), this.spaceType);
	}

	
	public float rotateSpeedX;

	
	public float rotateSpeedY;

	
	public float rotateSpeedZ;

	
	[SerializeField]
	private Space spaceType = Space.Self;
}
