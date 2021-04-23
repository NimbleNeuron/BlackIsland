using UnityEngine;

namespace BIFog
{
	
	public interface ISightEventHandler
	{
		
		
		Transform transform { get; }

		
		void OnSight();

		
		void OnHide();
	}
}