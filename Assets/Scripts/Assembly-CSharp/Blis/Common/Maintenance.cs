using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class Maintenance
	{
		public readonly string apiStatus;


		public readonly DateTime endDtm;


		public readonly string messageId;


		public readonly bool start;


		public readonly DateTime startDtm;


		public readonly string status;

		[JsonConstructor]
		public Maintenance(bool start, long startDtm, long endDtm, string messageId, string apiStatus, string status)
		{
			this.start = start;
			this.startDtm = GameUtil.ConvertFromUnixTimestamp(startDtm / 1000L);
			this.endDtm = GameUtil.ConvertFromUnixTimestamp(endDtm / 1000L);
			this.messageId = messageId;
			this.apiStatus = apiStatus;
			this.status = status;
		}
	}
}