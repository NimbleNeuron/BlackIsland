using System;
using UnityEngine;

namespace MLSpace
{
	public class CameraControl : MonoBehaviour
	{
		[Tooltip("Speed of camera movement.")] public float speed = 2f;


		[Tooltip("Speed of camera rotation.")] public float angularSpeed = 100f;


		private float m_totalXAngleDeg;


		private float m_totalYAngleDeg;


		private void Start()
		{
			Vector3 eulerAngles = transform.rotation.eulerAngles;
			m_totalXAngleDeg = eulerAngles.x;
			m_totalYAngleDeg = eulerAngles.y;
			transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
		}


		private void FixedUpdate()
		{
			if (!Input.GetMouseButton(0))
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				return;
			}

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			bool key = Input.GetKey(KeyCode.E);
			bool key2 = Input.GetKey(KeyCode.Q);
			float axisRaw = Input.GetAxisRaw("Mouse X");
			float num = -Input.GetAxisRaw("Mouse Y");
			float d = axis2 * speed * Time.deltaTime;
			float d2 = axis * speed * Time.deltaTime;
			float d3 = Convert.ToSingle(key) * speed * Time.deltaTime;
			float d4 = Convert.ToSingle(key2) * speed * Time.deltaTime;
			transform.position += transform.forward * d;
			transform.position += transform.right * d2;
			transform.position += transform.up * d3;
			transform.position -= transform.up * d4;
			float num2 = axisRaw * Time.deltaTime * angularSpeed;
			float num3 = num * Time.deltaTime * angularSpeed;
			m_totalXAngleDeg += num3;
			m_totalYAngleDeg += num2;
			Quaternion rotation = Quaternion.Euler(m_totalXAngleDeg, m_totalYAngleDeg, 0f);
			transform.rotation = rotation;
		}
	}
}