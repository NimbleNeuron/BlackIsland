using System.Collections.Generic;
using Blis.Client;


public class TutorialSubQuestData
{
	
	public TutorialSubQuestData(int code, string comment, int detailGroupId = -1, List<TutorialQuestInfo> tutorialQuestInfos = null)
	{
		this.code = code;
		this.comment = comment;
		this.detailGroupId = detailGroupId;
		this.tutorialQuestInfos = tutorialQuestInfos;
	}

	
	public readonly int code;

	
	public readonly string comment;

	
	public readonly int detailGroupId;

	
	public readonly List<TutorialQuestInfo> tutorialQuestInfos;
}
