namespace Blis.Client
{
	public interface ITeamMemberSlotEventListener
	{
		void OnSlotLeftClick(TeamMemberSlot slot);


		void OnSlotDoubleClick(TeamMemberSlot slot);


		void OnSlotRightClick(TeamMemberSlot slot);


		void OnPointerDown(TeamMemberSlot slot);


		void OnPointerUp(TeamMemberSlot slot);


		void OnPointerEnter(TeamMemberSlot slot);


		void OnPointerExit(TeamMemberSlot slot);
	}
}