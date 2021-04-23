using System.Collections.Generic;

namespace Blis.Common
{
	public class SkillSequencer
	{
		private readonly Dictionary<SkillSlotSet, SkillSequence> sequenceMap =
			new Dictionary<SkillSlotSet, SkillSequence>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>
				.Instance);

		public SkillSequencerSnapshot CreateSnapshot()
		{
			SkillSequencerSnapshot skillSequencerSnapshot = new SkillSequencerSnapshot();
			foreach (KeyValuePair<SkillSlotSet, SkillSequence> keyValuePair in sequenceMap)
			{
				skillSequencerSnapshot.sequenceMap.Add(keyValuePair.Key, new SkillSequenceSnapshot
				{
					currentSequence = keyValuePair.Value.Current(),
					maxSequence = keyValuePair.Value.MaxSequence
				});
			}

			return skillSequencerSnapshot;
		}


		public void Init(SkillSequencerSnapshot snapshot)
		{
			if (snapshot == null)
			{
				return;
			}

			sequenceMap.Clear();
			foreach (KeyValuePair<SkillSlotSet, SkillSequenceSnapshot> keyValuePair in snapshot.sequenceMap)
			{
				sequenceMap.Add(keyValuePair.Key, new SkillSequence(keyValuePair.Value));
			}
		}


		public void UpdateSkillSequence(SkillSlotSet skillSlotSet, int maxSequence)
		{
			sequenceMap[skillSlotSet] = new SkillSequence(maxSequence);
		}


		public int GetSequence(SkillSlotSet skillSlotSet)
		{
			if (!sequenceMap.ContainsKey(skillSlotSet))
			{
				return 0;
			}

			return sequenceMap[skillSlotSet].Current();
		}


		public void Next(SkillSlotSet skillSlotSet)
		{
			if (!sequenceMap.ContainsKey(skillSlotSet))
			{
				return;
			}

			sequenceMap[skillSlotSet].Next();
		}


		public void SetSequence(SkillSlotSet skillSlotSet, int seq)
		{
			if (!sequenceMap.ContainsKey(skillSlotSet))
			{
				return;
			}

			sequenceMap[skillSlotSet].Set(seq);
		}


		public void Reset(SkillSlotSet skillSlotSet)
		{
			if (!sequenceMap.ContainsKey(skillSlotSet))
			{
				return;
			}

			sequenceMap[skillSlotSet].Reset();
		}


		public void ResetAll()
		{
			foreach (KeyValuePair<SkillSlotSet, SkillSequence> keyValuePair in sequenceMap)
			{
				keyValuePair.Value.Reset();
			}
		}


		public bool IsSingleSequence(SkillSlotSet skillSlotSet)
		{
			return 1 == sequenceMap[skillSlotSet].MaxSequence;
		}


		public bool IsLastSequence(SkillSlotSet skillSlotSet)
		{
			return sequenceMap[skillSlotSet].MaxSequence - 1 <= GetSequence(skillSlotSet);
		}


		public int GetMaxSequence(SkillSlotSet skillSlotSet)
		{
			return sequenceMap[skillSlotSet].MaxSequence;
		}


		public List<SkillSlotSet> GetPlayingSkillSlotSets()
		{
			List<SkillSlotSet> list = new List<SkillSlotSet>();
			foreach (KeyValuePair<SkillSlotSet, SkillSequence> keyValuePair in sequenceMap)
			{
				if (keyValuePair.Value.IsPlaying())
				{
					list.Add(keyValuePair.Key);
				}
			}

			return list;
		}
	}
}