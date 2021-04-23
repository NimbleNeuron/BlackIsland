using UnityEngine;

namespace Blis.Client
{
	public class CharacterGuide : MonoBehaviour
	{
		[SerializeField] private LnText label = default;


		[SerializeField] private LnText focusLabel = default;

		public void SetName(int characterCode)
		{
			string text = Ln.Get(LnType.Character_Name, characterCode.ToString());
			label.text = text;
			focusLabel.text = text;
		}
	}
}