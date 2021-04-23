using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LocalEmmaSkillScript : LocalSkillScript
	{
		public enum EmmaPickingOrder
		{
			None,

			Pigeon,

			FireworkHat,

			Rabbit
		}

		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		protected static bool IsPigeon(LocalSummonBase summon)
		{
			return summon.SummonData.code == Singleton<EmmaSkillActive1Data>.inst.PigeonSummonCode;
		}


		protected static List<LocalSummonServant> GetPigeons(LocalPlayerCharacter owner)
		{
			List<LocalSummonBase> ownSummons = owner.GetOwnSummons(IsPigeon);
			if (ownSummons == null || ownSummons.Count <= 0)
			{
				return null;
			}

			List<LocalSummonServant> list = new List<LocalSummonServant>();
			foreach (LocalSummonBase localSummonBase in ownSummons)
			{
				list.Add(localSummonBase as LocalSummonServant);
			}

			return list;
		}


		protected static bool IsFireworkHat(LocalSummonBase summon)
		{
			return summon.SummonData.code == Singleton<EmmaSkillActive2Data>.inst.FireworkHatSummonCode;
		}


		protected static List<LocalSummonTrap> GetFireworkHats(LocalPlayerCharacter owner)
		{
			List<LocalSummonBase> ownSummons = owner.GetOwnSummons(IsFireworkHat);
			if (ownSummons == null || ownSummons.Count <= 0)
			{
				return null;
			}

			List<LocalSummonTrap> list = new List<LocalSummonTrap>();
			foreach (LocalSummonBase localSummonBase in ownSummons)
			{
				list.Add(localSummonBase as LocalSummonTrap);
			}

			return list;
		}


		protected void ShowPigeonsIndicator(List<LocalSummonServant> pigeons)
		{
			if (pigeons == null)
			{
				return;
			}

			foreach (LocalSummonServant localSummonServant in pigeons)
			{
				if (localSummonServant.HasIndicator())
				{
					localSummonServant.ShowIndicatorRenderers();
				}
				else
				{
					localSummonServant.AddIndicator(SkillData);
				}
			}
		}


		protected void HidePigeonsIndicator(List<LocalSummonServant> pigeons)
		{
			if (pigeons == null)
			{
				return;
			}

			foreach (LocalSummonServant localSummonServant in pigeons)
			{
				if (localSummonServant != null && localSummonServant.HasIndicator())
				{
					localSummonServant.HideIndicatorRenderers();
				}
			}
		}


		protected bool IsMySummon(LocalSummonBase summon)
		{
			return summon.Owner == LocalPlayerCharacter;
		}
	}
}