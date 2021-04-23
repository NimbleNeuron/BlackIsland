using MessagePack;

namespace Blis.Common
{
	[MessagePackObject()]
	public class HideNameSetting
	{
		[Key(0)] public bool hideNameFromEnemy;

		public HideNameSetting(bool hideNameFromEnemy)
		{
			this.hideNameFromEnemy = hideNameFromEnemy;
		}
	}
}