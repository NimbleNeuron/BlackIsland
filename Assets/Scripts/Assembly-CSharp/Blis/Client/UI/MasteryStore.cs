using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client.UI
{
	[UIActionMapping(typeof(UpdateMastery))]
	public class MasteryStore : UIStore<MasteryStore>
	{
		public enum MasteryUpdateType
		{
			None,

			LevelUp,

			ExpUp
		}


		private readonly Dictionary<MasteryType, Mastery> masteries =
			new Dictionary<MasteryType, Mastery>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private readonly List<Mastery> updatedCache = new List<Mastery>();


		public Dictionary<MasteryType, Mastery>.ValueCollection GetMasteries => masteries.Values;


		public List<Mastery> GetUpdatedMasteries { get; } = new List<Mastery>();


		protected override void ActionHandle(UIAction action)
		{
			action.IfTypeIs<UpdateMastery>(delegate(UpdateMastery data)
			{
				MasteryUpdateType masteryUpdateType = MasteryUpdateType.None;
				if (masteries.ContainsKey(data.masteryType))
				{
					masteryUpdateType = masteries[data.masteryType].level < data.level
						? MasteryUpdateType.LevelUp
						: MasteryUpdateType.ExpUp;
				}

				masteries[data.masteryType] = new Mastery
				{
					masteryUpdateType = masteryUpdateType,
					masteryType = data.masteryType,
					level = data.level,
					exp = data.exp,
					maxExp = data.maxExp
				};
				updatedCache.Add(masteries[data.masteryType]);
			});
		}


		protected override void PreCommit()
		{
			GetUpdatedMasteries.Clear();
			GetUpdatedMasteries.AddRange(updatedCache);
			updatedCache.Clear();
		}


		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			masteries.Clear();
			GetUpdatedMasteries.Clear();
			updatedCache.Clear();
		}


		public struct Mastery
		{
			public MasteryUpdateType masteryUpdateType;


			public MasteryType masteryType;


			public int level;


			public int exp;


			public int maxExp;
		}
	}
}