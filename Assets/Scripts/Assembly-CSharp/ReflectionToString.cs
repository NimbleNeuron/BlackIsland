using System;
using System.Reflection;
using System.Text;
using Blis.Common;


public static class ReflectionToString
{
	
	public static string ToReflectionString(this object obj)
	{
		Type type = obj.GetType();
		StringBuilder stringBuilder = GameUtil.StringBuilder;
		stringBuilder.Clear();
		stringBuilder.Append(type.Name);
		stringBuilder.Append("{\n");
		FieldInfo[] fields = type.GetFields();
		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			object value = fieldInfo.GetValue(obj);
			string value2 = (value != null) ? value.ToString() : "null";
			stringBuilder.Append("\t");
			stringBuilder.Append(fieldInfo.Name);
			stringBuilder.Append(": ");
			stringBuilder.Append(value2);
			if (i != fields.Length - 1)
			{
				stringBuilder.Append("\n");
			}
		}
		stringBuilder.Append("\n}");
		return stringBuilder.ToString();
	}
}
