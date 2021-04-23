using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Services;

namespace Werewolf.StatusIndicators.Components
{
	public class Cone : SpellIndicator
	{
		public const float CONE_ANIM_SPEED = 0.15f;


		public Projector LBorder;


		public Projector RBorder;


		[SerializeField] [Range(0f, 360f)] private float angle;


		public override ScalingType Scaling => ScalingType.LengthAndHeight;


		
		public float Angle {
			get => angle;
			set
			{
				angle = value;
				SetAngle(value);
			}
		}


		public override void Update()
		{
			if (Manager != null)
			{
				Manager.transform.rotation =
					Quaternion.LookRotation(FlattenVector(Manager.Get3DMousePosition()) - Manager.transform.position);
			}
		}


		public override void OnValueChanged()
		{
			base.OnValueChanged();
			SetAngle(angle);
		}


		public override void OnShow()
		{
			base.OnShow();
			StartCoroutine(FadeIn());
		}


		private void SetAngle(float angle)
		{
			SetShaderFloat("_Expand", Normalize.GetValue(angle - 1f, 360f));
			LBorder.transform.localEulerAngles = new Vector3(0f, 0f, (angle + 2f) / 2f);
			RBorder.transform.localEulerAngles = new Vector3(0f, 0f, (-angle + 2f) / 2f);
		}


		private IEnumerator FadeIn()
		{
			float final = angle;
			float current = 0f;
			using (List<Projector>.Enumerator enumerator = Projectors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Projector projector = enumerator.Current;
					projector.enabled = true;
				}

				goto IL_B0;
			}

			IL_74:
			SetAngle(current);
			current += final * 0.15f;
			yield return null;
			IL_B0:
			if (current >= final)
			{
				SetAngle(final);
				yield return null;
				yield break;
			}

			goto IL_74;
		}
	}
}