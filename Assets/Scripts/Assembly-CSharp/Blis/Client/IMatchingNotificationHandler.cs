using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client
{
	public interface IMatchingNotificationHandler
	{
		void OnStartMatching();


		void OnStopMatching();


		void OnCompleteMatching();


		void OnExitMatchingOtherUser();


		void OnOtherMemberNotJoinMatching();


		void OnChatMessage(MatchingChatMessage msg);


		void OnCharacterSelectPhase(List<MatchingTeamMember> teamMembers, List<long> freeCharacterList,
			List<long> banCharacterList);


		void OnObserverCharacterSelectPhase(List<MatchingTeam> matchingTeamList);


		void OnCharacterSelect(long userNum, int characterCode, int startingDataCode, int skinCode);


		void OnSkinSelect(long userNum, int skinCode);


		void OnCharacterPick(long userNum, int skinCode);


		void OnCancelCharacterPick(long cancelUserNum);


		void OnSelectSkinPhase();


		void OnDodgeMatching();


		void OnKickOutMatching();


		void OnAcceptMatching();


		void OnKickOutDeclineMatching();


		void OnStandBy();


		void OnStartGame(MatchingResult matchingResult);


		void OnStartCustomGame(MatchingResult matchingResult);


		void OnUpdateCustomGameRoom(CustomGameRoom customGameRoom);


		void OnCustomGameRoomChatMessage(MatchingChatMessage message);


		void OnKickedOutCustomGameRoom();


		void OnDodgeCustomMatching();


		void OnCustomGameRoomEmpty();
	}
}