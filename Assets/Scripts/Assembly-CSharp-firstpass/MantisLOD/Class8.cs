using System;
using System.Collections.Generic;
using UnityEngine;

namespace MantisLOD
{
	internal class Class8
	{
		private readonly Class6 class6_0 = new Class6();


		private readonly List<Class1> list_0 = new List<Class1>();


		private readonly List<Class2> list_1 = new List<Class2>();


		private readonly List<Class0> list_2 = new List<Class0>();


		private readonly List<Class4> list_3 = new List<Class4>();


		private readonly List<Vector3> list_4 = new List<Vector3>();


		private readonly List<Vector4> list_5 = new List<Vector4>();


		private readonly List<Vector2> list_6 = new List<Vector2>();


		private bool bool_0;


		private bool bool_1;


		private bool bool_2;


		private bool bool_3;


		private bool bool_4;


		private float float_0;


		private int int_0;


		private int int_1;


		private int int_2;


		private Vector3 vector3_0;


		private Vector3 vector3_1;


		public Class8()
		{
			bool_0 = true;
			bool_1 = false;
			bool_2 = false;
			bool_3 = false;
			bool_4 = false;
			int_1 = 0;
		}


		public int method_0()
		{
			return list_3.Count;
		}


		public void method_1(Vector3[] vector3_2, int int_3, int[] int_4, int int_5, Vector3[] vector3_3, int int_6,
			Color[] color_0, int int_7, Vector2[] vector2_0, int int_8, int int_9, int int_10, int int_11, int int_12,
			int int_13)
		{
			if (list_3.Count == 0)
			{
				bool_0 = int_9 == 1;
				bool_1 = int_10 == 1;
				bool_2 = int_11 == 1;
				bool_3 = int_12 == 1;
				bool_4 = int_13 == 1;
				method_3(vector3_2, int_3, int_4, int_5, vector3_3, int_6, color_0, int_7, vector2_0, int_8);
				method_11();
				method_13();
				method_14(0);
			}
		}


		public void method_2(int int_3, int[] int_4, ref int int_5)
		{
			if (list_3.Count == 0)
			{
				int_5 = 0;
				return;
			}

			int_3 = Math.Max(Math.Min(int_3, list_3.Count), 0);
			int_3 = method_14(int_3);
			int num = list_3.Count;
			List<List<int>> list = new List<List<int>>(int_2);
			for (int i = 0; i < int_2; i++)
			{
				list.Add(new List<int>());
			}

			int num2 = list_3.Count - 1;
			while (num2 >= 0 && num != int_3)
			{
				foreach (Class2 @class in list_3[num2].list_0)
				{
					list[@class.class0_0.class2_0.int_0].Add(@class.class0_0.int_1);
					list[@class.class0_0.class0_0.class2_0.int_0].Add(@class.class0_0.class0_0.int_1);
					list[@class.class0_0.class0_0.class0_0.class2_0.int_0].Add(@class.class0_0.class0_0.class0_0.int_1);
				}

				num--;
				num2--;
			}

			num = 0;
			for (int j = 0; j < int_2; j++)
			{
				int num3 = int_4[num] = list[j].Count;
				num++;
				if (num3 > 0)
				{
					list[j].CopyTo(int_4, num);
					num += num3;
				}
			}

			int_5 = num;
		}


