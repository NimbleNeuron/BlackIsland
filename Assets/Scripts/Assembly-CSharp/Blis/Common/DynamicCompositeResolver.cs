using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace Blis.Common
{
	
	public class DynamicCompositeResolver : IFormatterResolver
	{
		
		public void Register(params IFormatterResolver[] resolvers)
		{
			DynamicCompositeResolver.resolvers.AddRange(resolvers);
		}

		
		public void Register(IFormatterResolver resolver)
		{
			DynamicCompositeResolver.resolvers.Add(resolver);
		}

		
		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return DynamicCompositeResolver.FormatterCache<T>.formatter;
		}

		
		public static readonly DynamicCompositeResolver Instance = new DynamicCompositeResolver();

		
		private static List<IFormatterResolver> resolvers = new List<IFormatterResolver>();

		
		private static class FormatterCache<T>
		{
			
			static FormatterCache()
			{
				foreach (IFormatterResolver formatterResolver in DynamicCompositeResolver.resolvers)
				{
					IMessagePackFormatter<T> messagePackFormatter = formatterResolver.GetFormatter<T>();
					if (messagePackFormatter != null)
					{
						DynamicCompositeResolver.FormatterCache<T>.formatter = messagePackFormatter;
						break;
					}
				}
			}

			
			public static readonly IMessagePackFormatter<T> formatter;
		}
	}
}
