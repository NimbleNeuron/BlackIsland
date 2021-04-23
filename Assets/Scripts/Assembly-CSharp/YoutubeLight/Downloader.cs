using System;

namespace YoutubeLight
{
	
	public abstract class Downloader
	{
		
		protected Downloader(VideoInfo video, string savePath, int? bytesToDownload = null)
		{
			if (video == null)
			{
				throw new ArgumentNullException("video");
			}

			if (savePath == null)
			{
				throw new ArgumentNullException("savePath");
			}

			Video = video;
			SavePath = savePath;
			BytesToDownload = bytesToDownload;
		}

		
		
		
		public int? BytesToDownload { get; }

		
		
		
		public string SavePath { get; }

		
		
		
		public VideoInfo Video { get; }

		
		
		
		public event EventHandler DownloadFinished;

		
		
		
		public event EventHandler DownloadStarted;

		
		public abstract void Execute();

		
		protected void OnDownloadFinished(EventArgs e)
		{
			if (DownloadFinished != null)
			{
				DownloadFinished(this, e);
			}
		}

		
		protected void OnDownloadStarted(EventArgs e)
		{
			if (DownloadStarted != null)
			{
				DownloadStarted(this, e);
			}
		}
	}
}