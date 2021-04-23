using System.Collections.Generic;
using System.Linq;
using Blis.Common;

namespace Blis.Server
{
	
	public class PlayerCharacterService : ServiceBase
	{
		
		
		private IEnumerable<WorldPlayerCharacter> PlayerCharacters
		{
			get
			{
				return this.world.FindAll<WorldPlayerCharacter>();
			}
		}

		
		
		public int HighestPlayerLevel
		{
			get
			{
				return this.highestPlayerLevel;
			}
		}

		
		
		public float AvgMMR
		{
			get
			{
				return this.avgMMR;
			}
		}

		
		
		public int minMM
		{
			get
			{
				return this.minMMR;
			}
		}

		
		public int GetTopPlayerLevel()
		{
			return this.PlayerCharacters.Max((WorldPlayerCharacter playerCharacter) => playerCharacter.Status.Level);
		}

		
		public void StartMovingDistanceMeasurement()
		{
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.PlayerCharacters)
			{
				worldPlayerCharacter.StartMovingDistanceMeasurement();
			}
		}

		
		public void UpdateMasteryWhenGameStarted()
		{
			Dictionary<int, UpdateMasteryInfo> dictionary = new Dictionary<int, UpdateMasteryInfo>();
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.PlayerCharacters)
			{
				UpdateMasteryInfo updateMasteryInfo = this.game.Area.CheckFirstVisitArea(worldPlayerCharacter);
				if (updateMasteryInfo != null)
				{
					worldPlayerCharacter.AddMasteryConditionExp(updateMasteryInfo);
					dictionary.Add(worldPlayerCharacter.ObjectId, updateMasteryInfo);
				}
			}
		}

		
		public void Init()
		{
			if (!this.PlayerCharacters.Any<WorldPlayerCharacter>())
			{
				Log.W("[PlayerCharacterService] PlayerCharacters is Empty");
				return;
			}
			List<WorldPlayerCharacter> source = (from c in this.PlayerCharacters
			where !c.IsAI
			select c).ToList<WorldPlayerCharacter>();
			if (!source.Any<WorldPlayerCharacter>())
			{
				Log.W("[PlayerCharacterService] PlayerCharacters is Empty");
				return;
			}
			this.minMMR = source.Min((WorldPlayerCharacter character) => character.MMRContext.mmrBefore);
			this.avgMMR = (float)this.PlayerCharacters.Average(delegate(WorldPlayerCharacter c)
			{
				if (!c.IsAI)
				{
					return c.MMRContext.mmrBefore;
				}
				return this.minMMR;
			});
		}

		
		public void UpdateHighestPlayerLevel(WorldPlayerCharacter playerCharacter)
		{
			if (this.highestPlayerLevel < playerCharacter.Status.Level)
			{
				this.highestPlayerLevel = playerCharacter.Status.Level;
			}
		}

		
		public int GetLowestMmrInSurvivors()
		{
			int num = int.MinValue;
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.PlayerCharacters)
			{
				if (worldPlayerCharacter.IsAlive)
				{
					MMRContext mmrcontext = worldPlayerCharacter.MMRContext;
					int num2 = (mmrcontext != null) ? mmrcontext.mmrBefore : 0;
					if (num < 0)
					{
						if (num < num2)
						{
							num = num2;
						}
					}
					else if (num2 < num)
					{
						num = num2;
					}
				}
			}
			if (num >= 0)
			{
				return num;
			}
			return this.game.Player.GetCurFrameDeadPlayerLowestMmr();
		}

		
		public void SendStartEmotion()
		{
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.PlayerCharacters)
			{
				if (worldPlayerCharacter.IsAlive)
				{
					worldPlayerCharacter.SendEmotionIcon(EmotionPlateType.START);
				}
			}
		}

		
		private int highestPlayerLevel = 1;

		
		private float avgMMR;

		
		private int minMMR;
	}
}