		private void method_3(Vector3[] vector3_2, int int_3, int[] int_4, int int_5, Vector3[] vector3_3, int int_6,
			Color[] color_0, int int_7, Vector2[] vector2_0, int int_8)
		{
			Dictionary<Vector3, int> dictionary = new Dictionary<Vector3, int>(new Class7());
			List<int> list = new List<int>();
			for (int i = 0; i < int_3; i++)
			{
				if (!dictionary.ContainsKey(vector3_2[i]))
				{
					Class1 @class = new Class1
					{
						vector3_0 = vector3_2[i]
					};
					if (@class.vector3_0.x > vector3_0.x)
					{
						vector3_0.x = @class.vector3_0.x;
					}

					if (@class.vector3_0.y > vector3_0.y)
					{
						vector3_0.y = @class.vector3_0.y;
					}

					if (@class.vector3_0.z > vector3_0.z)
					{
						vector3_0.z = @class.vector3_0.z;
					}

					if (@class.vector3_0.x < vector3_1.x)
					{
						vector3_1.x = @class.vector3_0.x;
					}

					if (@class.vector3_0.y < vector3_1.y)
					{
						vector3_1.y = @class.vector3_0.y;
					}

					if (@class.vector3_0.z < vector3_1.z)
					{
						vector3_1.z = @class.vector3_0.z;
					}

					int count = list_0.Count;
					dictionary.Add(vector3_2[i], count);
					list.Add(count);
					list_0.Add(@class);
				}
				else
				{
					list.Add(dictionary[vector3_2[i]]);
				}
			}

			int num = 0;
			int j = 0;
			while (j < int_5)
			{
				int num2 = int_4[j];
				j++;
				for (int k = 0; k < num2; k += 3)
				{
					int num3 = list[int_4[j + k]];
					int num4 = list[int_4[j + k + 1]];
					int num5 = list[int_4[j + k + 2]];
					if (num3 != num4 && num4 != num5 && num5 != num3)
					{
						Class2 class2 = new Class2();
						Class0[] array =
						{
							new Class0(),
							new Class0(),
							new Class0()
						};
						for (int l = 0; l < 3; l++)
						{
							array[l].class0_0 = array[(l + 1) % 3];
							array[l].class2_0 = class2;
							int index = list[int_4[j + k + l]];
							array[l].class1_0 = list_0[index];
							array[l].int_1 = int_4[j + k + l];
							list_0[index].list_0.Add(array[l]);
							list_2.Add(array[l]);
						}

						class2.class0_0 = array[0];
						class2.int_0 = num;
						list_1.Add(class2);
					}
				}

				j += num2;
				num++;
			}

			int_2 = num;
			for (int m = 0; m < int_6; m++)
			{
				Vector3 item = vector3_3[m];
				list_4.Add(item);
			}

			for (int n = 0; n < int_7; n++)
			{
				Vector4 item2 = color_0[n];
				list_5.Add(item2);
			}

			for (int num6 = 0; num6 < int_8; num6++)
			{
				Vector2 item3 = vector2_0[num6];
				list_6.Add(item3);
			}

			float_0 = (vector3_0 - vector3_1).sqrMagnitude;
			int_1 = list_1.Count;
		}


		private void method_4(Class2 class2_0)
		{
			class2_0.vector3_0 =
				Vector3.Cross(class2_0.class0_0.class0_0.class1_0.vector3_0 - class2_0.class0_0.class1_0.vector3_0,
					class2_0.class0_0.class0_0.class0_0.class1_0.vector3_0 - class2_0.class0_0.class1_0.vector3_0);
			class2_0.vector3_0.Normalize();
		}


		private void method_5()
		{
			int num = 0;
			foreach (Class2 class2_ in list_1)
			{
				method_4(class2_);
				num++;
			}
		}


		private bool method_6(Class0 class0_0)
		{
			Class1 class1_ = class0_0.class1_0;
			Class1 class1_2 = class0_0.class0_0.class1_0;
			int num = 0;
			foreach (Class0 @class in class1_.list_0)
			{
				foreach (Class0 class2 in class1_2.list_0)
				{
					if (@class.class2_0 == class2.class2_0)
					{
						num++;
						break;
					}
				}
			}

			return num == 1;
		}


		private void method_7()
		{
			int num = 0;
			foreach (Class0 @class in list_2)
			{
				if (method_6(@class))
				{
					@class.class1_0.bool_1 = true;
					@class.class0_0.class1_0.bool_1 = true;
					num++;
				}
			}
		}


