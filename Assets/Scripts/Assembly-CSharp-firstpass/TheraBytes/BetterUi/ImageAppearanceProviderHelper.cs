using System;
using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	public static class ImageAppearanceProviderHelper
	{
		public static void SetMaterialType(string value, Graphic graphic, VertexMaterialData materialProperties,
			ref MaterialEffect effect, ref string materialType)
		{
			if (materialType == value && graphic.material != null)
			{
				return;
			}

			UpdateMaterial(graphic, materialProperties, ref effect, ref materialType);
			materialType = value;
		}


		public static void SetMaterialEffect(MaterialEffect value, Graphic graphic,
			VertexMaterialData materialProperties, ref MaterialEffect effect, ref string materialType)
		{
			if (effect == value && graphic.material != null)
			{
				return;
			}

			UpdateMaterial(graphic, materialProperties, ref effect, ref materialType);
			effect = value;
		}


		private static void UpdateMaterial(Graphic graphic, VertexMaterialData materialProperties,
			ref MaterialEffect effect, ref string materialType)
		{
			Materials.MaterialInfo materialInfo =
				SingletonScriptableObject<Materials>.Instance.GetMaterialInfo(materialType, effect);
			materialProperties.Clear();
			if (materialInfo != null)
			{
				graphic.material = materialInfo.Material;
				materialInfo.Properties.CopyTo(materialProperties);
				return;
			}

			graphic.material = null;
		}


		public static void SetMaterialProperty(int propertyIndex, float value, Graphic graphic,
			VertexMaterialData materialProperties, ref float materialProperty1, ref float materialProperty2,
			ref float materialProperty3)
		{
			if (propertyIndex < 0 || propertyIndex >= 3)
			{
				throw new ArgumentException("the propertyIndex can have the value 0, 1 or 2.");
			}

			switch (propertyIndex)
			{
				case 0:
					materialProperty1 = value;
					break;
				case 1:
					materialProperty2 = value;
					break;
				case 2:
					materialProperty3 = value;
					break;
			}

			if (materialProperties.FloatProperties.Length > propertyIndex)
			{
				materialProperties.FloatProperties[propertyIndex].Value = value;
				graphic.SetVerticesDirty();
			}
		}


		public static float GetMaterialPropertyValue(int propertyIndex, ref float materialProperty1,
			ref float materialProperty2, ref float materialProperty3)
		{
			if (propertyIndex < 0 || propertyIndex >= 3)
			{
				throw new ArgumentException("the propertyIndex can have the value 0, 1 or 2.");
			}

			switch (propertyIndex)
			{
				case 0:
					return materialProperty1;
				case 1:
					return materialProperty2;
				case 2:
					return materialProperty3;
				default:
					return 0f;
			}
		}


		public static void AddQuad(VertexHelper vertexHelper, Rect bounds, Vector2 posMin, Vector2 posMax,
			ColorMode mode, Color colorA, Color colorB, Vector2 uvMin, Vector2 uvMax,
			VertexMaterialData materialProperties)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			Color32[] array =
			{
				GetColor(mode, colorA, colorB, bounds, posMin.x, posMin.y),
				GetColor(mode, colorA, colorB, bounds, posMin.x, posMax.y),
				GetColor(mode, colorA, colorB, bounds, posMax.x, posMax.y),
				GetColor(mode, colorA, colorB, bounds, posMax.x, posMin.y)
			};
			float x = 0f;
			float y = 0f;
			float w = 0f;
			materialProperties.Apply(ref x, ref y, ref w);
			Vector2 uv = new Vector2(x, y);
			Vector3 back = Vector3.back;
			Vector4 tangent = new Vector4(1f, 0f, 0f, w);
			vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0f), array[0], new Vector2(uvMin.x, uvMin.y), uv, back,
				tangent);
			vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0f), array[1], new Vector2(uvMin.x, uvMax.y), uv, back,
				tangent);
			vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0f), array[2], new Vector2(uvMax.x, uvMax.y), uv, back,
				tangent);
			vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0f), array[3], new Vector2(uvMax.x, uvMin.y), uv, back,
				tangent);
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}


		private static Color32 GetColor(ColorMode mode, Color a, Color b, Rect bounds, float x, float y)
		{
			switch (mode)
			{
				case ColorMode.Color:
					return a;
				case ColorMode.HorizontalGradient:
				{
					float t = (x - bounds.xMin) / bounds.size.x;
					return Color.Lerp(a, b, t);
				}
				case ColorMode.VerticalGradient:
				{
					float t2 = 1f - (y - bounds.yMin) / bounds.size.y;
					return Color.Lerp(a, b, t2);
				}
				default:
					throw new NotImplementedException();
			}
		}
	}
}