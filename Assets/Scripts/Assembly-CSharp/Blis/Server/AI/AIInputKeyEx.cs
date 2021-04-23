namespace Blis.Server
{
	
	public static class AIInputKeyEx
	{
		
		public static int GetItemSlot(this AIInputKey inputKey)
		{
			switch (inputKey)
			{
				case AIInputKey.Key1:
					return 0;
				case AIInputKey.Key2:
					return 1;
				case AIInputKey.Key3:
					return 2;
				case AIInputKey.Key4:
					return 3;
				case AIInputKey.Key5:
					return 4;
				case AIInputKey.Key6:
					return 5;
				case AIInputKey.Key7:
					return 6;
				case AIInputKey.Key8:
					return 7;
				case AIInputKey.Key9:
					return 8;
				case AIInputKey.Key0:
					return 9;
				default:
					return -1;
			}
		}
	}
}