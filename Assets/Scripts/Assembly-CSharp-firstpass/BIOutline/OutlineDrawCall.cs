using UnityEngine;

namespace BIOutline
{
	public class OutlineDrawCall
	{
		public readonly int meshCount;


		public readonly Renderer render;


		public OutlineDrawCall(Renderer render)
		{
			this.render = render;
			meshCount = GetMeshCount(render);
		}


		private int GetMeshCount(Renderer renderer)
		{
			MeshFilter component = renderer.GetComponent<MeshFilter>();
			if (component != null)
			{
				if (component.sharedMesh != null)
				{
					return component.sharedMesh.subMeshCount;
				}

				return 0;
			}

			SkinnedMeshRenderer component2 = renderer.GetComponent<SkinnedMeshRenderer>();
			if (!(component2 != null))
			{
				return 1;
			}

			if (component2.sharedMesh != null)
			{
				return component2.sharedMesh.subMeshCount;
			}

			return 0;
		}
	}
}