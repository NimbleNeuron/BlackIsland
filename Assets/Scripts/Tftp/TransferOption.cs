using System;

namespace Tftp.Net
{
	
	public class TransferOption
	{
		
		
		
		public string Name { get; private set; }

		
		
		
		public string Value { get; set; }

		
		
		
		public bool IsAcknowledged { get; internal set; }

		
		internal TransferOption(string name, string value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name must not be null or empty.");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value must not be null.");
			}
			this.Name = name;
			this.Value = value;
		}

		
		public override string ToString()
		{
			return this.Name + "=" + this.Value;
		}
	}
}
