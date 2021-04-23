using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client.UI
{
	public class OpenBox : UIAction
	{
		public int boxId;


		public List<Item> boxItems = new List<Item>();

		public BoxWindowType boxWindowType;
	}
}