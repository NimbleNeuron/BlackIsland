using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UISkillAdd : BaseUI
	{
		[SerializeField] private LayoutElement[] skillDesc = default;


		[SerializeField] private Text[] skillDescAdd = default;


		[SerializeField] private Text[] skillDescValue = default;


		[SerializeField] private LayoutElement layoutElement = default;

		public void SetSkillData(string[] param, string[] value)
		{
			Clear();
			if (param == null || value == null)
			{
				return;
			}

			bool flag = false;
			int num = 0;
			while (num < param.Length && !string.IsNullOrEmpty(param[num]) && skillDesc.Length > num)
			{
				flag = true;
				skillDesc[num].ignoreLayout = false;
				skillDesc[num].transform.localScale = Vector3.one;
				skillDescAdd[num].text = param[num];
				skillDescValue[num].text = value[num];
				num++;
			}

			if (flag)
			{
				layoutElement.ignoreLayout = false;
				transform.localScale = Vector3.one;
			}
		}


		public void Clear()
		{
			layoutElement.ignoreLayout = true;
			transform.localScale = Vector3.zero;
			for (int i = 0; i < skillDesc.Length; i++)
			{
				skillDesc[i].ignoreLayout = true;
				skillDesc[i].transform.localScale = Vector3.zero;
			}
		}
	}
}