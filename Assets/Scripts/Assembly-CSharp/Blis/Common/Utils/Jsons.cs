using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Blis.Common.Utils
{
	public static class Jsons
	{
		private static readonly IContractResolver Resolver = new DefaultContractResolver();


		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			Formatting = Formatting.None,
			ContractResolver = Resolver,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore
		};


		private static readonly JsonSerializer Serializer = JsonSerializer.CreateDefault(Settings);


		public static readonly IArrayPool<char> Pool = JsonsRxArrayPool<char>.Shared;

		public static string DebugSerialize<T>(T entity)
		{
			return Serialize<T>(entity);
		}


		public static string Serialize<T>(T entity)
		{
			string result;
			using (StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture))
			{
				using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
				{
					ImplSerialize<T>(jsonTextWriter, entity);
					result = stringWriter.ToString();
				}
			}

			return result;
		}


		public static T Deserialize<T>(string text)
		{
			T result;
			using (StringReader stringReader = new StringReader(text))
			{
				using (JsonTextReader jsonTextReader = new JsonTextReader(stringReader))
				{
					result = ImplDeserialize<T>(jsonTextReader);
				}
			}

			return result;
		}


		public static object Deserialize(string text, Type type)
		{
			object result;
			using (StringReader stringReader = new StringReader(text))
			{
				using (JsonTextReader jsonTextReader = new JsonTextReader(stringReader))
				{
					result = ImplDeserialize(jsonTextReader, type);
				}
			}

			return result;
		}


		public static T Deserialize<T>(Stream stream)
		{
			T result;
			using (StreamReader streamReader = new StreamReader(stream))
			{
				using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
				{
					result = ImplDeserialize<T>(jsonTextReader);
				}
			}

			return result;
		}


		private static void ImplSerialize<T>(JsonTextWriter writer, T entity)
		{
			writer.ArrayPool = Pool;
			Serializer.Serialize(writer, entity, null);
		}


		public static T ImplDeserialize<T>(JsonTextReader reader)
		{
			reader.ArrayPool = Pool;
			return Serializer.Deserialize<T>(reader);
		}


		public static object ImplDeserialize(JsonTextReader reader, Type type)
		{
			reader.ArrayPool = Pool;
			return Serializer.Deserialize(reader, type);
		}


		public class JsonsRxArrayPool<T> : IArrayPool<T>
		{
			public static readonly JsonsRxArrayPool<T> Shared = new JsonsRxArrayPool<T>();


			private JsonsRxArrayPool() { }


			private static RxArrayPool<T> Internal => RxArrayPool<T>.Shared;


			public T[] Rent(int minimumLength)
			{
				return Internal.Rent(minimumLength);
			}


			public void Return(T[] array)
			{
				Internal.Return(array);
			}
		}
	}
}