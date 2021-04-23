using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterView : BaseUI
	{
		[SerializeField] private Text nickname = default;

		public void SetNickname(string name)
		{
			nickname.text = name;
		}
	}
}