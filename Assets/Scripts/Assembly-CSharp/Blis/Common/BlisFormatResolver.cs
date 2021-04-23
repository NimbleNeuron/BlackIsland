using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace Blis.Common
{
	public class BlisFormatResolver : IFormatterResolver
	{
		public static IFormatterResolver Instance = new BlisFormatResolver();


		private static readonly Dictionary<Type, Type> formatterMap = new Dictionary<Type, Type>
		{
			{
				typeof(List<PacketWrapper>),
				typeof(PacketListFormatter)
			},
			{
				typeof(BlisVector),
				typeof(BlisVectorFormatter)
			},
			{
				typeof(BlisFixedPoint),
				typeof(BlisFixedPointFormatter)
			}
		};


		private BlisFormatResolver() { }


		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}

		internal static object GetFormatter(Type t)
		{
			Type type;
			if (formatterMap.TryGetValue(t, out type))
			{
				return Activator.CreateInstance(type);
			}

			return null;
		}


		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter =
				(IMessagePackFormatter<T>) GetFormatter(typeof(T));

			static FormatterCache()
			{
				if (formatter == null)
				{
					formatter = Instance.GetFormatter<T>();
				}
			}
		}
	}
}