		private bool method_8(Class0 class0_0)
		{
			Class1 class1_ = class0_0.class1_0;
			Class1 class1_2 = class0_0.class0_0.class1_0;
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			foreach (Class0 @class in class1_.list_0)
			{
				if (@class.class0_0.class1_0 == class1_2)
				{
					list.Add(@class.class0_0.class0_0.int_1);
				}
			}

			foreach (Class0 class2 in class1_2.list_0)
			{
				if (class2.class0_0.class1_0 == class1_)
				{
					list2.Add(class2.class0_0.class0_0.int_1);
				}
			}

			if (list.Count != list2.Count)
			{
				return false;
			}

			bool flag = false;
			foreach (int num in list)
			{
				if (!flag)
				{
					bool flag2 = false;
					foreach (int num2 in list2)
					{
						if (num != num2 && list_6.Count > 0 && list_6[num] == list_6[num2])
						{
							flag2 = true;
							break;
						}
					}

					if (!flag2)
					{
						flag = true;
					}
				}
			}

			return !flag;
		}


		private void method_9()
		{
			int num = 0;
			foreach (Class0 @class in list_2)
			{
				if (method_8(@class))
				{
					@class.class1_0.bool_2 = true;
					@class.class0_0.class1_0.bool_2 = true;
					num++;
				}
			}
		}


		private float method_10(Class0 class0_0)
		{
			Class1 class1_ = class0_0.class1_0;
			Class1 class1_2 = class0_0.class0_0.class1_0;
			float sqrMagnitude = (class1_2.vector3_0 - class1_.vector3_0).sqrMagnitude;
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<Class2> list3 = new List<Class2>();
			foreach (Class0 @class in class1_.list_0)
			{
				foreach (Class0 class2 in class1_2.list_0)
				{
					if (@class.class2_0 == class2.class2_0)
					{
						list.Add(@class.int_1);
						list2.Add(@class.class2_0.int_0);
						list3.Add(@class.class2_0);
						break;
					}
				}
			}

			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			float num = float.MinValue;
			float num2 = 0f;
			foreach (Class0 class3 in class1_.list_0)
			{
				bool flag5 = false;
				float num3 = float.MaxValue;
				foreach (Class2 class4 in list3)
				{
					float num4 = (1f - Vector3.Dot(class3.class2_0.vector3_0, class4.vector3_0)) * 0.5f;
					if (num4 < num3)
					{
						num3 = num4;
					}

					if (class3.class2_0 == class4)
					{
						flag5 = true;
					}
				}

				if (num3 > num)
				{
					num = num3;
				}

				if (!flag5)
				{
					if (bool_4)
					{
						Class1 class5 = class1_2;
						Class1 class1_3 = class3.class0_0.class1_0;
						Class1 class1_4 = class3.class0_0.class0_0.class1_0;
						Vector3 normalized = (class1_3.vector3_0 - class5.vector3_0).normalized;
						Vector3 normalized2 = (class1_4.vector3_0 - class1_3.vector3_0).normalized;
						Vector3 normalized3 = (class5.vector3_0 - class1_4.vector3_0).normalized;
						float val = Vector3.Dot(normalized3, normalized);
						float val2 = Vector3.Dot(normalized, normalized2);
						float val3 = Vector3.Dot(normalized2, normalized3);
						float num5 = Math.Min(val, Math.Min(val2, val3));
						float num6 = (Math.Max(val, Math.Max(val2, val3)) - num5) * 0.5f;
						if (num6 > num2)
						{
							num2 = num6;
						}
					}

					if (bool_3 && !flag)
					{
						bool flag6 = false;
						foreach (int index in list)
						{
							if (list_4.Count == 0 || list_4[class3.int_1] == list_4[index])
							{
								flag6 = true;
								break;
							}
						}

						if (!flag6)
						{
							flag = true;
						}
					}

					if (!flag2)
					{
						bool flag7 = false;
						foreach (int index2 in list)
						{
							if (list_5.Count == 0 || list_5[class3.int_1] == list_5[index2])
							{
								flag7 = true;
								break;
							}
						}

						if (!flag7)
						{
							flag2 = true;
						}
					}

					if (!flag3)
					{
						bool flag8 = false;
						foreach (int index3 in list)
						{
							if (list_6.Count == 0 || list_6[class3.int_1] == list_6[index3])
							{
								flag8 = true;
								break;
							}
						}

						if (!flag8)
						{
							flag3 = true;
						}
					}

					if (!flag4)
					{
						bool flag9 = false;
						foreach (int num7 in list2)
						{
							if (class3.class2_0.int_0 == num7)
							{
								flag9 = true;
								break;
							}
						}

						if (!flag9)
						{
							flag4 = true;
						}
					}
				}
			}

			float num8 = flag ? float_0 : 0f;
			float num9 = flag2 ? float_0 : 0f;
			float num10 = flag3 ? float_0 : 0f;
			float num11 = flag4 ? float_0 : 0f;
			float num12 = 0f;
			if (bool_2)
			{
				num12 = !class1_.bool_2 || class1_2.bool_2 ? 0f : float_0;
			}

			float num13 = 0f;
			float num14 = 0f;
			if (bool_0)
			{
				if (class1_.bool_1 || class1_2.bool_1)
				{
					num13 = float_0;
				}
			}
			else if (class1_.bool_1)
			{
				if (class1_2.bool_1)
				{
					using (List<Class0>.Enumerator enumerator = class1_.list_0.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Class0 class6 = enumerator.Current;
							if (method_6(class6.class0_0.class0_0))
							{
								Vector3 b = class6.class0_0.class0_0.class1_0.vector3_0;
								Vector3 vector = class1_.vector3_0;
								Vector3 a = class1_2.vector3_0;
								float num15 = (1f - Vector3.Dot((vector - b).normalized, (a - vector).normalized)) *
								              0.5f;
								if (num15 > num14)
								{
									num14 = num15;
								}
							}
						}

						goto IL_5CA;
					}
				}

				num13 = float_0;
			}

