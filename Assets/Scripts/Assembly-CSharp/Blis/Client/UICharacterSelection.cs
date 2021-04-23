using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UICharacterSelection : BaseControl
	{
		[SerializeField] private Image characterImage;


		private int characterCode;


		public int CharacterCode => characterCode;


		public void SetCharacter(int code)
		{
			characterCode = code;
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
		}
	}
}