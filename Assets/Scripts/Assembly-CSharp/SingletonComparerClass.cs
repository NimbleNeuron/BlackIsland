using System;
using System.Collections.Generic;


public abstract class SingletonComparerClass<T, TCompare> : IEqualityComparer<TCompare>
	where T : class, new() where TCompare : class
{
	
	static SingletonComparerClass()
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