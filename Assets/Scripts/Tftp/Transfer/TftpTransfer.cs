using System;
using System.IO;
using System.Net;
using System.Threading;
using Tftp.Net.Channel;
using Tftp.Net.Trace;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer
{
	
	internal class TftpTransfer : ITftpTransfer, IDisposable
	{
		
		
		
		public TransferOptionSet ProposedOptions { get; set; }

		
		
		
		public TransferOptionSet NegotiatedOptions { get; private set; }

		
		
		
		public bool WasStarted { get; private set; }

		
		
		
		public Stream InputOutputStream { get; protected set; }

		
		public TftpTransfer(ITransferChannel connection, string filename, ITransferState initialState)
		{
			this.ProposedOptions = TransferOptionSet.NewDefaultSet();
			this.Filename = filename;
			this.RetryCount = 5;
			this.timer = new Timer(new TimerCallback(this.timer_OnTimer), null, 500, 500);
			this.SetState(initialState);
			this.connection = connection;
			this.connection.OnCommandReceived += this.connection_OnCommandReceived;
			this.connection.OnError += this.connection_OnError;
			this.connection.Open();
		}

		
		private void timer_OnTimer(object context)
		{
			lock (this)
			{
				this.state.OnTimer();
			}
		}

		
		private void connection_OnCommandReceived(ITftpCommand command, EndPoint endpoint)
		{
			lock (this)
			{
				this.state.OnCommand(command, endpoint);
			}
		}

		
		private void connection_OnError(TftpTransferError error)
		{
			lock (this)
			{
				this.RaiseOnError(error);
			}
		}

		
		internal virtual void SetState(ITransferState newState)
		{
			this.state = this.DecorateForLogging(newState);
			this.state.Context = this;
			this.state.OnStateEnter();
		}

		
		protected virtual ITransferState DecorateForLogging(ITransferState state)
		{
			if (!TftpTrace.Enabled)
			{
				return state;
			}
			return new LoggingStateDecorator(state, this);
		}

		
		internal ITransferChannel GetConnection()
		{
			return this.connection;
		}

		
		internal void RaiseOnProgress(long bytesTransferred)
		{
			if (this.OnProgress != null)
			{
				this.OnProgress(this, new TftpTransferProgress(bytesTransferred, this.ExpectedSize));
			}
		}

		
		internal void RaiseOnError(TftpTransferError error)
		{
			if (this.OnError != null)
			{
				this.OnError(this, error);
			}
		}

		
		internal void RaiseOnFinished()
		{
			if (this.OnFinished != null)
			{
				this.OnFinished(this);
			}
		}

		
		internal void FinishOptionNegotiation(TransferOptionSet negotiated)
		{
			this.NegotiatedOptions = negotiated;
			if (!this.NegotiatedOptions.IncludesBlockSizeOption)
			{
				this.NegotiatedOptions.BlockSize = 512;
			}
			if (!this.NegotiatedOptions.IncludesTimeoutOption)
			{
				this.NegotiatedOptions.Timeout = 5;
			}
		}

		
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.GetHashCode(),
				" (",
				this.Filename,
				")"
			});
		}

		
		internal void FillOrDisableTransferSizeOption()
		{
			try
			{
				this.ProposedOptions.TransferSize = (long)((int)this.InputOutputStream.Length);
			}
			catch (NotSupportedException)
			{
			}
			finally
			{
				if (this.ProposedOptions.TransferSize <= 0L)
				{
					this.ProposedOptions.IncludesTransferSizeOption = false;
				}
			}
		}

		
		
		
		public event TftpProgressHandler OnProgress;

		
		
		
		public event TftpEventHandler OnFinished;

		
		
		
		public event TftpErrorHandler OnError;

		
		
		
		public string Filename { get; private set; }

		
		
		
		public int RetryCount { get; set; }

		
		
		
		public virtual TftpTransferMode TransferMode { get; set; }

		
		
		
		public object UserContext { get; set; }

		
		
		
		public virtual TimeSpan RetryTimeout
		{
			get
			{
				return TimeSpan.FromSeconds((double)((this.NegotiatedOptions != null) ? this.NegotiatedOptions.Timeout : this.ProposedOptions.Timeout));
			}
			set
			{
				this.ThrowExceptionIfTransferAlreadyStarted();
				this.ProposedOptions.Timeout = value.Seconds;
			}
		}

		
		
		
		public virtual long ExpectedSize
		{
			get
			{
				if (this.NegotiatedOptions == null)
				{
					return this.ProposedOptions.TransferSize;
				}
				return this.NegotiatedOptions.TransferSize;
			}
			set
			{
				this.ThrowExceptionIfTransferAlreadyStarted();
				this.ProposedOptions.TransferSize = value;
			}
		}

		
		
		
		public virtual int BlockSize
		{
			get
			{
				if (this.NegotiatedOptions == null)
				{
					return this.ProposedOptions.BlockSize;
				}
				return this.NegotiatedOptions.BlockSize;
			}
			set
			{
				this.ThrowExceptionIfTransferAlreadyStarted();
				this.ProposedOptions.BlockSize = value;
			}
		}

		
		
		
		public virtual BlockCounterWrapAround BlockCounterWrapping
		{
			get
			{
				return this.wrapping;
			}
			set
			{
				this.ThrowExceptionIfTransferAlreadyStarted();
				this.wrapping = value;
			}
		}

		
		private void ThrowExceptionIfTransferAlreadyStarted()
		{
			if (this.WasStarted)
			{
				throw new InvalidOperationException("You cannot change tftp transfer options after the transfer has been started.");
			}
		}

		
		public void Start(Stream data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (this.WasStarted)
			{
				throw new InvalidOperationException("This transfer has already been started.");
			}
			this.WasStarted = true;
			this.InputOutputStream = data;
			lock (this)
			{
				this.state.OnStart();
			}
		}

		
		public void Cancel(TftpErrorPacket reason)
		{
			if (reason == null)
			{
				throw new ArgumentNullException("reason");
			}
			lock (this)
			{
				this.state.OnCancel(reason);
			}
		}

		
		public virtual void Dispose()
		{
			lock (this)
			{
				this.timer.Dispose();
				this.Cancel(new TftpErrorPacket(0, "ITftpTransfer has been disposed."));
				if (this.InputOutputStream != null)
				{
					this.InputOutputStream.Close();
					this.InputOutputStream = null;
				}
				this.connection.Dispose();
			}
		}

		
		protected ITransferState state;

		
		protected readonly ITransferChannel connection;

		
		protected Timer timer;

		
		private BlockCounterWrapAround wrapping;
	}
}
