using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcUpdateBeforeStart, false)]
	public class RpcUpdateBeforeStart : RpcPacket
	{
		
		[Key(2)] public Dictionary<SkillSlotIndex, int> characterSkillLevels;

		
		[Key(1)] public List<InvenItem> inventoryItems;

		
		[Key(3)] public List<MasteryValue> masteryValues;

		
		[Key(0)] public int skillPoint;

		
		public override void Action(ClientService clientService)
		{
			MyPlayerContext myPlayer = clientService.MyPlayer;
			if (myPlayer != null)
			{
				myPlayer.Character.OnUpdateInventory(inventoryItems);
			}

			MyPlayerContext myPlayer2 = clientService.MyPlayer;
			if (myPlayer2 != null)
			{
				myPlayer2.UpdateSkills(characterSkillLevels, skillPoint);
			}

			MyPlayerContext myPlayer3 = clientService.MyPlayer;
			if (myPlayer3 == null)
			{
				return;
			}

			myPlayer3.OnUpdateMastery(masteryValues, false);
		}
	}
}