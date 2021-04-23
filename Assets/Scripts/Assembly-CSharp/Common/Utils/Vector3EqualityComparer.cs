using UnityEngine;

namespace Common.Utils
{
	
	public class Vector3EqualityComparer : SingletonComparerStruct<Vector3EqualityComparer, Vector3>
	{
		
		public override bool Equals(Vector3 x, Vector3 y)
		{
			return Mathf.Approximately(x.x, y.x) && Mathf.Approximately(x.y, y.y) && Mathf.Approximately(x.z, y.z);
		}

		
		public override int GetHashCode(Vector3 obj)
		{
			return obj.GetHashCode();
		}
	}
}
