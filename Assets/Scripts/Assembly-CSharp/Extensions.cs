using System.Linq;


public static class Extensions
{
	
	public static T[] Splice<T>(this T[] source, int start)
	{
		return source.ToList<T>().Skip(start).ToList<T>().ToArray<T>();
	}
}