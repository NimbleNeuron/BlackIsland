using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class IvyInfo : MonoBehaviour
	{
		public IvyPreset originalPreset;


		public InfoPool infoPool;


		public MeshFilter meshFilter;


		public MeshRenderer meshRenderer;


		public void Setup(InfoPool infoPool, MeshFilter meshFilter, MeshRenderer meshRenderer, IvyPreset originalPreset)
		{
			this.infoPool = infoPool;
			this.meshFilter = meshFilter;
			this.meshRenderer = meshRenderer;
			this.originalPreset = originalPreset;
		}
	}
}