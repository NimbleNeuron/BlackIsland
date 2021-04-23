using UnityEngine;

public class Nl_DragObject : MonoBehaviour
{
	public static Camera Cam;


	private Vector3 dir;


	private float distanceZ;


	private bool isTaken;


	private Vector3 offset;


	private Rigidbody rigidboy;


	private void Start()
	{
		rigidboy = gameObject.GetComponent<Rigidbody>();
	}


	private void Update()
	{
		if (isTaken)
		{
			if (Input.GetMouseButton(1))
			{
				Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ);
				Vector3 a = Cam.ScreenToWorldPoint(position);
				rigidboy.MovePosition(a + offset);
			}
			else
			{
				rigidboy.useGravity = true;
				rigidboy.constraints = RigidbodyConstraints.None;
				isTaken = false;
			}

			if (Input.GetAxis("Horizontal") != 0f && Input.GetKey(KeyCode.LeftAlt))
			{
				transform.Rotate(Vector3.up * 100f * Time.deltaTime * Input.GetAxis("Horizontal"));
			}

			if (Input.GetAxis("Vertical") != 0f && Input.GetKey(KeyCode.LeftAlt))
			{
				transform.Rotate(Vector3.right * 100f * Time.deltaTime * Input.GetAxis("Vertical"));
			}
		}
	}


	private void OnMouseOver()
	{
		if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(1))
		{
			isTaken = true;
			distanceZ = Vector3.Distance(Cam.transform.position, gameObject.transform.position);
			Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ);
			Vector3 b = Cam.ScreenToWorldPoint(position);
			offset = rigidboy.position - b;
			rigidboy.velocity = Vector3.zero;
			rigidboy.useGravity = false;
			rigidboy.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}
}