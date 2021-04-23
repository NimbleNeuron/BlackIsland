using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UISkillBody : BaseUI
	{
		[SerializeField] private Text skillDesc = default;


		[SerializeField] private LayoutElement layoutElement = default;

		public void SetDesc(string desc)
		{
			layoutElement.ignoreLayout = false;
			transform.localScale = Vector3.one;
			skillDesc.text = desc;
			skillDesc.CalculateLayoutInputVertical();
		}


		public void Clear()
		{
			layoutElement.ignoreLayout = true;
			transform.localScale = Vector3.zero;
		}
	}
}