using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Blis.Client
{
	public class StringTo64bitHashCodeBuilderKnuth
	{
		private readonly List<string> paramList = new List<string>();


		public static long CalculateHash(string read)
		{
			long num = 3074457345618258791L;
			for (int i = 0; i < read.Length; i++)
			{
				num += read[i];
				num *= 3074457345618258799L;
			}

			return num;
		}


		public static long CalculateHashMultipleParam(params object[] objects)
		{
			string text = "";
			foreach (object obj in objects)
			{
				text = text + "?" + obj;
			}

			return CalculateHash(text);
		}


		public StringTo64bitHashCodeBuilderKnuth AddString(string str)
		{
			paramList.Add(str);
			return this;
		}


		public StringTo64bitHashCodeBuilderKnuth AddClassType(object classObject)
		{
			List<string> list = new List<string>();
			if (classObject.GetType().IsPrimitive)
			{
				paramList.Add(classObject.ToString());
				return this;
			}

			foreach (FieldInfo fieldInfo in classObject.GetType().GetFields())
			{
				object value = fieldInfo.GetValue(classObject);
				list.Add(value.ToString());
				if (fieldInfo.FieldType.IsArray)
				{
					foreach (object obj in (IList) value)
					{
						list.Add(obj.ToString());
					}
				}
			}

			paramList.AddRange(list);
			return this;
		}


		public long Build()
		{
			object[] objects = paramList.ToArray();
			return CalculateHashMultipleParam(objects);
		}
	}
}