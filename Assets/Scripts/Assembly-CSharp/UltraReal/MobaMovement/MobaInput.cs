using UnityEngine;

namespace UltraReal.MobaMovement
{
	
	public abstract class MobaInput : MonoBehaviour
	{
		
		public abstract bool GetMobaMoveButton();

		
		public abstract bool GetMobaMoveButtonDown();

		
		public abstract Vector3 GetMousePosition();
	}
}
