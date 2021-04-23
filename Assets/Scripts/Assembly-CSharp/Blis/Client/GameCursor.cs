using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class GameCursor : MonoBehaviour
	{
		private List<CursorData> cursors;


		private CursorMode mode;


		private CursorOption option;


		private CursorTarget target;

		private void Awake()
		{
			cursors = new List<CursorData>
			{
				new CursorData(CursorMode.Common, CursorTarget.None, "Cursor_01", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.Block, "Cursor_02", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.Ally, "Cursor_03", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.AllyDyingCondition, "Cursor_04", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.Item, "Cursor_04", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.ItemBox, "Cursor_07", Vector2.zero)
					.AddOption(CursorOption.Opened, "Cursor_08", Vector2.zero)
					.AddOption(CursorOption.Timer, "Cursor_09", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.ResourceBox, "Cursor_04", Vector2.zero)
					.AddOption(CursorOption.Timer, "Cursor_10", Vector2.zero)
					.AddOption(CursorOption.Restrict, "Cursor_11", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.EnemyNotSummon, "Cursor_05", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.EnemySummon, "Cursor_05", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.Disarmed, "Cursor_06", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.BlockSummon, "Cursor_02", Vector2.zero),
				new CursorData(CursorMode.Common, CursorTarget.SecurityConsole, "Cursor_12", Vector2.zero),
				new CursorData(CursorMode.Target, CursorTarget.None, "Pointer_01", new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.Item, "Pointer_01", new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.ItemBox, "Pointer_01", new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.ResourceBox, "Pointer_01", new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.Block, "Pointer_01", new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.Ally, "Pointer_02", new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.AllyDyingCondition, "Pointer_04",
					new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.EnemyNotSummon, "Pointer_03", new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.EnemySummon, "Pointer_04", Vector2.zero),
				new CursorData(CursorMode.Target, CursorTarget.BlockSummon, "Pointer_04", new Vector2(0.5f, 0.5f)),
				new CursorData(CursorMode.Target, CursorTarget.SecurityConsole, "Pointer_04", new Vector2(0.5f, 0.5f))
			};
			mode = CursorMode.Common;
			target = CursorTarget.None;
			option = CursorOption.None;
		}


		public void SetCursorTarget(CursorTarget cursorTarget)
		{
			if (target != cursorTarget)
			{
				target = cursorTarget;
				option = CursorOption.None;
				UpdateCursor();
			}
		}


		public void SetCursorMode(CursorMode cursorMode)
		{
			if (mode != cursorMode)
			{
				mode = cursorMode;
				UpdateCursor();
			}
		}


		public void SetCursorOption(CursorOption cursorOption)
		{
			if (option != cursorOption)
			{
				option = cursorOption;
				UpdateCursor();
			}
		}


		private void UpdateCursor()
		{
			CursorData cursorData = cursors.Find(x => x.cursorMode == mode && x.cursorTarget == target);
			if (cursorData == null)
			{
				Log.E("[GameCursor] Can not fine mode & target match cursorInfo");
				return;
			}

			// co: dotPeek
			Cursor.SetCursor(cursorData.GetCursorTexture(option), cursorData.GetPixelPoint(option),
				UnityEngine.CursorMode.Auto);
		}
	}
}