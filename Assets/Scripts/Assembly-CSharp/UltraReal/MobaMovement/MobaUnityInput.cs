using UnityEngine;

namespace UltraReal.MobaMovement
{
	
	public class MobaUnityInput : MobaInput
	{
		
		public override bool GetMobaMoveButton()
		{
			return Input.GetButton(this._mobaMoveButton);
		}

		
		public override bool GetMobaMoveButtonDown()
		{
			return Input.GetButtonDown(this._mobaMoveButton);
		}

		
		public override Vector3 GetMousePosition()
		{
			return Input.mousePosition;
		}

		
		[SerializeField]
		private string _mobaMoveButton = "Fire1";
	}
}
