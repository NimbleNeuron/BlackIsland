namespace Blis.Client
{
	public class TutorialQuestInfo
	{
		private bool isSuccess;


		public TutorialQuestInfo(TutorialQuestType tutorialQuestType, bool isSuccess)
		{
			TutorialQuestType = tutorialQuestType;
			this.isSuccess = isSuccess;
		}


		public TutorialQuestType TutorialQuestType { get; }


		public bool IsSuccess => isSuccess;


		public void Success()
		{
			isSuccess = true;
		}
	}
}