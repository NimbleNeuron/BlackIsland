using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdLookAt))]
	[Union(1, typeof(CmdStartSkill))]
	[Union(2, typeof(CmdStartStateSkill))]
	[Union(3, typeof(CmdPlayPassiveSkill))]
	[Union(4, typeof(CmdPlaySkillAction))]
	[Union(5, typeof(CmdFinishSkill))]
	[Union(6, typeof(CmdDestroy))]
	[MessagePackObject()]
	public abstract class LocalObjectCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalObject>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalObject self);
	}
}