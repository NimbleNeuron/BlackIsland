using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Controls/Better Image", 30)]
	public class BetterImage : Image, IResolutionDependency, IImageAppearanceProvider
	{
		private static readonly Vector2[] vertScratch = new Vector2[4];


		private static readonly Vector2[] uvScratch = new Vector2[4];


		[SerializeField] private ColorMode colorMode;


		[SerializeField] private Color secondColor = Color.white;


		[SerializeField] private VertexMaterialData materialProperties = new VertexMaterialData();


		[SerializeField] private string materialType;


		[SerializeField] private MaterialEffect materialEffect;


		[SerializeField] private float materialProperty1;


		[SerializeField] private float materialProperty2;


		[SerializeField] private float materialProperty3;


		[SerializeField] private bool keepBorderAspectRatio;


		[FormerlySerializedAs("spriteBorderScale")] [SerializeField]
		private Vector2SizeModifier spriteBorderScaleFallback =
			new Vector2SizeModifier(Vector2.one, Vector2.zero, 3f * Vector2.one);


		[SerializeField] private Vector2SizeConfigCollection customBorderScales = new Vector2SizeConfigCollection();


		private Animator animator;


		
		public bool KeepBorderAspectRatio {
			get => keepBorderAspectRatio;
			set
			{
				keepBorderAspectRatio = value;
				SetVerticesDirty();
			}
		}


		public Vector2SizeModifier SpriteBorderScale => customBorderScales.GetCurrentItem(spriteBorderScaleFallback);


		protected override void OnEnable()
		{
			base.OnEnable();
			animator = GetComponent<Animator>();
			if (MaterialProperties.FloatProperties != null)
			{
				if (MaterialProperties.FloatProperties.Length != 0)
				{
					materialProperty1 = MaterialProperties.FloatProperties[0].Value;
				}

				if (MaterialProperties.FloatProperties.Length > 1)
				{
					materialProperty2 = MaterialProperties.FloatProperties[1].Value;
				}

				if (MaterialProperties.FloatProperties.Length > 2)
				{
					materialProperty3 = MaterialProperties.FloatProperties[2].Value;
				}
			}
		}


		
		public string MaterialType {
			get => materialType;
			set => ImageAppearanceProviderHelper.SetMaterialType(value, this, materialProperties, ref materialEffect,
				ref materialType);
		}


		
		public MaterialEffect MaterialEffect {
			get => materialEffect;
			set => ImageAppearanceProviderHelper.SetMaterialEffect(value, this, materialProperties, ref materialEffect,
				ref materialType);
		}


		public VertexMaterialData MaterialProperties => materialProperties;


		
		public ColorMode ColoringMode {
			get => colorMode;
			set => colorMode = value;
		}


		
		public Color SecondColor {
			get => secondColor;
			set => secondColor = value;
		}


		public float GetMaterialPropertyValue(int propertyIndex)
		{
			return ImageAppearanceProviderHelper.GetMaterialPropertyValue(propertyIndex, ref materialProperty1,
				ref materialProperty2, ref materialProperty3);
		}


		public void SetMaterialProperty(int propertyIndex, float value)
		{
			ImageAppearanceProviderHelper.SetMaterialProperty(propertyIndex, value, this, materialProperties,
				ref materialProperty1, ref materialProperty2, ref materialProperty3);
		}


		public void OnResolutionChanged()
		{
			SetVerticesDirty();
		}


		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (animator != null && MaterialProperties.FloatProperties != null &&
			    Application.isPlaying == animator.isActiveAndEnabled)
			{
				if (MaterialProperties.FloatProperties.Length != 0)
				{
					MaterialProperties.FloatProperties[0].Value = materialProperty1;
				}

				if (MaterialProperties.FloatProperties.Length > 1)
				{
					MaterialProperties.FloatProperties[1].Value = materialProperty2;
				}

				if (MaterialProperties.FloatProperties.Length > 2)
				{
					MaterialProperties.FloatProperties[2].Value = materialProperty3;
				}
			}

			if (overrideSprite == null)
			{
				GenerateSimpleSprite(toFill, false);
				return;
			}

			switch (type)
			{
				case Type.Simple:
					GenerateSimpleSprite(toFill, preserveAspect);
					return;
				case Type.Sliced:
					GenerateSlicedSprite(toFill);
					return;
				case Type.Tiled:
					GenerateTiledSprite(toFill);
					return;
			}

			base.OnPopulateMesh(toFill);
		}


		private void GenerateSimpleSprite(VertexHelper vh, bool preserveAspect)
		{
			Rect drawingRect = GetDrawingRect(preserveAspect);
			Vector4 vector = overrideSprite == null ? Vector4.zero : DataUtility.GetOuterUV(overrideSprite);
			vh.Clear();
			AddQuad(vh, drawingRect, drawingRect.min, drawingRect.max, colorMode, color, secondColor,
				new Vector2(vector.x, vector.y), new Vector2(vector.z, vector.w));
		}


		private Rect GetDrawingRect(bool shouldPreserveAspect)
		{
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			if (!shouldPreserveAspect)
			{
				return pixelAdjustedRect;
			}

			Vector4 vector = overrideSprite != null ? DataUtility.GetPadding(overrideSprite) : Vector4.zero;
			Vector2 zero;
			if (overrideSprite != null)
			{
				zero = new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);
			}
			else
			{
				zero = Vector2.zero;
			}

			Vector2 vector2 = zero;
			int num = Mathf.RoundToInt(vector2.x);
			int num2 = Mathf.RoundToInt(vector2.y);
			Vector4 vector3 = new Vector4(vector.x / num, vector.y / num2, (num - vector.z) / num,
				(num2 - vector.w) / num2);
			if (vector2.sqrMagnitude > 0f)
			{
				float num3 = vector2.x / vector2.y;
				Vector2 pivot = rectTransform.pivot;
				if (num3 <= pixelAdjustedRect.width / pixelAdjustedRect.height)
				{
					float width = pixelAdjustedRect.width;
					pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
					float x = pixelAdjustedRect.x;
					float num4 = width - pixelAdjustedRect.width;
					pixelAdjustedRect.x = x + num4 * pivot.x;
				}
				else
				{
					float height = pixelAdjustedRect.height;
					pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
					float y = pixelAdjustedRect.y;
					float num5 = height - pixelAdjustedRect.height;
					pixelAdjustedRect.y = y + num5 * pivot.y;
				}
			}

			return new Rect(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x,
				pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.width * vector3.z,
				pixelAdjustedRect.height * vector3.w);
		}


		private void GenerateSlicedSprite(VertexHelper toFill)
		{
			if (!hasBorder)
			{
				base.OnPopulateMesh(toFill);
				return;
			}

			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector4 vector4;
			if (overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(overrideSprite);
				vector2 = DataUtility.GetInnerUV(overrideSprite);
				vector3 = DataUtility.GetPadding(overrideSprite);
				vector4 = overrideSprite.border;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector4.zero;
			}

			Vector2 vector5 = SpriteBorderScale.CalculateSize(this);
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			vector4 = new Vector4(vector5.x * vector4.x, vector5.y * vector4.y, vector5.x * vector4.z,
				vector5.y * vector4.w);
			vector4 = GetAdjustedBorders(vector4 / pixelsPerUnit, pixelAdjustedRect, KeepBorderAspectRatio,
				new Vector2(overrideSprite.textureRect.width * vector5.x,
					overrideSprite.textureRect.height * vector5.y));
			if (vector4.x + vector4.z > pixelAdjustedRect.width)
			{
				float num = pixelAdjustedRect.width / (vector4.x + vector4.z);
				vector4.x *= num;
				vector4.z *= num;
			}

			if (vector4.y + vector4.w > pixelAdjustedRect.height)
			{
				float num2 = pixelAdjustedRect.height / (vector4.y + vector4.w);
				vector4.y *= num2;
				vector4.w *= num2;
			}

			vector3 /= pixelsPerUnit;
			vertScratch[0] = new Vector2(vector3.x, vector3.y);
			vertScratch[3] = new Vector2(pixelAdjustedRect.width - vector3.z, pixelAdjustedRect.height - vector3.w);
			vertScratch[1].x = vector4.x;
			vertScratch[1].y = vector4.y;
			vertScratch[2].x = pixelAdjustedRect.width - vector4.z;
			vertScratch[2].y = pixelAdjustedRect.height - vector4.w;
			for (int i = 0; i < 4; i++)
			{
				Vector2[] array = vertScratch;
				int num3 = i;
				array[num3].x = array[num3].x + pixelAdjustedRect.x;
				Vector2[] array2 = vertScratch;
				int num4 = i;
				array2[num4].y = array2[num4].y + pixelAdjustedRect.y;
			}

			uvScratch[0] = new Vector2(vector.x, vector.y);
			uvScratch[1] = new Vector2(vector2.x, vector2.y);
			uvScratch[2] = new Vector2(vector2.z, vector2.w);
			uvScratch[3] = new Vector2(vector.z, vector.w);
			toFill.Clear();
			for (int j = 0; j < 3; j++)
			{
				int num5 = j + 1;
				for (int k = 0; k < 3; k++)
				{
					if (fillCenter || j != 1 || k != 1)
					{
						int num6 = k + 1;
						Vector2 posMin = new Vector2(vertScratch[j].x, vertScratch[k].y);
						Vector2 posMax = new Vector2(vertScratch[num5].x, vertScratch[num6].y);
						AddQuad(toFill, pixelAdjustedRect, posMin, posMax, colorMode, color, secondColor,
							new Vector2(uvScratch[j].x, uvScratch[k].y),
							new Vector2(uvScratch[num5].x, uvScratch[num6].y));
					}
				}
			}
		}


		private void GenerateTiledSprite(VertexHelper toFill)
		{
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector2 vector4;
			if (overrideSprite == null)
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector2.one * 100f;
			}
			else
			{
				vector = DataUtility.GetOuterUV(overrideSprite);
				vector2 = DataUtility.GetInnerUV(overrideSprite);
				vector3 = overrideSprite.border;
				vector4 = overrideSprite.rect.size;
			}

			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			float num = (vector4.x - vector3.x - vector3.z) / pixelsPerUnit;
			float num2 = (vector4.y - vector3.y - vector3.w) / pixelsPerUnit;
			vector3 = GetAdjustedBorders(vector3 / pixelsPerUnit, pixelAdjustedRect, false,
				new Vector2(overrideSprite.textureRect.width, overrideSprite.textureRect.height));
			Vector2 vector5 = SpriteBorderScale.CalculateSize(this);
			num *= vector5.x;
			num2 *= vector5.y;
			Vector2 vector6 = new Vector2(vector2.x, vector2.y);
			Vector2 vector7 = new Vector2(vector2.z, vector2.w);
			UIVertex.simpleVert.color = color;
			float num3 = vector5.x * vector3.x;
			float num4 = pixelAdjustedRect.width - vector5.x * vector3.z;
			float num5 = vector5.y * vector3.y;
			float num6 = pixelAdjustedRect.height - vector5.y * vector3.w;
			toFill.Clear();
			Vector2 vector8 = vector7;
			Vector2 position = pixelAdjustedRect.position;
			if (num <= 0f)
			{
				num = num4 - num3;
			}

			if (num2 <= 0f)
			{
				num2 = num6 - num5;
			}

			if (fillCenter)
			{
				for (float num7 = num5; num7 < num6; num7 += num2)
				{
					float num8 = num7 + num2;
					if (num8 > num6)
					{
						vector8.y = vector6.y + (vector7.y - vector6.y) * (num6 - num7) / (num8 - num7);
						num8 = num6;
					}

					vector8.x = vector7.x;
					for (float num9 = num3; num9 < num4; num9 += num)
					{
						float num10 = num9 + num;
						if (num10 > num4)
						{
							vector8.x = vector6.x + (vector7.x - vector6.x) * (num4 - num9) / (num10 - num9);
							num10 = num4;
						}

						AddQuad(toFill, pixelAdjustedRect, new Vector2(num9, num7) + position,
							new Vector2(num10, num8) + position, colorMode, color, secondColor, vector6, vector8);
					}
				}
			}

			if (hasBorder)
			{
				vector8 = vector7;
				for (float num11 = num5; num11 < num6; num11 += num2)
				{
					float num12 = num11 + num2;
					if (num12 > num6)
					{
						vector8.y = vector6.y + (vector7.y - vector6.y) * (num6 - num11) / (num12 - num11);
						num12 = num6;
					}

					AddQuad(toFill, pixelAdjustedRect, new Vector2(0f, num11) + position,
						new Vector2(num3, num12) + position, colorMode, color, secondColor,
						new Vector2(vector.x, vector6.y), new Vector2(vector6.x, vector8.y));
					AddQuad(toFill, pixelAdjustedRect, new Vector2(num4, num11) + position,
						new Vector2(pixelAdjustedRect.width, num12) + position, colorMode, color, secondColor,
						new Vector2(vector7.x, vector6.y), new Vector2(vector.z, vector8.y));
				}

				vector8 = vector7;
				for (float num13 = num3; num13 < num4; num13 += num)
				{
					float num14 = num13 + num;
					if (num14 > num4)
					{
						vector8.x = vector6.x + (vector7.x - vector6.x) * (num4 - num13) / (num14 - num13);
						num14 = num4;
					}

					AddQuad(toFill, pixelAdjustedRect, new Vector2(num13, 0f) + position,
						new Vector2(num14, num5) + position, colorMode, color, secondColor,
						new Vector2(vector6.x, vector.y), new Vector2(vector8.x, vector6.y));
					AddQuad(toFill, pixelAdjustedRect, new Vector2(num13, num6) + position,
						new Vector2(num14, pixelAdjustedRect.height) + position, colorMode, color, secondColor,
						new Vector2(vector6.x, vector7.y), new Vector2(vector8.x, vector.w));
				}

				AddQuad(toFill, pixelAdjustedRect, new Vector2(0f, 0f) + position, new Vector2(num3, num5) + position,
					colorMode, color, secondColor, new Vector2(vector.x, vector.y), new Vector2(vector6.x, vector6.y));
				AddQuad(toFill, pixelAdjustedRect, new Vector2(num4, 0f) + position,
					new Vector2(pixelAdjustedRect.width, num5) + position, colorMode, color, secondColor,
					new Vector2(vector7.x, vector.y), new Vector2(vector.z, vector6.y));
				AddQuad(toFill, pixelAdjustedRect, new Vector2(0f, num6) + position,
					new Vector2(num3, pixelAdjustedRect.height) + position, colorMode, color, secondColor,
					new Vector2(vector.x, vector7.y), new Vector2(vector6.x, vector.w));
				AddQuad(toFill, pixelAdjustedRect, new Vector2(num4, num6) + position,
					new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + position, colorMode, color,
					secondColor, new Vector2(vector7.x, vector7.y), new Vector2(vector.z, vector.w));
			}
		}


		private void AddQuad(VertexHelper vertexHelper, Rect bounds, Vector2 posMin, Vector2 posMax, ColorMode mode,
			Color colorA, Color colorB, Vector2 uvMin, Vector2 uvMax)
		{
			ImageAppearanceProviderHelper.AddQuad(vertexHelper, bounds, posMin, posMax, mode, colorA, colorB, uvMin,
				uvMax, materialProperties);
		}


		private Vector4 GetAdjustedBorders(Vector4 border, Rect rect, bool keepAspect, Vector2 texSize)
		{
			float num = 1f;
			for (int i = 0; i <= 1; i++)
			{
				float num2 = border[i] + border[i + 2];
				if (rect.size[i] < num2)
				{
					if (keepAspect)
					{
						num = Mathf.Min(num, rect.size[i] / num2);
					}
					else
					{
						float num3 = rect.size[i] / num2;
						ref Vector4 ptr = ref border;
						int index = i;
						ptr[index] *= num3;
						ptr = ref border;
						index = i + 2;
						ptr[index] *= num3;
					}
				}
				else if (num2 == 0f && keepAspect)
				{
					int num4 = (i + 1) % 2;
					num2 = border[num4] + border[num4 + 2];
					num = rect.size[i] / texSize[i];
					if (num * num2 > rect.size[num4])
					{
						num = rect.size[num4] / num2;
					}
				}
			}

			if (keepAspect)
			{
				border = num * border;
			}

			return border;
		}
	}
}