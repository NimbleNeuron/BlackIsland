using System;
using System.Diagnostics;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

namespace Blis.Common
{
	
	public class CustomerSupportUtil
	{
		
		public CustomerSupportUtil()
		{
			this.playerLogPath = Path.Combine(new string[]
			{
				Environment.GetEnvironmentVariable("AppData"),
				"..",
				"LocalLow",
				Application.companyName,
				Application.productName
			});
		}

		
		public string GetPlayerLogPath()
		{
			return Path.Combine(this.playerLogPath, "Player.log");
		}

		
		public void CompressPlayerLog(ProgressHandler progressHandler)
		{
			string sourcePath = this.GetPlayerLogPath();
			this.CompressSingleFile(sourcePath, progressHandler);
		}

		
		public void CompressSingleFile(string sourcePath, ProgressHandler progressHandler)
		{
			string text = sourcePath + ".zip";
			Path.GetDirectoryName(sourcePath);
			string fileName = Path.GetFileName(sourcePath);
			UnityEngine.Debug.Log(string.Concat(new string[]
			{
				"sourcePath [",
				sourcePath,
				"], zipFilePath [",
				text,
				"], entryName [",
				fileName,
				"]"
			}));
			try
			{
				FastZipEvents fastZipEvents = new FastZipEvents();
				fastZipEvents.Progress = progressHandler;
				FileInfo fileInfo = new FileInfo(sourcePath);
				new FastZip(fastZipEvents).CreateZip(text, fileInfo.Directory.FullName, false, fileInfo.Name);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}

		
		public string GetIssueUrl(string locale = "en-us")
		{
			return "https://eternalreturn.zendesk.com/hc/" + locale + "/requests/new";
		}

		
		public void OpenLogFolder()
		{
			string directoryName = Path.GetDirectoryName(this.GetPlayerLogPath());
			this.OpenFileBrowser(directoryName);
		}

		
		public void OpenFileBrowser(string path)
		{
			Process.Start("explorer.exe", path);
		}

		
		private string playerLogPath = "~/Library/Logs/Unity/";
	}
}
