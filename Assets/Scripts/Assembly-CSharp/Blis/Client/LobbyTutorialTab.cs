using UnityEngine;

namespace Blis.Client
{
	public class LobbyTutorialTab : LobbyTabBaseUI, ILobbyTab
	{
		[SerializeField] private TutorialPage tutorialPage = default;


		public void OnOpen(LobbyTab from)
		{
			EnableCanvas(true);
			tutorialPage.OpenPage();
		}


		public TabCloseResult OnClose(LobbyTab to)
		{
			EnableCanvas(false);
			return TabCloseResult.Success;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
		}
	}
}