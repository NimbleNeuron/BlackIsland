using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class SkillSet
	{
		private readonly List<int> getSkillsByLevelReturnValue = new List<int>();


		private readonly Dictionary<int, List<int>> skillSetMap = new Dictionary<int, List<int>>();


		public void AddActiveSequence(int level, int code)
		{
			if (skillSetMap.ContainsKey(level))
			{
				skillSetMap[level].Add(code);
				return;
			}

			skillSetMap.Add(level, new List<int>
			{
				code
			});
		}


		public List<int> GetSkillsByLevel(int level)
		{
			if (level <= 0)
			{
				level = 1;
			}

			getSkillsByLevelReturnValue.Clear();
			if (skillSetMap.ContainsKey(level))
			{
				getSkillsByLevelReturnValue.AddRange(skillSetMap[level]);
			}

			return getSkillsByLevelReturnValue;
		}


		public int GetSkill(int level, int sequence)
		{
			List<int> skillsByLevel = GetSkillsByLevel(level);
			if (sequence < 0)
			{
				return 0;
			}

			if (skillsByLevel.Count <= sequence)
			{
				return 0;
			}

			return skillsByLevel[sequence];
		}


		public int GetActiveMaxSequence()
		{
			if (0 < skillSetMap.Count)
			{
				return skillSetMap.First<KeyValuePair<int, List<int>>>().Value.Count;
			}

			return 0;
		}


		public bool Any(int skillCode)
		{
			foreach (KeyValuePair<int, List<int>> keyValuePair in skillSetMap)
			{
				using (List<int>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current == skillCode)
						{
							return true;
						}
					}
				}
			}

			return false;
		}


		public int GetSequence(int skillCode)
		{
			foreach (KeyValuePair<int, List<int>> keyValuePair in skillSetMap)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					if (keyValuePair.Value[i].Equals(skillCode))
					{
						return i;
					}
				}
			}

			return -1;
		}
	}
}