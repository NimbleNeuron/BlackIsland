using System.Collections.Generic;


public class TutorialMessageBoxData
{
	
	public TutorialMessageBoxData(string title, string subTitle, string desc, string imgName, Dictionary<int, TutorialItemDataInfo> dicItems)
	{
		this.title = title;
		this.subTitle = subTitle;
		this.desc = desc;
		this.imgName = imgName;
		this.dicItems = dicItems;
	}

	
	public readonly string title;

	
	public readonly string subTitle;

	
	public readonly string desc;

	
	public readonly string imgName;

	
	public readonly Dictionary<int, TutorialItemDataInfo> dicItems;
}
