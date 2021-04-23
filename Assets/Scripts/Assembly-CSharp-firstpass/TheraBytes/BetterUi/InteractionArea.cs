using System;
using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Controls/Interaction Area", 30)]
	public class InteractionArea : Graphic, ICanvasRaycastFilter, IResolutionDependency
	{
		public enum Shape
		{
			Rectangle,


			RoundedRectangle,


			Ellipse
		}


		public Shape ClickableShape;


		[SerializeField] private FloatSizeModifier cornerRadiusFallback = new FloatSizeModifier(5f, 0f, 1000f);


		[SerializeField] private FloatSizeConfigCollection cornerRadiusConfigs = new FloatSizeConfigCollection();


		public float CurrentCornerRadius => cornerRadiusConfigs.GetCurrentItem(cornerRadiusFallback).LastCalculatedSize;


		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateCornerRadius();
		}


		private void OnDrawGizmos()
		{
			if (!raycastTarget || !isActiveAndEnabled)
			{
				return;
			}

			Gizmos.color = Color.gray;
			DrawGizmos();
		}


		private void OnDrawGizmosSelected()
		{
			Gizmos.color = raycastTarget && isActiveAndEnabled ? Color.green : 0.5f * Color.green;
			DrawGizmos();
		}


		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out vector);
			Rect rect = rectTransform.rect;
			vector.x += rectTransform.pivot.x * rect.width;
			vector.y += rectTransform.pivot.y * rect.height;
			switch (ClickableShape)
			{
				case Shape.Rectangle:
					return true;
				case Shape.RoundedRectangle:
				{
					float num = Mathf.Min(CurrentCornerRadius, Mathf.Min(0.5f * rect.width, 0.5f * rect.height));
					if (vector.x >= num && vector.x <= rect.width - num ||
					    vector.y >= num && vector.y <= rect.height - num)
					{
						return true;
					}

					float num2 = 0f;
					float num3 = 0f;
					if (vector.x < num)
					{
						num2 = num - vector.x;
					}
					else if (vector.x > rect.width - num)
					{
						num2 = vector.x - (rect.width - num);
					}

					if (vector.y < num)
					{
						num3 = num - vector.y;
					}
					else if (vector.y > rect.height - num)
					{
						num3 = vector.y - (rect.height - num);
					}

					return num2 * num2 + num3 * num3 <= num * num;
				}
				case Shape.Ellipse:
				{
					float num4 = 0.5f * Mathf.Max(rect.width, rect.height);
					float num5 = rect.width / rect.height;
					float num6 = vector.x - num4;
					float num7 = vector.y * num5 - num4;
					return num6 * num6 + num7 * num7 <= num4 * num4;
				}
				default:
					throw new NotImplementedException();
			}
		}


		public void OnResolutionChanged()
		{
			UpdateCornerRadius();
		}


		public override void SetMaterialDirty() { }


		public override void SetVerticesDirty() { }


		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}


		public void UpdateCornerRadius()
		{
			cornerRadiusConfigs.GetCurrentItem(cornerRadiusFallback).CalculateSize(this);
		}


		private void DrawGizmos()
		{
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			Vector3 zeroPoint = array[0];
			Vector3 upperPoint = array[1];
			Vector3 rightPoint = array[3];
			switch (ClickableShape)
			{
				case Shape.Rectangle:
				{
					Vector3 vector = Transpose(new Vector2(0f, 0f), zeroPoint, upperPoint, rightPoint);
					Vector3 vector2 = Transpose(new Vector2(1f, 0f), zeroPoint, upperPoint, rightPoint);
					Vector3 vector3 = Transpose(new Vector2(1f, 1f), zeroPoint, upperPoint, rightPoint);
					Vector3 vector4 = Transpose(new Vector2(0f, 1f), zeroPoint, upperPoint, rightPoint);
					Gizmos.DrawLine(vector, vector2);
					Gizmos.DrawLine(vector2, vector3);
					Gizmos.DrawLine(vector3, vector4);
					Gizmos.DrawLine(vector4, vector);
					return;
				}
				case Shape.RoundedRectangle:
				{
					float width = rectTransform.rect.width;
					float height = rectTransform.rect.height;
					float num = Mathf.Min(CurrentCornerRadius, Mathf.Min(0.5f * width, 0.5f * height));
					float num2 = num / width;
					float num3 = num / height;
					Vector3 from = Transpose(new Vector2(num2, 0f), zeroPoint, upperPoint, rightPoint);
					Vector3 to = Transpose(new Vector2(1f - num2, 0f), zeroPoint, upperPoint, rightPoint);
					Gizmos.DrawLine(from, to);
					Vector3 from2 = Transpose(new Vector2(0f, num3), zeroPoint, upperPoint, rightPoint);
					Vector3 to2 = Transpose(new Vector2(0f, 1f - num3), zeroPoint, upperPoint, rightPoint);
					Gizmos.DrawLine(from2, to2);
					Vector3 from3 = Transpose(new Vector2(num2, 1f), zeroPoint, upperPoint, rightPoint);
					Vector3 to3 = Transpose(new Vector2(1f - num2, 1f), zeroPoint, upperPoint, rightPoint);
					Gizmos.DrawLine(from3, to3);
					Vector3 from4 = Transpose(new Vector2(1f, num3), zeroPoint, upperPoint, rightPoint);
					Vector3 to4 = Transpose(new Vector2(1f, 1f - num3), zeroPoint, upperPoint, rightPoint);
					Gizmos.DrawLine(from4, to4);
					float num4 = 0.31415927f;
					for (int i = 0; i < 5; i++)
					{
						float f = num4 * i;
						float f2 = num4 * (i + 1);
						Vector2 vector5 = new Vector2(num2 * Mathf.Cos(f), num3 * Mathf.Sin(f));
						Vector2 vector6 = new Vector2(num2 * Mathf.Cos(f2), num3 * Mathf.Sin(f2));
						Vector2 vector7 = new Vector2(num2, num3);
						Vector3 from5 = Transpose(vector7 - vector5, zeroPoint, upperPoint, rightPoint);
						Vector3 to5 = Transpose(vector7 - vector6, zeroPoint, upperPoint, rightPoint);
						Gizmos.DrawLine(from5, to5);
						vector7 = new Vector2(1f - num2, 1f - num3);
						Vector3 from6 = Transpose(vector7 + vector5, zeroPoint, upperPoint, rightPoint);
						to5 = Transpose(vector7 + vector6, zeroPoint, upperPoint, rightPoint);
						Gizmos.DrawLine(from6, to5);
						vector7 = new Vector2(1f - num2, num3);
						Vector3 from7 = Transpose(new Vector2(vector7.x + vector5.x, vector7.y - vector5.y), zeroPoint,
							upperPoint, rightPoint);
						to5 = Transpose(new Vector2(vector7.x + vector6.x, vector7.y - vector6.y), zeroPoint,
							upperPoint, rightPoint);
						Gizmos.DrawLine(from7, to5);
						vector7 = new Vector2(num2, 1f - num3);
						Vector3 from8 = Transpose(new Vector2(vector7.x - vector5.x, vector7.y + vector5.y), zeroPoint,
							upperPoint, rightPoint);
						to5 = Transpose(new Vector2(vector7.x - vector6.x, vector7.y + vector6.y), zeroPoint,
							upperPoint, rightPoint);
						Gizmos.DrawLine(from8, to5);
					}

					return;
				}
				case Shape.Ellipse:
				{
					float num5 = 0.31415927f;
					for (int j = 0; j < 20; j++)
					{
						float f3 = num5 * j;
						float f4 = num5 * ((j + 1) % 20);
						Vector2 relativePosition =
							new Vector2(0.5f + 0.5f * Mathf.Cos(f3), 0.5f + 0.5f * Mathf.Sin(f3));
						Vector2 relativePosition2 =
							new Vector2(0.5f + 0.5f * Mathf.Cos(f4), 0.5f + 0.5f * Mathf.Sin(f4));
						Vector3 from9 = Transpose(relativePosition, zeroPoint, upperPoint, rightPoint);
						Vector3 to6 = Transpose(relativePosition2, zeroPoint, upperPoint, rightPoint);
						Gizmos.DrawLine(from9, to6);
					}

					return;
				}
				default:
					throw new NotImplementedException();
			}
		}


		private Vector3 Transpose(Vector2 relativePosition, Vector3 zeroPoint, Vector3 upperPoint, Vector3 rightPoint)
		{
			Vector3 b = relativePosition.x * (rightPoint - zeroPoint);
			Vector3 b2 = relativePosition.y * (upperPoint - zeroPoint);
			return zeroPoint + b + b2;
		}
	}
}