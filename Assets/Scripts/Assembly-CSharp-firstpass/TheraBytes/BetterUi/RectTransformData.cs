using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class RectTransformData : IScreenConfigConnection
	{
		public Vector3 LocalPosition;


		public Vector2 AnchoredPosition;


		public Vector2 SizeDelta;


		public Vector2 AnchorMin;


		public Vector2 AnchorMax;


		public Vector2 Pivot;


		public Vector3 Scale;


		[FormerlySerializedAs("Rotation")] [SerializeField]
		private Quaternion rotation;


		public Vector3 EulerAngles;


		[SerializeField] private bool saveRotationAsEuler;


		[SerializeField] private string screenConfigName;


		public RectTransformData() { }


		public RectTransformData(RectTransform rectTransform)
		{
			PullFromTransform(rectTransform);
		}


		
		public Quaternion Rotation {
			get
			{
				if (!saveRotationAsEuler)
				{
					return rotation;
				}

				return Quaternion.Euler(EulerAngles);
			}
			set
			{
				rotation = value;
				if (saveRotationAsEuler)
				{
					EulerAngles = rotation.eulerAngles;
				}
			}
		}


		
		public bool SaveRotationAsEuler {
			get => saveRotationAsEuler;
			set
			{
				if (saveRotationAsEuler == value)
				{
					return;
				}

				saveRotationAsEuler = value;
			}
		}


		
		public Vector2 OffsetMax {
			get => AnchoredPosition + Vector2.Scale(SizeDelta, Vector2.one - Pivot);
			set
			{
				Vector2 vector = value - (AnchoredPosition + Vector2.Scale(SizeDelta, Vector2.one - Pivot));
				SizeDelta += vector;
				AnchoredPosition += Vector2.Scale(vector, Pivot);
			}
		}


		
		public Vector2 OffsetMin {
			get => AnchoredPosition - Vector2.Scale(SizeDelta, Pivot);
			set
			{
				Vector2 vector = value - (AnchoredPosition - Vector2.Scale(SizeDelta, Pivot));
				SizeDelta -= vector;
				AnchoredPosition += Vector2.Scale(vector, Vector2.one - Pivot);
			}
		}


		
		public string ScreenConfigName {
			get => screenConfigName;
			set => screenConfigName = value;
		}


		public void PullFromTransform(RectTransform transform)
		{
			LocalPosition = transform.localPosition;
			AnchorMin = transform.anchorMin;
			AnchorMax = transform.anchorMax;
			Pivot = transform.pivot;
			AnchoredPosition = transform.anchoredPosition;
			SizeDelta = transform.sizeDelta;
			Scale = transform.localScale;
			Rotation = transform.localRotation;
			EulerAngles = transform.localEulerAngles;
		}


		public void PushToTransform(RectTransform transform)
		{
			transform.localPosition = LocalPosition;
			transform.anchorMin = AnchorMin;
			transform.anchorMax = AnchorMax;
			transform.pivot = Pivot;
			transform.anchoredPosition = AnchoredPosition;
			transform.sizeDelta = SizeDelta;
			transform.localScale = Scale;
			if (SaveRotationAsEuler)
			{
				transform.eulerAngles = EulerAngles;
				return;
			}

			transform.localRotation = Rotation;
		}


		public static RectTransformData Lerp(RectTransformData a, RectTransformData b, float amount)
		{
			return Lerp(a, b, amount, a.SaveRotationAsEuler || b.SaveRotationAsEuler);
		}


		public static RectTransformData Lerp(RectTransformData a, RectTransformData b, float amount, bool eulerRotation)
		{
			return new RectTransformData
			{
				AnchoredPosition = Vector2.Lerp(a.AnchoredPosition, b.AnchoredPosition, amount),
				AnchorMax = Vector2.Lerp(a.AnchorMax, b.AnchorMax, amount),
				AnchorMin = Vector2.Lerp(a.AnchorMin, b.AnchorMin, amount),
				LocalPosition = Vector3.Lerp(a.LocalPosition, b.LocalPosition, amount),
				Pivot = Vector2.Lerp(a.Pivot, b.Pivot, amount),
				Scale = Vector3.Lerp(a.Scale, b.Scale, amount),
				SizeDelta = Vector2.Lerp(a.SizeDelta, b.SizeDelta, amount),
				Rotation = Quaternion.Lerp(a.Rotation, b.Rotation, amount),
				EulerAngles = Vector3.Lerp(a.EulerAngles, b.EulerAngles, amount),
				SaveRotationAsEuler = eulerRotation
			};
		}


		public static RectTransformData LerpUnclamped(RectTransformData a, RectTransformData b, float amount)
		{
			return LerpUnclamped(a, b, amount, a.SaveRotationAsEuler || b.SaveRotationAsEuler);
		}


		public static RectTransformData LerpUnclamped(RectTransformData a, RectTransformData b, float amount,
			bool eulerRotation)
		{
			return new RectTransformData
			{
				AnchoredPosition = Vector2.LerpUnclamped(a.AnchoredPosition, b.AnchoredPosition, amount),
				AnchorMax = Vector2.LerpUnclamped(a.AnchorMax, b.AnchorMax, amount),
				AnchorMin = Vector2.LerpUnclamped(a.AnchorMin, b.AnchorMin, amount),
				LocalPosition = Vector3.LerpUnclamped(a.LocalPosition, b.LocalPosition, amount),
				Pivot = Vector2.LerpUnclamped(a.Pivot, b.Pivot, amount),
				Scale = Vector3.LerpUnclamped(a.Scale, b.Scale, amount),
				SizeDelta = Vector2.LerpUnclamped(a.SizeDelta, b.SizeDelta, amount),
				Rotation = Quaternion.LerpUnclamped(a.Rotation, b.Rotation, amount),
				EulerAngles = Vector3.LerpUnclamped(a.EulerAngles, b.EulerAngles, amount),
				SaveRotationAsEuler = eulerRotation
			};
		}


		public override string ToString()
		{
			return string.Format("RectTransformData: sizeDelta {{{0}, {1}}} - anchoredPosition {{{2}, {3}}}",
				SizeDelta.x, SizeDelta.y, AnchoredPosition.x, AnchoredPosition.y);
		}
	}
}