using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class MicrosecondEpochConverter : DateTimeConverterBase
	{
		
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue((((DateTime)value).ToUniversalTime() - MicrosecondEpochConverter.EpochBase).TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
		}

		
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
			{
				return null;
			}
			long num = (long)reader.Value;
			return MicrosecondEpochConverter.EpochBase.AddMilliseconds((double)num).ToLocalTime();
		}

		
		private static readonly DateTime EpochBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
