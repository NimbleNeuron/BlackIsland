using System.Collections.Generic;

namespace Blis.Common
{
	public interface AuthToken
	{
		string GetPlayerId();


		Dictionary<string, string> GetAttributesMap();
	}
}