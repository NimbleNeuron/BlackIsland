namespace Blis.Common
{
	public static class Extensions
	{
		public static bool IsPlayerObject(this ObjectType obj)
		{
			return obj == ObjectType.PlayerCharacter || obj == ObjectType.BotPlayerCharacter;
		}


		public static bool IsSummonObject(this ObjectType obj)
		{
			return obj == ObjectType.SummonCamera || obj == ObjectType.SummonServant || obj == ObjectType.SummonTrap;
		}
	}
}