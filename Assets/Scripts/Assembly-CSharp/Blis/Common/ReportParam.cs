namespace Blis.Common
{
	public class ReportParam
	{
		public long gameId;
		public string reportedNickname;
		public long reportedUserNum;
		public string reportType;

		public ReportParam(ReportType reportType, long reportedUserNum, string reportedNickname, long gameId)
		{
			this.reportType = reportType.ToString();
			this.reportedUserNum = reportedUserNum;
			this.reportedNickname = reportedNickname;
			this.gameId = gameId;
		}
	}
}