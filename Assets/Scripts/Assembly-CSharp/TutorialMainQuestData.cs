using Blis.Common;


public class TutorialMainQuestData
{
	
	public TutorialMainQuestData(TutorialType tutorialType, int targetStack, int characterCode, string mainTitle, string mainComment, int detailGroupId = -1)
	{
		this.tutorialType = tutorialType;
		this.targetStack = targetStack;
		this.characterCode = characterCode;
		this.mainTitle = mainTitle;
		this.mainComment = mainComment;
		this.detailGroupId = detailGroupId;
	}

	
	public readonly TutorialType tutorialType;

	
	public readonly int targetStack;

	
	public readonly int characterCode;

	
	public readonly string mainTitle;

	
	public readonly string mainComment;

	
	public readonly int detailGroupId;
}
