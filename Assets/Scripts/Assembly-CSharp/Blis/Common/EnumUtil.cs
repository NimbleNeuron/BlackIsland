using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public static class EnumUtil
	{
		public static object ParseEnum(Type type, string value)
		{
			return Enum.Parse(type, value);
		}


		public static IEnumerable<T> GetValues<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
	}
}