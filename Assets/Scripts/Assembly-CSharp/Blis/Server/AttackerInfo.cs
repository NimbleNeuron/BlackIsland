namespace Blis.Server
{
	
	public class AttackerInfo
	{
		
		private static readonly SimpleCharacterStat emptyStat = new SimpleCharacterStat();

		
		public static readonly AttackerInfo Empty = new AttackerInfo();

		
		private WorldCharacter attacker;

		
		private SimpleCharacterStat cachedStat;

		
		
		public WorldCharacter Attacker => attacker;

		
		
		public SimpleCharacterStat CachedStat {
			get
			{
				if (cachedStat != null)
				{
					return cachedStat;
				}

				return emptyStat;
			}
		}

		
		public void SetAttackerStat(WorldCharacter attacker, SimpleCharacterStat attackerStat)
		{
			this.attacker = attacker;
			if (attacker == null)
			{
				cachedStat = null;
				return;
			}

			if (attackerStat != null)
			{
				attackerStat.CopyStats(attacker.Stat);
			}

			cachedStat = attackerStat;
		}
	}
}