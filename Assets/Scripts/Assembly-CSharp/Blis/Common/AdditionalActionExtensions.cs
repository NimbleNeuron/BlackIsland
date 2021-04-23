namespace Blis.Common
{
	
	public static class AdditionalActionExtensions
	{
		
		public static bool CanInputReleaseKey(this AdditionalAction additionalAction)
		{
			return additionalAction > AdditionalAction.AgainPressNoQuickCast &&
			       additionalAction - AdditionalAction.AgainPress <= 1;
		}
	}
}