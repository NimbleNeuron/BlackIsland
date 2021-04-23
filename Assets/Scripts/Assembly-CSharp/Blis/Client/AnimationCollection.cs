using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class AnimationCollection
	{
		private readonly AnimationParamCollection animationParamCollection = new AnimationParamCollection();


		private readonly List<string> loadedFilePathList = new List<string>();


		private bool isLoadedCommon;

		public bool HasAnimationParam(int key)
		{
			return animationParamCollection.HasAnimationParam(key);
		}


		public AnimationParam GetAnimationParam(int key, GameObject gameObject = null)
		{
			return animationParamCollection.GetAnimationParam(key, gameObject);
		}


		public int AddAnimationParam(AnimationParam animationParam)
		{
			return animationParamCollection.AddAnimationParam(animationParam);
		}


		public void RemoveAnimationParam(int key)
		{
			animationParamCollection.RemoveAnimationParam(key);
		}


		public void LoadGameCommon()
		{
			if (isLoadedCommon)
			{
				return;
			}

			isLoadedCommon = true;
			Load("Game/Object");
		}


		public void LoadLobbyCharacter(int characterCode)
		{
			CharacterData characterData = GameDB.character.GetCharacterData(characterCode);
			Load("Lobby/Player/" + characterData.resource);
		}


		public void LoadGameCharacter(int characterCode)
		{
			CharacterData characterData = GameDB.character.GetCharacterData(characterCode);
			Load("Game/Player/" + characterData.resource);
		}


		private void Load(string filePath)
		{
			using (List<string>.Enumerator enumerator = loadedFilePathList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == filePath)
					{
						return;
					}
				}
			}

			loadedFilePathList.Add(filePath);
			TextAsset[] array = Resources.LoadAll<TextAsset>("AnimDataTable/" + filePath);
			for (int i = 0; i < array.Length; i++)
			{
				animationParamCollection.Load(array[i]);
			}
		}
	}
}