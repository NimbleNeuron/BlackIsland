using UnityEngine;

namespace Blis.Client
{
	public class ShortcutInputEvent
	{
		public readonly GameInputEvent gameInputEvent;


		public KeyCode[] combinationKeys;


		public KeyCode key;


		public ShortcutInputEvent(GameInputEvent gameInputEvent, KeyCode key, params KeyCode[] combinationKeys)
		{
			this.gameInputEvent = gameInputEvent;
			this.key = key;
			this.combinationKeys = combinationKeys;
		}
	}
}