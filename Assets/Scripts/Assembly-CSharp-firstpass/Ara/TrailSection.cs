using System.Collections.Generic;
using UnityEngine;

namespace Ara
{
	[CreateAssetMenu(menuName = "Ara Trails/Trail Section")]
	public class TrailSection : ScriptableObject
	{
		[HideInInspector] public List<Vector2> vertices;


		public int snapX;


		public int snapY;


		public int Segments => vertices.Count - 1;


		public void OnEnable()
		{
			if (vertices == null)
			{
				vertices = new List<Vector2>();
				CirclePreset(8);
			}
		}


		public void CirclePreset(int segments)
		{
			vertices.Clear();
			for (int i = 0; i <= segments; i++)
			{
				float f = 6.2831855f / segments * i;
				vertices.Add(Mathf.Cos(f) * Vector2.right + Mathf.Sin(f) * Vector2.up);
			}
		}


		public static int SnapTo(float val, int snapInterval, int threshold)
		{
			int num = (int) val;
			if (snapInterval <= 0)
			{
				return num;
			}

			int num2 = Mathf.FloorToInt(val / snapInterval) * snapInterval;
			int num3 = num2 + snapInterval;
			if (num - num2 < threshold)
			{
				return num2;
			}

			if (num3 - num < threshold)
			{
				return num3;
			}

			return num;
		}
	}
}