using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class CharacterSelectionView : BaseUI, ICharacterSelectCardListener
	{
		[SerializeField] private GameObject characterCardPrefab = default;


		[SerializeField] private Transform characterCardsParent = default;


		private readonly List<LobbyPortraitCharacterCard> cards = new List<LobbyPortraitCharacterCard>();


		private int currentCharacterCode;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			NoticeService.onReceivedCharacter -= OnReceivedCharacter;
			ShopProductService.onReceivedDlcCharacter -= OnReceivedCharacter;
		}


		public void OnClickCharacterCard(int characterCode)
		{
			if (currentCharacterCode == characterCode)
			{
				return;
			}

			currentCharacterCode = characterCode;
			Singleton<SoundControl>.inst.StopVoiceAudio();
			Singleton<SoundControl>.inst.StopSfxAudio();
			MonoBehaviourInstance<LobbyService>.inst.SelectCharacter(characterCode);
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			NoticeService.onReceivedCharacter += OnReceivedCharacter;
			ShopProductService.onReceivedDlcCharacter += OnReceivedCharacter;
		}


		private void OnReceivedCharacter(int characterCode)
		{
			LobbyPortraitCharacterCard lobbyPortraitCharacterCard = cards.Find(x => x.CharacterCode == characterCode);
			if (lobbyPortraitCharacterCard != null)
			{
				bool freeRotation = MonoBehaviourInstance<MatchingService>.inst.IsFreeCharacter(characterCode);
				lobbyPortraitCharacterCard.SetCharacterCode(characterCode, true, freeRotation);
			}
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			List<int> list = (from c in GameDB.character.GetAllCharacterData()
				select c.code).ToList<int>();
			if (characterCardsParent.childCount < list.Count)
			{
				for (int i = 0; i < list.Count - characterCardsParent.childCount; i++)
				{
					Instantiate<GameObject>(characterCardPrefab, characterCardsParent);
				}
			}

			for (int j = 0; j < characterCardsParent.childCount; j++)
			{
				LobbyPortraitCharacterCard component =
					characterCardsParent.GetChild(j).GetComponent<LobbyPortraitCharacterCard>();
				if (j < list.Count)
				{
					int characterCode = list[j];
					bool have = Lobby.inst.CharacterList.Exists(x => x.characterCode == characterCode);
					bool freeRotation = MonoBehaviourInstance<MatchingService>.inst.IsFreeCharacter(characterCode);
					component.SetCharacterCode(characterCode, have, freeRotation);
					component.SetListener(this);
					cards.Add(component);
				}
				else if (component != null)
				{
					component.SetCharacterCode(0, false, false);
				}
			}
		}


		public void FocusCharacter(int characterCode)
		{
			foreach (LobbyPortraitCharacterCard lobbyPortraitCharacterCard in cards)
			{
				lobbyPortraitCharacterCard.Focus(lobbyPortraitCharacterCard.CharacterCode == characterCode);
			}
		}
	}
}