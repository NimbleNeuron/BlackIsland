using System.Collections.Generic;

namespace MantisLOD
{
	internal abstract class Class5
	{
		private const int int_0 = 1;


		private readonly List<Class0> list_0;


		public Class5()
		{
			list_0 = new List<Class0>();
			list_0.Add(new Class0());
		}


		public Class5(int int_1)
		{
			list_0 = new List<Class0>(int_1);
			list_0.Add(new Class0());
		}


		public void method_0(Class0 class0_0)
		{
			list_0.Add(class0_0);
			class0_0.int_0 = method_5();
			method_6(method_5());
		}


		public Class0 method_1()
		{
			if (method_5() == 0)
			{
				return null;
			}

			Class0 result = list_0[1];
			list_0[1].int_0 = -1;
			list_0[1] = list_0[method_5()];
			list_0[1].int_0 = 1;
			method_7(1);
			list_0.RemoveAt(method_5());
			return result;
		}


		public bool method_2(int int_1)
		{
			if (method_5() == 0)
			{
				return false;
			}

			list_0[int_1].int_0 = -1;
			list_0[int_1] = list_0[method_5()];
			list_0[int_1].int_0 = int_1;
			method_7(int_1);
			list_0.RemoveAt(method_5());
			return true;
		}


		public int method_3()
		{
			return list_0.Count - 1;
		}


		public Class0 method_4()
		{
			if (method_5() == 0)
			{
				return null;
			}

			return list_0[1];
		}


		private int method_5()
		{
			return list_0.Count - 1;
		}


		protected abstract bool vmethod_0(Class0 class0_0, Class0 class0_1);


		private void method_6(int int_1)
		{
			int num = int_1;
			int num2 = int_1 / 2;
			Class0 @class = list_0[num];
			while (num > 1 && vmethod_0(list_0[num2], @class))
			{
				list_0[num] = list_0[num2];
				list_0[num].int_0 = num;
				num = num2;
				num2 /= 2;
			}

			list_0[num] = @class;
			list_0[num].int_0 = num;
		}


		private void method_7(int int_1)
		{
			int index = int_1;
			int i = int_1 * 2;
			Class0 @class = list_0[index];
			while (i <= method_5())
			{
				if (i < method_5() && vmethod_0(list_0[i], list_0[i + 1]))
				{
					i++;
				}

				if (!vmethod_0(@class, list_0[i]))
				{
					break;
				}

				list_0[index] = list_0[i];
				list_0[index].int_0 = index;
				index = i;
				i *= 2;
			}

			list_0[index] = @class;
			list_0[index].int_0 = index;
		}
	}
}