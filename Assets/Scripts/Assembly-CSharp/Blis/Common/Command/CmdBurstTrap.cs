using System.Collections.Generic;
using Blis.Client;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdBurstTrap, false)]
	public class CmdBurstTrap : LocalSummonTrapCommandPacket
	{
		[Key(1)] public List<int> targets;


		public override void Action(ClientService service, LocalSummonTrap self)
		{
			self.OnBurst();
			foreach (int objectId in targets)
			{
				GameObject resource = self.LoadEffect(self.SummonData.targetHitEffect);
				LocalCharacter localCharacter = service.World.Find<LocalCharacter>(objectId);
				if (localCharacter != null)
				{
					localCharacter.PlayLocalEffectPoint(resource, Vector3.zero);
				}
			}
		}
	}
}