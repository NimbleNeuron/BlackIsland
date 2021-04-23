using System.Collections.Generic;

namespace Blis.Server
{
	
	public class SkillScriptParameterCollection
	{
		
		private readonly Dictionary<SkillScriptParameterType, float> dataMap =
			new Dictionary<SkillScriptParameterType, float>(
				SingletonComparerEnum<SkillParameterTypeComparer, SkillScriptParameterType>.Instance);

		
		private SkillScriptParameterCollection() { }

		
		public static SkillScriptParameterCollection Create()
		{
			return new SkillScriptParameterCollection();
		}

		
		public void Clear()
		{
			dataMap.Clear();
		}

		
		public void Add(SkillScriptParameterType type, float value)
		{
			if (!dataMap.ContainsKey(type))
			{
				dataMap.Add(type, 0f);
			}

			Dictionary<SkillScriptParameterType, float> dictionary = dataMap;
			dictionary[type] += value;
		}

		
		public float Get(SkillScriptParameterType type)
		{
			if (!dataMap.ContainsKey(type))
			{
				return 0f;
			}

			return dataMap[type];
		}

		
		public void Merge(SkillScriptParameterCollection parametersCollection)
		{
			if (parametersCollection == null)
			{
				return;
			}

			foreach (KeyValuePair<SkillScriptParameterType, float> keyValuePair in parametersCollection.dataMap)
			{
				Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}
}