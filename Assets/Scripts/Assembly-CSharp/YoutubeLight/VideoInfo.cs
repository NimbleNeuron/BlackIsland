using System.Collections.Generic;

namespace YoutubeLight
{
	
	public class VideoInfo
	{
		
		internal VideoInfo(int formatCode) : this(formatCode, VideoType.Unknown, 0, false, false, AudioType.Unknown, 0, AdaptiveType.None)
		{
		}

		
		internal VideoInfo(VideoInfo info) : this(info.FormatCode, info.VideoType, info.Resolution, info.HDR, info.Is3D, info.AudioType, info.AudioBitrate, info.AdaptiveType)
		{
		}

		
		private VideoInfo(int formatCode, VideoType videoType, int resolution, bool HDR, bool is3D, AudioType audioType, int audioBitrate, AdaptiveType adaptiveType)
		{
			this.FormatCode = formatCode;
			this.VideoType = videoType;
			this.Resolution = resolution;
			this.Is3D = is3D;
			this.AudioType = audioType;
			this.AudioBitrate = audioBitrate;
			this.AdaptiveType = adaptiveType;
			this.HDR = HDR;
		}

		
		
		
		public AdaptiveType AdaptiveType { get; private set; }

		
		
		
		public int AudioBitrate { get; private set; }

		
		
		public string AudioExtension
		{
			get
			{
				switch (this.AudioType)
				{
				case AudioType.Aac:
					return ".aac";
				case AudioType.Mp3:
					return ".mp3";
				case AudioType.Opus:
					return ".webm";
				case AudioType.Vorbis:
					return ".ogg";
				default:
					return null;
				}
			}
		}

		
		
		
		public AudioType AudioType { get; private set; }

		
		
		public bool CanExtractAudio
		{
			get
			{
				return this.VideoType == VideoType.Flv;
			}
		}

		
		
		
		public string DownloadUrl { get; internal set; }

		
		
		
		public int FormatCode { get; private set; }

		
		
		
		public bool Is3D { get; private set; }

		
		
		
		public bool HDR { get; private set; }

		
		
		
		public bool RequiresDecryption { get; internal set; }

		
		
		
		public int Resolution { get; private set; }

		
		
		
		public string Title { get; internal set; }

		
		
