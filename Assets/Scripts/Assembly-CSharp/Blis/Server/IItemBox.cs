using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	public interface IItemBox
	{
		
		List<Item> Open(PlayerSession playerSession);

		
		void AddItem(Item item);

		
		Item FindItem(int itemId);

		
		void RemoveItem(int itemId);

		
		void Close(PlayerSession playerSession);

		
		bool IsFullCapacity();

		
		bool IsEmpty();
	}
}