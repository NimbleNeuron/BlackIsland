using System.Collections.Generic;
using UnityEngine;

namespace MantisLOD
{
	internal class Class7 : IEqualityComparer<Vector3>
	{
		public bool Equals(Vector3 vec1, Vector3 vec2)
		{
			return vec1.x + 1E-07f >= vec2.x && vec1.x <= vec2.x + 1E-07f && vec1.y + 1E-07f >= vec2.y &&
			       vec1.y <= vec2.y + 1E-07f && vec1.z + 1E-07f >= vec2.z && vec1.z <= vec2.z + 1E-07f;
		}


		public int GetHashCode(Vector3 vector3_0)
		{
			return vector3_0.x.GetHashCode() ^ vector3_0.y.GetHashCode() ^ vector3_0.z.GetHashCode();
		}
	}
}