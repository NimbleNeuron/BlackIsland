using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class CharacterStateDB
	{
		public const int MonsterResetState = 10000;


		public const int RestStateCode = 10001;


		public const int RegenStateCode = 10002;


		public const int SpRegenStateCode = 10003;


		public const int DayNightStateCode = 10004;


		public const int SightShare = 10005;


		public const int StealthSelf = 10006;


		public const int EvasionNormalAttack = 10007;


		public const int Untargetable = 10008;


		public const int DyingCondition = 10009;


		public const int SoloModeStateCode = 11000;


		public const int DuoModeStateCode = 11001;


		public const int TrioModeStateCode = 11002;


		public const int WicklineKillBuffGroup = 5000000;


		public const int DecreaseRecovery = 5001001;


		private const int CrowdControlStateCode = 2000000;


		public const int AirborneCode = 2000001;


		public const int FearCode = 2000003;


		public const int TauntCode = 2000004;


		public const int CharmCode = 2000005;


		public const int SilenceCode = 2000006;


		public const int BlockedSightCode = 2000007;


		public const int FetterCode = 2000008;


		public const int StunCode = 2000009;


		public const int KnockbackCode = 2000010;


		public const int SleepCode = 2000011;


		public const int DisarmedCode = 2000012;


		public const int SuppressedCode = 2000013;


		public const int BlindCode = 2000014;


		public const int GroundingCode = 2000015;


		public const int GrabCode = 2000016;


		public const int UninteractionableCode = 2000017;


		public const int DanceCode = 1008511;


		public static readonly Dictionary<NoiseType, int> NoiseIgnoreCode = new Dictionary<NoiseType, int>
		{
			{
				NoiseType.BasicHit,
				12001
			},
			{
				NoiseType.Gunshot,
				12002
			},
			{
				NoiseType.FixedBoxOpen,
				12003
			},
			{
				NoiseType.AirSupplyOpen,
				12004
			},
			{
				NoiseType.CollectibleOpen,
				12005
			},
			{
				NoiseType.TrapHit,
				12006
			},
			{
				NoiseType.MonsterKilled,
				12007
			},
			{
				NoiseType.PlayerKilled,
				12008
			},
			{
				NoiseType.Crafting,
				12009
			},
			{
				NoiseType.FootstepSoil,
				12010
			},
			{
				NoiseType.FootstepWater,
				12011
			},
			{
				NoiseType.FootstepWood,
				12012
			},
			{
				NoiseType.FootstepConcrete,
				12013
			},
			{
				NoiseType.FootstepMetal,
				12014
			},
			{
				NoiseType.FootstepAsphalt,
				12015
			},
			{
				NoiseType.FootstepGrass,
				12016
			},
			{
				NoiseType.HyperLoopExit,
				12017
			}
		};


		public readonly Dictionary<int, CharacterStateGroupData> stateGroupMap =
			new Dictionary<int, CharacterStateGroupData>();


		public readonly Dictionary<int, CharacterStateData> stateMap = new Dictionary<int, CharacterStateData>();

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(CharacterStateGroupData))
			{
				stateGroupMap.Clear();
				using (List<CharacterStateGroupData>.Enumerator enumerator = data.Cast<CharacterStateGroupData>()
					.ToList<CharacterStateGroupData>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CharacterStateGroupData characterStateGroupData = enumerator.Current;
						stateGroupMap.Add(characterStateGroupData.group, characterStateGroupData);
					}

					return;
				}
			}

			if (typeFromHandle == typeof(CharacterStateData))
			{
				stateMap.Clear();
				foreach (CharacterStateData characterStateData in data.Cast<CharacterStateData>()
					.ToList<CharacterStateData>())
				{
					stateMap.Add(characterStateData.code, characterStateData);
				}
			}
		}


		public CharacterStateGroupData GetGroupData(int group)
		{
			if (!stateGroupMap.ContainsKey(group))
			{
				throw new Exception();
			}

			return stateGroupMap[group];
		}


		public CharacterStateData GetData(int code)
		{
			if (!stateMap.ContainsKey(code))
			{
				throw new Exception();
			}

			return stateMap[code];
		}


		public CharacterStateData GetData(int group, int level)
		{
			foreach (KeyValuePair<int, CharacterStateData> keyValuePair in stateMap)
			{
				if (keyValuePair.Value.group == group && keyValuePair.Value.level == level)
				{
					return keyValuePair.Value;
				}
			}

			return null;
		}
	}
}