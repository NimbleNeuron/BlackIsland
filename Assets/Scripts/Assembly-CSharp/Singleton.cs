using System;


public class Singleton<T> where T : Singleton<T>, new()
{
	
	
	
	public static T inst { get; private set; }

	
	protected virtual void OnClear()
	{
	}

	
	protected virtual void OnCreated()
	{
	}

	
	static Singleton()
	{
		if (Singleton<T>.inst == null)
		{
			Singleton<T>.inst = Activator.CreateInstance<T>();
			Singleton<T>.inst.OnCreated();
		}
	}

	
	public void Clear()
	{
		this.OnClear();
		Singleton<T>.inst = default(T);
		Singleton<T>.inst = Activator.CreateInstance<T>();
		Singleton<T>.inst.OnCreated();
	}
}