			IL_5CA:
			if (bool_1)
			{
				num *= num;
			}

			if (num < 1E-06f)
			{
				num = 1E-06f;
			}

			return (float) (sqrMagnitude * ((num * 20.0 + num14 * 20.0 + num2 * 1.0) / 41.0) + num13 + num8 + num9 +
			                num10 + num12 + num11);
		}


		private void method_11()
		{
			method_5();
			method_7();
			method_9();
			foreach (Class0 @class in list_2)
			{
				@class.float_0 = method_10(@class);
				class6_0.method_0(@class);
			}
		}


		private bool method_12(Class0 class0_0)
		{
			Class1 class1_ = class0_0.class1_0;
			Class1 class1_2 = class0_0.class0_0.class1_0;
			List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
			List<Class2> list2 = new List<Class2>();
			foreach (Class0 @class in class1_.list_0)
			{
				foreach (Class0 class2 in class1_2.list_0)
				{
					if (@class.class2_0 == class2.class2_0)
					{
						if (@class.class0_0.class1_0 == class1_2)
						{
							list.Add(new KeyValuePair<int, int>(@class.int_1, @class.class0_0.int_1));
						}
						else if (@class.class0_0.class0_0.class1_0 == class1_2)
						{
							list.Add(new KeyValuePair<int, int>(@class.int_1, @class.class0_0.class0_0.int_1));
						}

						list2.Add(@class.class2_0);
						break;
					}
				}
			}

			Class4 class3 = new Class4();
			List<Class0> list3 = new List<Class0>();
			foreach (Class0 class4 in class1_.list_0)
			{
				bool flag = false;
				foreach (Class2 class5 in list2)
				{
					if (class4.class2_0 == class5)
					{
						flag = true;
						break;
					}
				}

				if (!flag)
				{
					class4.class1_0 = class1_2;
					int value = list[list.Count - 1].Value;
					foreach (KeyValuePair<int, int> keyValuePair in list)
					{
						if (list_6.Count == 0 || list_6[class4.int_1] == list_6[keyValuePair.Key])
						{
							value = keyValuePair.Value;
							break;
						}
					}

					Class3 item = new Class3
					{
						class0_0 = class4,
						int_0 = class4.int_1,
						int_1 = value
					};
					class3.list_1.Add(item);
					class4.int_1 = value;
					list3.Add(class4);
				}
			}

			foreach (Class0 class6 in class1_2.list_0)
			{
				bool flag2 = false;
				foreach (Class2 class7 in list2)
				{
					if (class6.class2_0 == class7)
					{
						flag2 = true;
						break;
					}
				}

				if (!flag2)
				{
					list3.Add(class6);
				}
			}

			class1_2.list_0 = list3;
			foreach (Class0 class8 in class1_2.list_0)
			{
				class8.float_0 = method_10(class8);
				class8.class0_0.float_0 = method_10(class8.class0_0);
				class8.class0_0.class0_0.float_0 = method_10(class8.class0_0.class0_0);
				class6_0.method_2(class8.int_0);
				class6_0.method_0(class8);
				class6_0.method_2(class8.class0_0.int_0);
				class6_0.method_0(class8.class0_0);
				class6_0.method_2(class8.class0_0.class0_0.int_0);
				class6_0.method_0(class8.class0_0.class0_0);
				method_4(class8.class2_0);
			}

			using (List<Class2>.Enumerator enumerator3 = list2.GetEnumerator())
			{
				IL_510:
				while (enumerator3.MoveNext())
				{
					Class2 class9 = enumerator3.Current;
					class9.bool_0 = false;
					int_1--;
					class3.list_0.Add(class9);
					class9.class0_0.bool_0 = false;
					class9.class0_0.class0_0.bool_0 = false;
					class9.class0_0.class0_0.class0_0.bool_0 = false;
					class6_0.method_2(class9.class0_0.int_0);
					class6_0.method_2(class9.class0_0.class0_0.int_0);
					class6_0.method_2(class9.class0_0.class0_0.class0_0.int_0);
					Class0 class0_ = class9.class0_0;
					Class0 class10 = class0_;
					while (class10.class1_0 == class1_ || class10.class1_0 == class1_2)
					{
						class10 = class10.class0_0;
						if (class10 == class0_)
						{
							goto IL_510;
						}
					}

					class10.class1_0.list_0.Remove(class10);
				}
			}

			class1_.bool_0 = false;
			class3.class1_0 = class1_;
			class3.class1_1 = class1_2;
			if (class0_0.float_0 >= float_0)
			{
				class3.bool_0 = false;
			}

			list_3.Add(class3);
			return true;
		}


		private void method_13()
		{
			int num = int_1;
			while (int_1 > 0 && class6_0.method_4() != null)
			{
				method_12(class6_0.method_4());
				if (num > int_1 + 2500)
				{
					num = int_1;
				}
			}

			int_0 = list_3.Count;
		}


		private int method_14(int int_3)
		{
			while (int_0 != int_3)
			{
				if (int_0 > int_3)
				{
					int_0--;
					using (List<Class3>.Enumerator enumerator = list_3[int_0].list_1.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Class3 @class = enumerator.Current;
							@class.class0_0.class1_0 = list_3[int_0].class1_0;
							@class.class0_0.int_1 = @class.int_0;
						}

						continue;
					}
				}

				if (!list_3[int_0].bool_0)
				{
					break;
				}

				foreach (Class3 class2 in list_3[int_0].list_1)
				{
					class2.class0_0.class1_0 = list_3[int_0].class1_1;
					class2.class0_0.int_1 = class2.int_1;
				}

				int_0++;
			}

			return int_0;
		}
	}
}