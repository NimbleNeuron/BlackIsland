using System.Collections.Generic;


public class TutorialQuestData
{
	
	public TutorialQuestData(int code, TutorialMainQuestData mainQuestData, List<TutorialSubQuestData> subQuestDatas)
	{
		this.code = code;
		this.mainQuestData = mainQuestData;
		this.subQuestDatas = subQuestDatas;
	}

	
	public readonly int code;

	
	public readonly TutorialMainQuestData mainQuestData;

	
	public readonly List<TutorialSubQuestData> subQuestDatas;
}
