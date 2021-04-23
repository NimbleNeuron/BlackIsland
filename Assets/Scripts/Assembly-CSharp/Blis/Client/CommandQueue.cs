using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class CommandQueue
	{
		private const int PendingCommandWindowSize = 500;


		private readonly CommandEntry[] pendingCommands = new CommandEntry[500];


		private int currentSeq = -1;


		private int maxPendingSeq = -1;


		private int pendingSeq = -1;


		private int playoutBufferCount = 1;


		public int CurrentSeq => currentSeq;


		public CommandEntry Dequeue()
		{
			int num = currentSeq + 1;
			if (num <= 0 && pendingSeq - num < playoutBufferCount)
			{
				return null;
			}

			if (pendingSeq < num)
			{
				return null;
			}

			CommandEntry commandEntry = pendingCommands[num % 500];
			if (commandEntry == null)
			{
				return null;
			}

			pendingCommands[num % 500] = null;
			currentSeq = num;
			return commandEntry;
		}


		public void DequeueAll()
		{
			Log.H(string.Format("Dequeue All ({0})", maxPendingSeq - currentSeq));
			if (maxPendingSeq - currentSeq >= 500)
			{
				Debug.LogException(new CommandQueueOverflowException());
			}

			try
			{
				while (Dequeue() != null) { }
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}


		public void Enqueue(CommandList commandList)
		{
			maxPendingSeq = Mathf.Max(commandList.seq, maxPendingSeq);
			if (maxPendingSeq - currentSeq >= 500)
			{
				Log.E("[Client] CommandQueueOverflowException: Current: {0}, Pending: {1}, MaxPending: {2}", currentSeq,
					pendingSeq, maxPendingSeq);
				throw new CommandQueueOverflowException();
			}

			for (int i = 0; i < commandList.commands.Length; i++)
			{
				int num = commandList.seq - i;
				if (num <= pendingSeq)
				{
					break;
				}

				int num2 = num % 500;
				string arg = "";
				if (commandList.commands.Length > i && pendingCommands[num2] == null)
				{
					pendingCommands[num2] = new CommandEntry(num, commandList.commands[i]);
					arg = arg + ", " + num;
				}
			}

			int num3 = pendingSeq + 1;
			pendingSeq = maxPendingSeq;
			for (int j = num3; j <= maxPendingSeq; j++)
			{
				if (pendingCommands[j % 500] == null)
				{
					pendingSeq = j - 1;
					return;
				}
			}
		}


		public bool IsQueueFilled()
		{
			return pendingSeq > 0 && FastForwardRequired();
		}


		public bool FastForwardRequired()
		{
			return pendingSeq - currentSeq > playoutBufferCount + 1;
		}


		public List<int> GetMissingSeqs()
		{
			List<int> list = new List<int>();
			if (maxPendingSeq - pendingSeq <= 3)
			{
				return list;
			}

			for (int i = 0; i < 3; i++)
			{
				int num = pendingSeq + 1 + i;
				if (pendingCommands[num % 500] == null)
				{
					list.Add(num);
				}
			}

			return list;
		}


		public void SetPlayoutBufferCount(int count)
		{
			playoutBufferCount = count;
		}


		public void SkipToSeq(int seq)
		{
			maxPendingSeq = seq;
			pendingSeq = seq;
			currentSeq = seq;
		}
	}
}