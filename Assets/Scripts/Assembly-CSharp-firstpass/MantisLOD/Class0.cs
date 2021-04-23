using System;

namespace MantisLOD
{
	internal class Class0 : IComparable
	{
		public bool bool_0;


		public Class0 class0_0;


		public Class1 class1_0;


		public Class2 class2_0;


		public float float_0;


		public int int_0;


		public int int_1;


		public Class0()
		{
			bool_0 = true;
		}


		public int CompareTo(object object_0)
		{
			return float_0.CompareTo((object_0 as Class0).float_0);
		}
	}
}