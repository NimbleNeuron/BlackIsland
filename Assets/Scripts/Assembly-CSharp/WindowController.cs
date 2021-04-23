using System;
using System.Runtime.InteropServices;

public class WindowController
{
	
	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	
	[DllImport("user32.dll")]
	private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

	
	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	
	[DllImport("user32.dll")]
	private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

	
	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	
	public static void Init()
	{
		if (!WindowController.isInitialized)
		{
			WindowController.isInitialized = true;
			WindowController.hWnd = WindowController.GetActiveWindow();
		}
	}

	
	public static void Find()
	{
		if (!WindowController.hWnd.Equals(IntPtr.Zero))
		{
			WindowController.ShowWindowAsync(WindowController.hWnd, 1);
		}
		WindowController.SetForegroundWindowForce();
	}

	
	private static void SetForegroundWindowForce()
	{
		if (WindowController.GetForegroundWindow() != WindowController.hWnd)
		{
			WindowController.SetWindowPos(WindowController.hWnd, -1, 0, 0, 0, 0, 67);
			WindowController.SetWindowPos(WindowController.hWnd, -2, 0, 0, 0, 0, 67);
		}
	}

	
	[DllImport("user32.dll")]
	private static extern bool FlashWindowEx(ref WindowController.FLASHWINFO pwfi);

	
	public static bool FlashWindowExUntilFront()
	{
		WindowController.FLASHWINFO structure = default(WindowController.FLASHWINFO);
		structure.cbSize = Convert.ToUInt32(Marshal.SizeOf<WindowController.FLASHWINFO>(structure));
		structure.hwnd = WindowController.hWnd;
		structure.dwFlags = 7U;
		structure.uCount = uint.MaxValue;
		structure.dwTimeout = 0U;
		return WindowController.FlashWindowEx(ref structure);
	}

	
	public static bool FlashWindowExUntilStop()
	{
		WindowController.FLASHWINFO structure = default(WindowController.FLASHWINFO);
		structure.cbSize = Convert.ToUInt32(Marshal.SizeOf<WindowController.FLASHWINFO>(structure));
		structure.hwnd = WindowController.hWnd;
		structure.dwFlags = 7U;
		structure.uCount = uint.MaxValue;
		structure.dwTimeout = 0U;
		WindowController.isFlashing = true;
		return WindowController.FlashWindowEx(ref structure);
	}

	
	public static bool FlashStopWindowEx()
	{
		if (!WindowController.isFlashing)
		{
			return false;
		}
		WindowController.FLASHWINFO structure = default(WindowController.FLASHWINFO);
		structure.cbSize = Convert.ToUInt32(Marshal.SizeOf<WindowController.FLASHWINFO>(structure));
		structure.hwnd = WindowController.hWnd;
		structure.dwFlags = 0U;
		structure.uCount = uint.MaxValue;
		structure.dwTimeout = 0U;
		WindowController.isFlashing = false;
		return WindowController.FlashWindowEx(ref structure);
	}

	
	private const int SW_SHOWNORMAL = 1;

	
	private const int SW_SHOWMINIMIZED = 2;

	
	private const int SW_SHOWMAXIMIZED = 3;

	
	private const int HWND_TOPMOST = -1;

	
	private const int HWND_NOTOPMOST = -2;

	
	private const int SWP_NOSIZE = 1;

	
	private const int SWP_NOMOVE = 2;

	
	private const int SWP_SHOWWINDOW = 64;

	
	private static IntPtr hWnd;

	
	private static bool isInitialized;

	
	private static bool isFlashing;

	
	public const uint FLASHW_STOP = 0U;

	
	public const uint FLASHW_ALL = 3U;

	
	public const uint FLASHW_TIMER = 4U;

	
	public const uint FLASHW_TIMERNOFG = 12U;

	
	public struct FLASHWINFO
	{
		
		public uint cbSize;

		
		public IntPtr hwnd;

		
		public uint dwFlags;

		
		public uint uCount;

		
		public uint dwTimeout;
	}
}
