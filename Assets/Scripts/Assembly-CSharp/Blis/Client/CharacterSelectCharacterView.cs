using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class CharacterSelectCharacterView : BaseUI
	{
		private CharacterSelectMyCharacterView myCharacterView;


		private CharacterSelectTeamCharacterView teamCharacterView;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			myCharacterView = GameUtil.Bind<CharacterSelectMyCharacterView>(gameObject, "MyCharacterView");
			teamCharacterView = GameUtil.Bind<CharacterSelectTeamCharacterView>(gameObject, "TeamView");
		}


		public void Clear()
		{
			myCharacterView.Clear();
			teamCharacterView.Clear();
		}


		public void UpdateMyCharacter(Sprite characterSprite, string characterName)
		{
			myCharacterView.SetCharacter(characterSprite, characterName);
		}


		public void PickMyCharacter()
		{
			myCharacterView.PickCharacter();
		}


		public void CancelPickMyCharacter()
		{
			myCharacterView.CancelPickCharacter();
		}


		public void UpdateMyTeam(MatchingService.MatchingUser userInfo)
		{
			teamCharacterView.SetTeamSlot(userInfo);
		}


		public void PickCharacter(MatchingService.MatchingUser userInfo)
		{
			teamCharacterView.PickCharacter(userInfo);
		}
	}
}