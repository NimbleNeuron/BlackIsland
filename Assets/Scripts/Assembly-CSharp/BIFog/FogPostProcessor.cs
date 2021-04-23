using UnityEngine;

namespace BIFog
{
	
	public abstract class FogPostProcessor : MonoBehaviour
	{
		
		public abstract void Process(RenderTexture source, RenderTexture dest);
	}
}
