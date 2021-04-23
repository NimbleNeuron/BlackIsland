using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class ServiceBase : MonoBehaviour
	{
		
		protected readonly GameServer server = MonoBehaviourInstance<GameService>.inst.Server;

		
		protected readonly GameWorld world = MonoBehaviourInstance<GameService>.inst.World;

		
		protected readonly GameService game = MonoBehaviourInstance<GameService>.inst;
	}
}
