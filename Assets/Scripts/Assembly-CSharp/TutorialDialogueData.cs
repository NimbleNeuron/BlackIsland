public class TutorialDialogueData
{
	
	public TutorialDialogueData(int code, int characterCode, string comment, string img = "")
	{
		this.code = code;
		this.characterCode = characterCode;
		this.comment = comment;
		this.img = img;
	}

	
	public readonly int code;

	
	public readonly int characterCode;

	
	public readonly string comment;

	
	public readonly string img;
}
