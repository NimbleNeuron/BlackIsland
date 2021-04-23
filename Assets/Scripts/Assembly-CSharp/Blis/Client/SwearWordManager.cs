using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class SwearWordManager : SingletonMonoBehaviour<SwearWordManager>
	{
		private const string SWEAR_WORD_BASE_PATH = "LocalizationDB/SwearWordData";


		private const string SWEAR_CHAT_WORD_BASE_PATH = "LocalizationDB/SwearChatWordData";


		private readonly char replaceChar = '*';


		private SwearChatWordData swearChatWordData;


		private SwearWordData swearWordData;

		private void Start()
		{
			DontDestroyOnLoad(transform.gameObject);
		}


		public void LoadSwearWords()
		{
			swearWordData = Resources.Load<SwearWordData>("LocalizationDB/SwearWordData");
			if (swearWordData == null)
			{
				Log.E("[SwearWordManager] nickNameData is null");
			}

			swearChatWordData = Resources.Load<SwearChatWordData>("LocalizationDB/SwearChatWordData");
			if (swearChatWordData == null)
			{
				Log.E("[SwearWordManager] swear chat data is null");
			}
		}


		public string CheckAndReplaceChat(string inputText)
		{
			if (swearChatWordData == null || swearChatWordData.swearWords.Length == 0)
			{
				Log.E("[SwearWordManager.CheckAndReplaceChat] swear chat data is null");
				return inputText;
			}

			List<detectedSwearWord> detectedSwearWords = GetDetectedSwearWords(inputText, swearChatWordData.swearWords);
			return ReplaceSwearWord(detectedSwearWords, inputText, replaceChar);
		}


		public string CheckAndReplaceNickName(string inputText)
		{
			if (swearWordData == null || swearWordData.swearWords.Length == 0)
			{
				Log.E("[SwearWordManager.CheckAndReplaceNickName] nickNameData is null");
				return inputText;
			}

			List<detectedSwearWord> detectedSwearWords = GetDetectedSwearWords(inputText, swearWordData.swearWords);
			return ReplaceSwearWord(detectedSwearWords, inputText, replaceChar);
		}


		private string ReplaceSwearWord(List<detectedSwearWord> detectedSwearWords, string inputText, char replaceText)
		{
			if (detectedSwearWords == null || detectedSwearWords.Count <= 0)
			{
				return inputText;
			}

			char[] array = new char[inputText.Length];
			try
			{
				for (int i = 0; i < inputText.Length; i++)
				{
					if (IsIncludedSwearingIndex(detectedSwearWords, inputText[i], i))
					{
						array[i] = replaceText;
					}
					else
					{
						array[i] = inputText[i];
					}
				}
			}
			catch (Exception ex)
			{
				Log.E("[SwearWordManager.ReplaceSwearWord] error = {0}", ex.Message);
				return inputText;
			}

			return new string(array);
		}


		private bool IsIncludedSwearingIndex(List<detectedSwearWord> detectedSwearWords, char originChar, int index)
		{
			using (List<detectedSwearWord>.Enumerator enumerator = detectedSwearWords.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsIncludedIndex(index))
					{
						return true;
					}
				}
			}

			return false;
		}


		public bool IsSwearWordNickName(string inputText)
		{
			if (swearWordData == null)
			{
				Log.E("[SwearWordManager.IsSwearWodNickName] nickNameData is null");
				return false;
			}

			List<detectedSwearWord> detectedSwearWords = GetDetectedSwearWords(inputText, swearWordData.swearWords);
			return detectedSwearWords != null && detectedSwearWords.Count > 0;
		}


		public bool IsSwearWordChat(string inputText)
		{
			if (swearChatWordData == null)
			{
				Log.E("[SwearWordManager.IsSwearWodNickName] nickNameData is null");
				return false;
			}

			List<detectedSwearWord> detectedSwearWords = GetDetectedSwearWords(inputText, swearChatWordData.swearWords);
			return detectedSwearWords != null && detectedSwearWords.Count > 0;
		}


		private List<detectedSwearWord> GetDetectedSwearWords(string inputText, string[] swearWords)
		{
			if (string.IsNullOrEmpty(inputText))
			{
				Log.H("[SwearWordManager] text is null");
				return null;
			}

			inputText = inputText.ToLower();
			List<detectedSwearWord> list = new List<detectedSwearWord>();
			try
			{
				for (int i = 0; i < swearWords.Length; i++)
				{
					if (string.IsNullOrEmpty(swearWords[i]))
					{
						Log.H("[SwearWordManager] data is null / index = {0}", i);
					}
					else
					{
						List<int> swearWordStartingIndexes = GetSwearWordStartingIndexes(inputText, swearWords[i]);
						if (swearWordStartingIndexes != null && swearWordStartingIndexes.Count > 0)
						{
							for (int j = 0; j < swearWordStartingIndexes.Count; j++)
							{
								list.Add(new detectedSwearWord
								{
									swearWord = swearWords[i],
									startIndex = swearWordStartingIndexes[j],
									endIndex = swearWordStartingIndexes[j] + (swearWords[i].Length - 1),
									dbIndex = i
								});
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.E("[SwearWordManager.GetDetectedSwearWords] error = {0}", ex.Message);
				list = null;
			}

			if (list != null && list.Count > 0)
			{
				Log.H("[SwearWordManager] detected swear word!!");
				foreach (detectedSwearWord detectedSwearWord in list)
				{
					Log.H("[SwearWordManager] startingIndex = {0}, endIndex = {3}, swearWord = {1}, dbIndex = {2}",
						detectedSwearWord.startIndex, detectedSwearWord.swearWord, detectedSwearWord.dbIndex,
						detectedSwearWord.endIndex);
				}
			}

			return list;
		}


		private List<int> GetSwearWordStartingIndexes(string inputText, string swearWord)
		{
			List<int> list = new List<int>();
			int[] pi = GetPi(swearWord);
			int length = inputText.Length;
			int length2 = swearWord.Length;
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				while (num > 0 && inputText[i] != swearWord[num])
				{
					num = pi[num - 1];
				}

				if (inputText[i] == swearWord[num])
				{
					if (num == length2 - 1)
					{
						list.Add(i - length2 + 1);
						num = pi[num];
					}
					else
					{
						num++;
					}
				}
			}

			return list;
		}


		private int[] GetPi(string swearWord)
		{
			int length = swearWord.Length;
			int num = 0;
			int[] array = new int[length];
			for (int i = 1; i < length; i++)
			{
				while (num > 0 && swearWord[i] != swearWord[num])
				{
					num = array[num - 1];
				}

				if (swearWord[i] == swearWord[num])
				{
					num = array[i] = num + 1;
				}
			}

			return array;
		}


		private class detectedSwearWord
		{
			public int dbIndex;


			public int endIndex;


			public int startIndex;


			public string swearWord;

			public bool IsIncludedIndex(int index)
			{
				return startIndex <= index && index <= endIndex;
			}
		}
	}
}