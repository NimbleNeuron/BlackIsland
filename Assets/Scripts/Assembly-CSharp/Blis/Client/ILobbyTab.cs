namespace Blis.Client
{
	public interface ILobbyTab
	{
		void OnOpen(LobbyTab from);


		TabCloseResult OnClose(LobbyTab to);
	}
}