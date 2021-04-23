using System;
using System.Collections.Generic;


public abstract class SingletonComparerEnum<T, TCompare> : IEqualityComparer<TCompare>
	where T : class, new() where TCompare : Enum
{
	static SingletonComparerEnum()
	{
		if (Instance == null)
		{
			Instance = Activator.CreateInstance<T>();
		}
	}
	
	public static T Instance { get; }

	
	public abstract bool Equals(TCompare x, TCompare y);

	
	public abstract int GetHashCode(TCompare obj);
}