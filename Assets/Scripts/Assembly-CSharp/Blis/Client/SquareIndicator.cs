using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class SquareIndicator : SpellIndicator
	{
		[SerializeField] private Transform square = default;


		[SerializeField] private Transform squareAnchor = default;

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (fixedDirection != null)
			{
				Vector3 direction = FlattenVector(fixedDirection.Value);
				squareAnchor.rotation = GameUtil.LookRotation(direction);
				return;
			}

			if (Manager != null)
			{
				Vector3 vector = Manager.Get3DMousePosition();
				Vector3 vector2 = FlattenVector(vector - transform.position);
				if (vector2 != Vector3.zero)
				{
					squareAnchor.rotation = GameUtil.LookRotation(vector2);
				}

				DrawSquareOnMouse(vector);
			}
		}


		protected override void UpdateWidth(float width)
		{
			base.UpdateWidth(width);
			square.localScale = new Vector3(width, square.localScale.y, 1f);
		}


		protected override void UpdateScale(float scale)
		{
			base.UpdateScale(scale);
			square.localScale = new Vector3(square.localScale.x, scale, 1f);
		}


		private void DrawSquareOnMouse(Vector3 vec3MousePos)
		{
			square.position = vec3MousePos;
		}
	}
}