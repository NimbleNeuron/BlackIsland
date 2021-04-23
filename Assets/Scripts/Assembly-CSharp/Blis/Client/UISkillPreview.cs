using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UISkillPreview : BaseUI
	{
		[SerializeField] private Text title = default;


		[SerializeField] private Text skillName = default;


		[SerializeField] private Image image = default;

		public void SetTitle(string title)
		{
			this.title.text = title;
		}


		public void SetSkillName(string skillName)
		{
			this.skillName.text = skillName;
		}


		public void SetSkillSprite(Sprite sprite)
		{
			image.sprite = sprite;
		}
	}
}