using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class CharacterSelectCharacterSlots : BaseUI, ICharacterSelectCardListener
	{
		private readonly List<LobbyPortraitCharacterCard> characterCards = new List<LobbyPortraitCharacterCard>();


		public void OnClickCharacterCard(int characterCode)
		{
			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(characterCode);
			if (characterMasteryData == null)
			{
				return;
			}

			RecommendStarting recommendStarting =
				GameDB.recommend.FindStartingData(characterCode, characterMasteryData.weapon1);
			if (recommendStarting == null)
			{
				return;
			}

			MatchingService inst = MonoBehaviourInstance<MatchingService>.inst;
			if (inst == null)
			{
				return;
			}

			Character character = Lobby.inst.CharacterList.FirstOrDefault(c => c.characterCode == characterCode);
			int skinCode = character != null ? character.lastSkinCode : 0;
			inst.SelectCharacter(characterCode, recommendStarting.code, skinCode);
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GetComponentsInChildren<LobbyPortraitCharacterCard>(characterCards);
		}


		public void SetCharacter(int index, CharacterData characterData)
		{
			if (characterCards == null)
			{
				return;
			}

			if (index < 0 || characterCards.Count <= index)
			{
				return;
			}

			if (characterData == null)
			{
				characterCards[index].SetCharacterCode(0, false, false, false, false);
				characterCards[index].SetFocusScale(1f);
				characterCards[index].Focus(false);
				characterCards[index].SetListener(null);
				return;
			}

			bool have = Lobby.inst.CharacterList.Exists(x => x.characterCode == characterData.code);
			bool freeRotation = MonoBehaviourInstance<MatchingService>.inst.IsFreeCharacter(characterData.code);
			int num = MonoBehaviourInstance<MatchingService>.inst.IsSelectedCharacter(characterData.code);
			bool flag = MonoBehaviourInstance<MatchingService>.inst.IsPicked(characterData.code);
			characterCards[index].SetCharacterCode(characterData.code, have, freeRotation,
				MonoBehaviourInstance<MatchingService>.inst.IsBanCharacter(characterData.code),
				SingletonMonoBehaviour<KakaoPcService>.inst.BenefitByKakaoPcCafe);
			characterCards[index].SetFocusScale(flag ? 0.9f : 1f);
			characterCards[index].SetFocusColor(num, flag);
			characterCards[index].Focus(0 < num);
			characterCards[index].SetListener(this);
		}
	}
}