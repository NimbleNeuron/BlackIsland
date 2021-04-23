using Ara;
using UnityEngine;

namespace AraSamples
{
	[RequireComponent(typeof(AraTrail))]
	public class WallPlayerController : MonoBehaviour
	{
		public float speed = 10f;


		public int boardSize = 5;


		public int maxTrailLenght = 10;


		public Color[] colors = new Color[4];


		private int coordX;


		private int coordZ;


		private AraTrail trail;


		private void Awake()
		{
			trail = GetComponent<AraTrail>();
		}


		private void Update()
		{
			float num = Time.deltaTime * speed;
			Vector3 position = transform.position;
			Vector3 vector = new Vector3(coordX, position.y, coordZ);
			transform.position = Vector3.MoveTowards(position, vector, num);
			if (trail.points.Count == 0)
			{
				trail.initialColor = colors[0];
				trail.EmitPoint(position);
			}

			if (Vector3.Distance(position, vector) < num)
			{
				transform.position = vector;
				if (Input.GetKey(KeyCode.W))
				{
					trail.initialColor = colors[0];
					trail.EmitPoint(position);
					coordX++;
				}
				else if (Input.GetKey(KeyCode.S))
				{
					trail.initialColor = colors[1];
					trail.EmitPoint(position);
					coordX--;
				}
				else if (Input.GetKey(KeyCode.A))
				{
					trail.initialColor = colors[2];
					trail.EmitPoint(position);
					coordZ++;
				}
				else if (Input.GetKey(KeyCode.D))
				{
					trail.initialColor = colors[3];
					trail.EmitPoint(position);
					coordZ--;
				}

				coordX = Mathf.Clamp(coordX, -boardSize, boardSize);
				coordZ = Mathf.Clamp(coordZ, -boardSize, boardSize);
			}

			int num2 = Mathf.Max(0, trail.points.Count - maxTrailLenght);
			if (num2 > 0)
			{
				trail.points.RemoveRange(0, num2);
			}
		}
	}
}