		public string VideoExtension
		{
			get
			{
				switch (this.VideoType)
				{
				case VideoType.Mobile_3gp:
					return ".3gp";
				case VideoType.Flv:
					return ".flv";
				case VideoType.Mp4:
					return ".mp4";
				case VideoType.WebM:
					return ".webm";
				}
				return null;
			}
		}

		
		
		
		public VideoType VideoType { get; private set; }

		
		
		
		internal string HtmlPlayerVersion { get; set; }

		
		
		
		internal string HtmlscriptName { get; set; }

		
		public override string ToString()
		{
			return string.Format("Full Title: {0}, Type: {1}, Resolution: {2}p", this.Title + this.VideoExtension, this.VideoType, this.Resolution);
		}

		
		internal static IEnumerable<VideoInfo> Defaults = new List<VideoInfo>
		{
			new VideoInfo(5, VideoType.Flv, 240, false, false, AudioType.Mp3, 64, AdaptiveType.Audio_Video),
			new VideoInfo(6, VideoType.Flv, 270, false, false, AudioType.Mp3, 64, AdaptiveType.Audio_Video),
			new VideoInfo(17, VideoType.Mobile_3gp, 144, false, false, AudioType.Aac, 24, AdaptiveType.Audio_Video),
			new VideoInfo(18, VideoType.Mp4, 360, false, false, AudioType.Aac, 96, AdaptiveType.Audio_Video),
			new VideoInfo(22, VideoType.Mp4, 720, false, false, AudioType.Aac, 192, AdaptiveType.Audio_Video),
			new VideoInfo(34, VideoType.Flv, 360, false, false, AudioType.Aac, 128, AdaptiveType.Audio_Video),
			new VideoInfo(35, VideoType.Flv, 480, false, false, AudioType.Aac, 128, AdaptiveType.Audio_Video),
			new VideoInfo(36, VideoType.Mobile_3gp, 240, false, false, AudioType.Aac, 38, AdaptiveType.Audio_Video),
			new VideoInfo(37, VideoType.Mp4, 1080, false, false, AudioType.Aac, 192, AdaptiveType.Audio_Video),
			new VideoInfo(38, VideoType.Mp4, 3072, false, false, AudioType.Aac, 192, AdaptiveType.Audio_Video),
			new VideoInfo(43, VideoType.WebM, 360, false, false, AudioType.Vorbis, 128, AdaptiveType.Audio_Video),
			new VideoInfo(44, VideoType.WebM, 480, false, false, AudioType.Vorbis, 128, AdaptiveType.Audio_Video),
			new VideoInfo(45, VideoType.WebM, 720, false, false, AudioType.Vorbis, 192, AdaptiveType.Audio_Video),
			new VideoInfo(46, VideoType.WebM, 1080, false, false, AudioType.Vorbis, 192, AdaptiveType.Audio_Video),
			new VideoInfo(82, VideoType.Mp4, 360, false, true, AudioType.Aac, 96, AdaptiveType.Audio_Video),
			new VideoInfo(83, VideoType.Mp4, 480, false, true, AudioType.Aac, 96, AdaptiveType.Audio_Video),
			new VideoInfo(84, VideoType.Mp4, 720, false, true, AudioType.Aac, 152, AdaptiveType.Audio_Video),
			new VideoInfo(85, VideoType.Mp4, 1080, false, true, AudioType.Aac, 152, AdaptiveType.Audio_Video),
			new VideoInfo(100, VideoType.Hls, 240, false, true, AudioType.Vorbis, 128, AdaptiveType.Audio_Video),
			new VideoInfo(100, VideoType.Hls, 360, false, true, AudioType.Vorbis, 128, AdaptiveType.Audio_Video),
			new VideoInfo(100, VideoType.Hls, 480, false, true, AudioType.Vorbis, 128, AdaptiveType.Audio_Video),
			new VideoInfo(100, VideoType.Hls, 720, false, true, AudioType.Vorbis, 128, AdaptiveType.Audio_Video),
			new VideoInfo(100, VideoType.Hls, 1080, false, true, AudioType.Vorbis, 128, AdaptiveType.Audio_Video),
			new VideoInfo(100, VideoType.WebM, 360, false, true, AudioType.Vorbis, 128, AdaptiveType.Audio_Video),
			new VideoInfo(101, VideoType.WebM, 360, false, true, AudioType.Vorbis, 192, AdaptiveType.Audio_Video),
			new VideoInfo(102, VideoType.WebM, 720, false, true, AudioType.Vorbis, 192, AdaptiveType.Audio_Video),
			new VideoInfo(132, VideoType.Hls, 240, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(133, VideoType.Mp4, 240, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(134, VideoType.Mp4, 360, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(135, VideoType.Mp4, 480, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(136, VideoType.Mp4, 720, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(137, VideoType.Mp4, 1080, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(138, VideoType.Mp4, 2160, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(139, VideoType.Unknown, 0, false, false, AudioType.Aac, 48, AdaptiveType.Audio),
			new VideoInfo(140, VideoType.Unknown, 0, false, false, AudioType.Aac, 128, AdaptiveType.Audio),
			new VideoInfo(141, VideoType.Unknown, 0, false, false, AudioType.Aac, 256, AdaptiveType.Audio),
			new VideoInfo(160, VideoType.Mp4, 144, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(167, VideoType.WebM, 360, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(168, VideoType.WebM, 480, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(169, VideoType.WebM, 720, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(170, VideoType.WebM, 1080, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(171, VideoType.Unknown, 0, false, false, AudioType.Vorbis, 128, AdaptiveType.Audio),
			new VideoInfo(172, VideoType.Unknown, 0, false, false, AudioType.Vorbis, 192, AdaptiveType.Audio),
			new VideoInfo(218, VideoType.WebM, 480, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(219, VideoType.WebM, 144, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(242, VideoType.WebM, 240, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(243, VideoType.WebM, 360, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(244, VideoType.WebM, 480, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(247, VideoType.WebM, 720, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(248, VideoType.WebM, 1080, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(249, VideoType.Unknown, 0, false, false, AudioType.Opus, 50, AdaptiveType.Audio),
			new VideoInfo(250, VideoType.Unknown, 0, false, false, AudioType.Opus, 70, AdaptiveType.Audio),
			new VideoInfo(251, VideoType.Unknown, 0, false, false, AudioType.Opus, 160, AdaptiveType.Audio),
			new VideoInfo(264, VideoType.Mp4, 1440, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(266, VideoType.Mp4, 2160, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(271, VideoType.WebM, 1440, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(272, VideoType.WebM, 4320, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(278, VideoType.WebM, 144, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(298, VideoType.Mp4, 720, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(299, VideoType.Mp4, 1080, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(302, VideoType.WebM, 720, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(303, VideoType.WebM, 1080, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(308, VideoType.WebM, 1440, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(313, VideoType.WebM, 2160, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(315, VideoType.WebM, 2160, false, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(330, VideoType.WebM, 144, true, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(331, VideoType.WebM, 240, true, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(332, VideoType.WebM, 360, true, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(333, VideoType.WebM, 480, true, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(334, VideoType.WebM, 720, true, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(335, VideoType.WebM, 1080, true, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(336, VideoType.WebM, 1440, true, false, AudioType.Unknown, 0, AdaptiveType.Video),
			new VideoInfo(337, VideoType.WebM, 2160, true, false, AudioType.Unknown, 0, AdaptiveType.Video)
		};
	}
}
