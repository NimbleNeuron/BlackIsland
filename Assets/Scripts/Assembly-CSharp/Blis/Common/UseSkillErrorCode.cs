namespace Blis.Common
{
	public enum UseSkillErrorCode
	{
		None = -1,
		NotAvailableNow,
		NotLearn,
		NotReady,
		NotEnoughStamina,
		NotEnoughExCost,
		Resting,
		Disarmed,
		InvalidAction,
		NotInvalidTarget,
		NoMessageError = 9999
	}
}