using UnityEngine;

public class Rotator : MonoBehaviour
{
	public float x;


	public float y;


	public float z;


	private void OnEnable()
	{
		InvokeRepeating("Rotate", 0f, 0.0167f);
	}


	private void OnDisable()
	{
		CancelInvoke();
	}


	private void Rotate()
	{
		transform.localEulerAngles += new Vector3(x, y, z);
	}
}