using UnityEngine;

namespace BIOutline
{
	public class OutlineMaterial
	{
		private static readonly string linerShaderName = "Hidden/OutlineMetaShader";


		private static Shader linerShader;


		public readonly Color color;


		public readonly Material mat;


		public OutlineMaterial(Color color)
		{
			this.color = color;
			if (linerShader == null)
			{
				linerShader = Shader.Find(linerShaderName);
			}

			mat = new Material(linerShader);
			mat.SetColor("_Color", color);
		}
	}
}