using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Animation
{
	
	[Serializable]
	public class SpriteChunkList
	{
		
		
		public string Title
		{
			get
			{
				return this.title;
			}
		}

		
		
		public List<SpriteChunk> SpriteChunkDataList
		{
			get
			{
				return this.spriteChunkDataList;
			}
		}

		
		[SerializeField]
		private string title = default;

		
		[SerializeField]
		private List<SpriteChunk> spriteChunkDataList = default;
	}
}
