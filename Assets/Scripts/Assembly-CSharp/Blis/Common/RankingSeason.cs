using System;

namespace Blis.Common
{
	public class RankingSeason
	{
		public DateTime endDtm;


		public int id;


		public RankingSeasonType seasonType;


		public DateTime startDtm;


		public string title;


		public RankingSeason(int id, string title, RankingSeasonType seasonType, long startDtm, long endDtm)
		{
			this.id = id;
			this.title = title;
			this.seasonType = seasonType;
			this.startDtm = GameUtil.ConvertFromUnixTimestamp(startDtm / 1000L);
			this.endDtm = GameUtil.ConvertFromUnixTimestamp(endDtm / 1000L);
		}
	}
}