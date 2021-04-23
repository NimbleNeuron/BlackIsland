using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class ShoichiSkillScript : SkillScript
	{
		
		protected void CreatePassiveDagger(Vector3 destination, Vector3 direction, float distance = 1f)
		{
			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			if (worldPlayerCharacter == null)
			{
				return;
			}

			Vector3 position = destination + distance * direction;
			int walkableNavMask = Caster.Character.WalkableNavMask;
			Vector3 vector;
			if (!MoveAgent.CanStandToPosition(position, walkableNavMask, 2f, out vector))
			{
				if (MoveAgent.SamplePosition(position, walkableNavMask, out vector))
				{
					position = vector;
				}
			}
			else
			{
				position = vector;
			}

			MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter,
				Singleton<ShoichiSkillPassiveData>.inst.PassiveSummonObjectId, position);
		}

		
		protected void CreatePassiveDagger(Vector3 finalDestination)
		{
			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			if (worldPlayerCharacter == null)
			{
				return;
			}

			int walkableNavMask = Caster.Character.WalkableNavMask;
			Vector3 vector;
			if (!MoveAgent.CanStandToPosition(finalDestination, walkableNavMask, 2f, out vector))
			{
				if (MoveAgent.SamplePosition(finalDestination, walkableNavMask, out vector))
				{
					finalDestination = vector;
				}
			}
			else
			{
				finalDestination = vector;
			}

			MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter,
				Singleton<ShoichiSkillPassiveData>.inst.PassiveSummonObjectId, finalDestination);
		}
	}
}