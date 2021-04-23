using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbySlimCharacterCard : LobbyCharacterCard
	{
		[SerializeField] private Text characterName = default;


		[SerializeField] private Text focusedCharacterName = default;


		[SerializeField] private Image characterSprite = default;


		[SerializeField] private Image focusedCharacterSprite = default;


		private int characterCode;


		private bool have;


		private bool rotationFlag;

		public override void SetCharacterCode(int characterCode, bool have, bool freeRotation)
		{
			if (characterCode <= 0)
			{
				gameObject.SetActive(false);
				return;
			}

			gameObject.SetActive(true);
			this.characterCode = characterCode;
			this.have = have;
			rotationFlag = freeRotation;
			string text = Ln.Get(LnType.Character_Name, characterCode.ToString());
			Sprite characterScoreSprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterScoreSprite(characterCode);
			characterName.text = text;
			focusedCharacterName.text = text;
			characterSprite.sprite = characterScoreSprite;
			focusedCharacterSprite.sprite = characterScoreSprite;
		}


		public override void OnLnDataChange()
		{
			SetCharacterCode(characterCode, have, rotationFlag);
		}
	}
}