using System;
using System.Collections.Generic;


public abstract class SingletonComparerStruct<T, TCompare> : IEqualityComparer<TCompare> where T : class, new() where TCompare : struct
{
	
	
	
	public static T Instance { get; private set; }

	
	static SingletonComparerStruct()
	{
		if (SingletonComparerStruct<T, TCompare>.Instance == null)
		{
			SingletonComparerStruct<T, TCompare>.Instance = Activator.CreateInstance<T>();
		}
	}

	
	public abstract bool Equals(TCompare x, TCompare y);

	
	public abstract int GetHashCode(TCompare obj);
}
