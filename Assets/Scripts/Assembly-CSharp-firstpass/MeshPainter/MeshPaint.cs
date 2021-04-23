using System;
using UnityEngine;

namespace MeshPainter
{
	[RequireComponent(typeof(MeshRenderer))]
	public class MeshPaint : MonoBehaviour
	{
		[Serializable]
		public class Detail
		{
			[SerializeField] private Texture2D _texture;


			[SerializeField] private Texture2D _normal;


			[SerializeField] private Vector2 _tiling;


			
			public Texture2D Texture {
				get => _texture;
				set => _texture = value;
			}


			
			public Texture2D Normal {
				get => _normal;
				set => _normal = value;
			}


			
			public Vector2 Tiling {
				get => _tiling;
				set => _tiling = value;
			}
		}
	}
}