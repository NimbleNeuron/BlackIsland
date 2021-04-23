namespace Blis.Common
{
	public class CrowdControlData
	{
		public readonly bool blockedByPlayingState;


		public readonly bool cancelPlayingSkill;


		public readonly StateType newStateType;


		public readonly bool pausePlayingSkill;


		public readonly StateType playingStateType;

		public CrowdControlData(StateType newStateType, StateType playingStateType, bool blockedByPlayingState,
			bool cancelPlayingSkill, bool pausePlayingSkill)
		{
			this.newStateType = newStateType;
			this.playingStateType = playingStateType;
			this.blockedByPlayingState = blockedByPlayingState;
			this.cancelPlayingSkill = cancelPlayingSkill;
			this.pausePlayingSkill = pausePlayingSkill;
		}
	}
}