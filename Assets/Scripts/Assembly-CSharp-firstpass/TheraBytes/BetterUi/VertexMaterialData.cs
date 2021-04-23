using System;
using System.Collections.Generic;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class VertexMaterialData
	{
		public FloatProperty[] FloatProperties;


		public void Apply(ref float uvX, ref float uvY, ref float tangentW)
		{
			Apply<float>(FloatProperties, ref uvX, ref uvY, ref tangentW);
		}


		private static void Apply<T>(IEnumerable<Property<T>> prop, ref float uvX, ref float uvY, ref float tangentW)
		{
			if (prop == null)
			{
				return;
			}

			foreach (Property<T> property in prop)
			{
				property.SetValue(ref uvX, ref uvY, ref tangentW);
			}
		}


		public void Clear()
		{
			FloatProperties = new FloatProperty[0];
		}


		public void CopyTo(VertexMaterialData target)
		{
			target.FloatProperties = CloneArray<FloatProperty, float>(FloatProperties);
		}


		public VertexMaterialData Clone()
		{
			VertexMaterialData vertexMaterialData = new VertexMaterialData();
			CopyTo(vertexMaterialData);
			return vertexMaterialData;
		}


		private static T[] CloneArray<T, TValue>(T[] array) where T : Property<TValue>
		{
			T[] array2 = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Clone() as T;
			}

			return array2;
		}


		[Serializable]
		public abstract class Property<T>
		{
			public string Name;


			public T Value;


			public abstract void SetValue(ref float uvX, ref float uvY, ref float tangentW);


			public abstract Property<T> Clone();
		}


		[Serializable]
		public class FloatProperty : Property<float>
		{
			public enum Mapping
			{
				TexcoordX,


				TexcoordY,


				TangentW
			}


			public Mapping PropertyMap;


			public float Min;


			public float Max;


			public bool IsRestricted => Min < Max;


			public override void SetValue(ref float uvX, ref float uvY, ref float tangentW)
			{
				switch (PropertyMap)
				{
					case Mapping.TexcoordX:
						uvX = Value;
						return;
					case Mapping.TexcoordY:
						uvY = Value;
						return;
					case Mapping.TangentW:
						tangentW = Value;
						return;
					default:
						throw new ArgumentException();
				}
			}


			public override Property<float> Clone()
			{
				return new FloatProperty
				{
					Name = Name,
					Value = Value,
					PropertyMap = PropertyMap
				};
			}
		}
	}
}