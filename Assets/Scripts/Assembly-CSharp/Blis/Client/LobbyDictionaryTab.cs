using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LobbyDictionaryTab : LobbyTabBaseUI, ILobbyTab
	{
		[SerializeField] private SlimCharacterSelectionView characterSelectionView = default;


		[SerializeField] private LobbyItemBookView lobbyItemBookView = default;


		private int currentCharacterCode;


		public void OnOpen(LobbyTab from)
		{
			EnableCanvas(true);
			Singleton<ItemService>.inst.SetLevelData(GameDB.level.DefaultLevel);
			SelectCharacter(currentCharacterCode == 0 ? GameConstants.LobbyInitialCharacter : currentCharacterCode);
		}


		public TabCloseResult OnClose(LobbyTab to)
		{
			EnableCanvas(false);
			return TabCloseResult.Success;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			characterSelectionView.OnCharacterSelected += SelectCharacter;
			lobbyItemBookView.SetDraggable(false);
		}


		private void SelectCharacter(int characterCode)
		{
			if (currentCharacterCode != characterCode)
			{
				currentCharacterCode = characterCode;
				CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(characterCode);
				lobbyItemBookView.SetWeaponTypes((from x in characterMasteryData.GetMasteries()
					select x.GetWeaponType()).ToArray<WeaponType>());
				characterSelectionView.FocusCharacter(characterCode);
				characterSelectionView.FocusCharacterPosition(characterCode);
			}
		}
	}
}