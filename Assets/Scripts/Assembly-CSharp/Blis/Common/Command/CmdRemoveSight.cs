using System.Collections.Generic;
using Blis.Client;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdRemoveSight, false)]
	public class CmdRemoveSight : LocalCharacterCommandPacket
	{
		[Key(1)] public int attachSightId;


		[Key(2)] public int targetId;


		public override void Action(ClientService service, LocalCharacter self)
		{
			List<LocalSightAgent> attachedSights = service.World.Find<LocalObject>(targetId).attachedSights;
			if (attachedSights != null)
			{
				for (int i = 0; i < attachedSights.Count; i++)
				{
					if (attachedSights[i].AttachSightId == attachSightId)
					{
						Object.Destroy(attachedSights[i]);
						return;
					}
				}
			}
		}
	}
}