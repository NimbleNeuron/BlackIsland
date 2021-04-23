using System.Collections.Generic;
using System.Reflection;


public class AttributeHandler<Tkey, Tinterface, Tattribute>
{
	
	private readonly Dictionary<Tkey, Tinterface> handlers;

	
	public AttributeHandler()
	{
		handlers = new Dictionary<Tkey, Tinterface>();
		Assembly.GetAssembly(typeof(Tinterface));
	}
}