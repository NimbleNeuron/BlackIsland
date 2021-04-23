using UnityEngine;

public class FollowMouse : MonoBehaviour
{
	private void Update()
	{
		transform.position = Input.mousePosition;
	}


	private void OnEnable()
	{
		Cursor.visible = false;
	}


	private void OnDisable()
	{
		Cursor.visible = true;
	}
}