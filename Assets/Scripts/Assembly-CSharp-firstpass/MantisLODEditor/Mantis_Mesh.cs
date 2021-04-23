using UnityEngine;

namespace MantisLODEditor
{
	public class Mantis_Mesh
	{
		public int index;


		public Mesh mesh;


		public int[][] origin_triangles;


		public int out_count;


		public int[] out_triangles;


		public string uuid;


		public Mantis_Mesh()
		{
			index = -1;
			uuid = null;
			out_count = 0;
		}
	}
}