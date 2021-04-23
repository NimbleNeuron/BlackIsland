using UnityEngine;

namespace BIFog
{
	
	public struct ViewCastInfo
	{
		
		public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle, Vector3 _reverseNormal)
		{
			hit = _hit;
			point = _point;
			dst = _dst;
			angle = _angle;
			reverseNormal = _reverseNormal;
		}

		
		public bool hit;

		
		public Vector3 point;

		
		public float dst;

		
		public float angle;

		
		public Vector3 reverseNormal;
	}
}