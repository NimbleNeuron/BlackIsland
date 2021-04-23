using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class MMRContext
	{
		
		
		public float DuelRating
		{
			get
			{
				return this.duelRating;
			}
		}

		
		
		public float MatchRating
		{
			get
			{
				return this.matchRating;
			}
		}

		
		public MMRContext(WorldPlayerCharacter worldPlayerCharacter, int mmr)
		{
			this.self = worldPlayerCharacter;
			this.mmrBefore = mmr;
		}

		
		private float getExpectedScore(float mmrTarget)
		{
			return 1f / (1f + Mathf.Pow(10f, (mmrTarget - (float)this.mmrBefore) / 800f));
		}

		
		public void OnKill(WorldPlayerCharacter targetPlayerCharacter)
		{
			if (targetPlayerCharacter == null)
			{
				return;
			}
			float num = 1.3f - this.getExpectedScore((float)targetPlayerCharacter.MMRContext.mmrBefore);
			this.duelRating += num;
			Log.V("[DuelRating] {5} | before: {0}, Target: {1}, target: {2}, delta: {3}, after: {4}", new object[]
			{
				this.mmrBefore,
				targetPlayerCharacter.PlayerSession.nickname,
				targetPlayerCharacter.MMRContext.mmrBefore,
				num,
				this.duelRating,
				this.self.PlayerSession.nickname
			});
		}

		
		public void OnDeath(WorldCharacter finishingAttacker)
		{
			int lowestMmrInSurvivors;
			if (finishingAttacker == null || !finishingAttacker.IsTypeOf<WorldPlayerCharacter>())
			{
				lowestMmrInSurvivors = MonoBehaviourInstance<GameService>.inst.PlayerCharacter.GetLowestMmrInSurvivors();
			}
			else
			{
				lowestMmrInSurvivors = ((WorldPlayerCharacter)finishingAttacker).MMRContext.mmrBefore;
			}
			float expectedScore = this.getExpectedScore((float)lowestMmrInSurvivors);
			this.duelRating -= expectedScore;
			Log.V("[DuelRating] {4} | before: {0}, KillerMMR: {1}, delta: -{2}, after: {3}", new object[]
			{
				this.mmrBefore,
				lowestMmrInSurvivors,
				expectedScore,
				this.duelRating,
				this.self.PlayerSession.nickname
			});
		}

		
		public float getDuelRating()
		{
			return this.duelRating;
		}

		
		public void SetMatchRating(float matchRating)
		{
			this.matchRating = matchRating;
		}

		
		public readonly WorldPlayerCharacter self;

		
		public readonly int mmrBefore;

		
		private float duelRating;

		
		private float matchRating;
	}
}
