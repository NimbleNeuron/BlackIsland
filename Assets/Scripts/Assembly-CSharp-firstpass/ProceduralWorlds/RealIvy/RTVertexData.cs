using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public struct RTVertexData
	{
		public Vector3 vertex;


		public Vector3 normal;


		public Vector2 uv;


		public Vector2 uv2;


		public Color color;


		public RTVertexData(Vector3 vertex, Vector3 normal, Vector2 uv, Vector2 uv2, Color color)
		{
			this.vertex = vertex;
			this.normal = normal;
			this.uv = uv;
			this.uv2 = uv2;
			this.color = color;
		}
	}
}