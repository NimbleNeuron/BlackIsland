using UnityEngine;

namespace UltraReal.MobaMovement
{
	
	public class MobaPcDirectInput : MobaInput
	{
		
		
		
		public MobaPcDirectInput.MouseButton MoveButton
		{
			get
			{
				return this._moveMouseButton;
			}
			set
			{
				this._moveMouseButton = value;
			}
		}

		
		public override bool GetMobaMoveButton()
		{
			return Input.GetMouseButton((int)this._moveMouseButton);
		}

		
		public override bool GetMobaMoveButtonDown()
		{
			return Input.GetMouseButtonDown((int)this._moveMouseButton);
		}

		
		public override Vector3 GetMousePosition()
		{
			return Input.mousePosition;
		}

		
		[SerializeField]
		private MobaPcDirectInput.MouseButton _moveMouseButton = MobaPcDirectInput.MouseButton.Right;

		
		public enum MouseButton
		{
			
			Left,
			
			Right,
			
			Middle
		}
	}
}
