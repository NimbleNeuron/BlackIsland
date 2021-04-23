using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class Session
	{
		
		
		public int ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}

		
		
		public bool IsReady
		{
			get
			{
				return this.isReady;
			}
		}

		
		
		public virtual bool IsObserverSession
		{
			get
			{
				return false;
			}
		}

		
		
		public int ObjectId
		{
			get
			{
				return this.worldObject.ObjectId;
			}
		}

		
		
		public ObjectType ObjectType
		{
			get
			{
				return this.worldObject.ObjectType;
			}
		}

		
		
		public int TeamNumber
		{
			get
			{
				return this.worldObject.TeamNumber;
			}
		}

		
		
		public ColliderAgent ColliderAgent
		{
			get
			{
				return this.worldObject.ColliderAgent;
			}
		}

		
		public Vector3 GetPosition()
		{
			return this.worldObject.GetPosition();
		}

		
		public void SetPosition(Vector3 position)
		{
			this.worldObject.SetPosition(position);
		}

		
		public Quaternion GetRotation()
		{
			return this.worldObject.GetRotation();
		}

		
		public void SetRotation(Quaternion rot)
		{
			this.worldObject.SetRotation(rot);
		}

		
		protected Session(int connectionId, long userId, string nickname)
		{
			this.connectionId = connectionId;
			this.userId = userId;
			if (userId == 0) this.userId = 100;
			this.nickname = nickname;
		}

		
		public void Ready()
		{
			this.isReady = true;
		}

		
		public void UpdateConnectionId(int connectionId)
		{
			this.connectionId = connectionId;
		}

		
		protected void SetWorldObject(WorldObject worldObject)
		{
			this.worldObject = worldObject;
		}

		
		protected virtual List<PacketWrapper> processSight(List<PacketWrapper> commandQueue)
		{
			return commandQueue;
		}

		
		public IPacket GetCommands(int currentSeq)
		{
			this.currentSeq = currentSeq;
			this.commandLists[currentSeq % 1000] = new CommandPack(currentSeq, this.processSight(this.commandQueue));
			this.commandQueue.Clear();
			CommandList commandList = new CommandList(currentSeq, 3);
			for (int i = 0; i < 3; i++)
			{
				int num = (currentSeq - i) % 1000;
				if (num < 0)
				{
					break;
				}
				CommandPack commandPack = this.commandLists[num];
				if (commandPack != null)
				{
					commandList.Add(commandPack.commands, i);
				}
			}
			return commandList;
		}

		
		public List<CommandList> GetMissingCommands(List<int> seqs)
		{
			if (seqs == null)
			{
				return null;
			}
			List<CommandList> list = new List<CommandList>();
			foreach (int num in seqs)
			{
				if (this.currentSeq - num >= 1000)
				{
					Log.W("Requested too old command [{0}], Reconnecting is required.", this.currentSeq);
					break;
				}
				List<PacketWrapper> commands = this.commandLists[num % 1000].commands;
				if (commands == null)
				{
					Log.W("Failed to find missed commands for seq [{0}]", num);
				}
				else
				{
					CommandList commandList = new CommandList(num, 1);
					commandList.Add(commands, 0);
					list.Add(commandList);
				}
			}
			return list;
		}

		
		public void EnqueueCommandPacket(PacketWrapper commandPacketWrapper)
		{
			this.commandQueue.Add(commandPacketWrapper);
		}

		
		public void EnqueueCommandPacket(CommandPacket commandPacket)
		{
			this.EnqueueCommandPacket(new PacketWrapper(commandPacket));
		}

		
		public void EnqueueCommandPacket(CmdFinishGameTeamAlive commandPacket)
		{
			this.finishGameTeamAlive = commandPacket;
			this.EnqueueCommandPacket(this.finishGameTeamAlive);
		}

		
		public void ResendFinishGameTeamAlive()
		{
			if (this.finishGameTeamAlive != null)
			{
				this.EnqueueCommandPacket(this.finishGameTeamAlive);
			}
		}

		
		public readonly long userId;

		
		public readonly string nickname;

		
		private int connectionId;

		
		private WorldObject worldObject;

		
		private bool isReady;

		
		private const int CommandListWindowSize = 1000;

		
		private readonly List<PacketWrapper> commandQueue = new List<PacketWrapper>();

		
		private readonly CommandPack[] commandLists = new CommandPack[1000];

		
		private int currentSeq;

		
		private CommandPacket finishGameTeamAlive;
	}
}
