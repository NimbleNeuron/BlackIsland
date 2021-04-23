using System.Collections.Generic;
using UnityEngine;

namespace MantisLOD
{
	public static class MantisLODSimpler
	{
		private static readonly List<Class8> list_0 = new List<Class8>();


		public static int get_triangle_list(int index, float goal, int[] triangle_array, ref int triangle_count)
		{
			if (index >= 0 && index < list_0.Count && list_0[index] != null)
			{
				Class8 @class = list_0[index];
				int int_ = (int) (@class.method_0() * (1f - goal * 0.01f) + 0.5f);
				@class.method_2(int_, triangle_array, ref triangle_count);
				return 1;
			}

			return 0;
		}


		public static int create_progressive_mesh(Vector3[] vertex_array, int vertex_count, int[] triangle_array,
			int triangle_count, Vector3[] normal_array, int normal_count, Color[] color_array, int color_count,
			Vector2[] uv_array, int uv_count, int protect_boundary, int protect_detail, int protect_symmetry,
			int protect_normal, int protect_shape)
		{
			bool flag = false;
			int num = -1;
			for (int i = 0; i < list_0.Count; i++)
			{
				if (list_0[i] == null)
				{
					flag = true;
					list_0[i] = new Class8();
					num = i;
					break;
				}
			}

			if (!flag)
			{
				list_0.Add(new Class8());
				num = list_0.Count - 1;
			}

			list_0[num].method_1(vertex_array, vertex_count, triangle_array, triangle_count, normal_array, normal_count,
				color_array, color_count, uv_array, uv_count, protect_boundary, protect_detail, protect_symmetry,
				protect_normal, protect_shape);
			return num;
		}


		public static int delete_progressive_mesh(int index)
		{
			if (index >= 0 && index < list_0.Count && list_0[index] != null)
			{
				list_0[index] = null;
				return 1;
			}

			return 0;
		}
	}
}