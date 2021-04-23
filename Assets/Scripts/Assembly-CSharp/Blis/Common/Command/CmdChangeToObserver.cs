using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdChangeToObserver, false)]
	public class CmdChangeToObserver : CommandPacket
	{
		
		[Key(5)] public SnapshotWrapper characterSnapshot;

		
		[Key(4)] public List<SimpleMonsterInfo> monsterList;

		
		[Key(1)] public string nickname;

		
		[Key(6)] public byte[] playerSnapshot;

		
		[Key(3)] public List<SimpleSummonInfo> summonList;

		
		[Key(0)] public long userId;

		
		[Key(2)] public List<SimpleUserInfo> userList;

		
		public override void Action(ClientService clientService)
		{
			LocalPlayerCharacter character = clientService.MyPlayer.Character;
			clientService.ConvertMyPlayerContextToPlayerContext();
			clientService.CreateMyObserver(userId, nickname, playerSnapshot, characterSnapshot);
			clientService.MyObserver.Observer.HostileAgent.SelectTeamNumber(character.TeamNumber);
			clientService.MyObserver.Observer.HostileAgent.SelectTeamNumber(character.TeamSlot);
			foreach (SimpleUserInfo simpleUserInfo in userList)
			{
				PlayerContext playerContext;
				if (simpleUserInfo.playerSnapshot != null &&
				    (playerContext = clientService.FindPlayerContext(simpleUserInfo.userId)) != null)
				{
					playerContext.Init(simpleUserInfo.playerSnapshot);
					LocalPlayerCharacter character2 = playerContext.Character;
					character2.SetPosition(simpleUserInfo.position.ToVector3());
					character2.OnVisible();
					character2.InSight();
					character2.MoveAgent.ApplySnapshot(simpleUserInfo.moveAgentSnapshot, clientService.World);
					character2.FogHiderOnCenter.SetIgnore(false);
				}
			}

			foreach (SimpleSummonInfo simpleSummonInfo in summonList)
			{
				LocalSummonBase localSummonBase = clientService.World.Find<LocalSummonBase>(simpleSummonInfo.objectId);
				localSummonBase.SetPosition(simpleSummonInfo.position.ToVector3());
				localSummonBase.OnVisible();
				localSummonBase.InSight();
				localSummonBase.FogHiderOnCenter.SetIgnore(false);
				ILocalMoveAgentOwner localMoveAgentOwner = localSummonBase as ILocalMoveAgentOwner;
				if (localMoveAgentOwner != null)
				{
					localMoveAgentOwner.MoveAgent.ApplySnapshot(simpleSummonInfo.moveAgentSnapshot,
						clientService.World);
				}
			}

			foreach (SimpleMonsterInfo simpleMonsterInfo in monsterList)
			{
				LocalMonster localMonster = clientService.World.Find<LocalMonster>(simpleMonsterInfo.objectId);
				localMonster.SetPosition(simpleMonsterInfo.position.ToVector3());
				localMonster.InSight();
				localMonster.MoveAgent.ApplySnapshot(simpleMonsterInfo.moveAgentSnapshot, clientService.World);
				localMonster.FogHiderOnCenter.SetIgnore(false);
				localMonster.ClientCharacter.SetStealth(false);
				localMonster.ClientCharacter.SetVisible(true);
			}

			GlobalUserData.IsPlayer = false;
			clientService.BuildTeam();
			clientService.MyObserver.ShowObserverUI(character);
		}
	}
}