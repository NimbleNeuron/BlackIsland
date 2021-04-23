using System;
using System.Collections.Generic;
using System.IO;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class AnimationParamCollection
	{
		private readonly SortedDictionary<int, AnimationParam> animationParams =
			new SortedDictionary<int, AnimationParam>();

		public static int Int32ParseFast(string value)
		{
			int num = 0;
			int length = value.Length;
			bool flag = false;
			for (int i = 0; i < length; i++)
			{
				if (value[i].CompareTo('-') == 0)
				{
					flag = true;
				}
				else
				{
					num = 10 * num + (value[i] - '0');
				}
			}

			if (flag)
			{
				return -num;
			}

			return num;
		}


		public static string GetAnimDataTableFullPath(string myFilePath)
		{
			Log.E("Unity Editor only Method : GetAnimDataTableFullPath");
			return null;
		}


		public static string GetBinaryAnimDataTableFullPath(string myFilePath)
		{
			return Application.dataPath + "/Resources/AnimDataTable/" + myFilePath;
		}


		public void Load(TextAsset textAsset)
		{
			string text = textAsset.name + ".xml";
			text = GetBinaryAnimDataTableFullPath(text);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
			try
			{
				StringReader stringReader = new StringReader(textAsset.text);
				foreach (XmlUnidirectionalDocument xmlUnidirectionalDocument in XmlUnidirectionalDocument
					.Create(stringReader).Childs)
				{
					int layer = 0;
					string tag = "";
					foreach (XmlUnidirectionalAttribute xmlUnidirectionalAttribute in xmlUnidirectionalDocument
						.Attributes)
					{
						string key = xmlUnidirectionalAttribute.Key;
						if (!(key == "layer"))
						{
							if (!(key == "name"))
							{
								if (key == "clipname")
								{
									string value = xmlUnidirectionalAttribute.Value;
								}
							}
							else
							{
								tag = xmlUnidirectionalAttribute.Value;
							}
						}
						else
						{
							layer = int.Parse(xmlUnidirectionalAttribute.Value);
						}
					}

					foreach (XmlUnidirectionalDocument xmlUnidirectionalDocument2 in xmlUnidirectionalDocument.Childs)
					{
						int[] array = null;
						int key2 = 0;
						int eventIndex = 0;
						Type type = null;
						object param = null;
						foreach (XmlUnidirectionalAttribute xmlUnidirectionalAttribute2 in xmlUnidirectionalDocument2
							.Attributes)
						{
							string key = xmlUnidirectionalAttribute2.Key;
							if (!(key == "syncLayerIndices"))
							{
								if (!(key == "key"))
								{
									if (!(key == "eventIndex"))
									{
										if (!(key == "type"))
										{
											if (key == "eventname")
											{
												string value2 = xmlUnidirectionalAttribute2.Value;
											}
										}
										else
										{
											type = Type.GetType(xmlUnidirectionalAttribute2.Value);
											param = Activator.CreateInstance(type);
										}
									}
									else
									{
										eventIndex = Int32ParseFast(xmlUnidirectionalAttribute2.Value);
									}
								}
								else
								{
									key2 = Int32ParseFast(xmlUnidirectionalAttribute2.Value);
								}
							}
							else if (!string.IsNullOrEmpty(xmlUnidirectionalAttribute2.Value))
							{
								string[] array2 = xmlUnidirectionalAttribute2.Value.Split(',');
								array = new int[array2.Length];
								for (int i = 0; i < array2.Length; i++)
								{
									array[i] = Int32ParseFast(array2[i]);
								}
							}
						}

						try
						{
							param = xmlUnidirectionalDocument2.DeserializeFromNetSerializer(type);
						}
						catch (Exception)
						{
							Log.W("Fail Deserialize animationEvent : " + text);
						}

						if (!animationParams.ContainsKey(key2))
						{
							animationParams.Add(key2,
								new AnimationParam(layer, array, tag, param, eventIndex, fileNameWithoutExtension));
						}
					}
				}

				stringReader.Close();
			}
			catch (Exception ex)
			{
				Log.W(ex.Message);
			}
		}


		public int AddAnimationParam(AnimationParam animationParam)
		{
			int hashCode = animationParam.GetHashCode();
			AnimationParam animationParam2;
			if (animationParams.TryGetValue(hashCode, out animationParam2))
			{
				animationParams[hashCode] = animationParam;
			}
			else
			{
				animationParams.Add(hashCode, animationParam);
			}

			return hashCode;
		}


		public void RemoveAnimationParam(int key)
		{
			AnimationParam animationParam;
			if (animationParams.TryGetValue(key, out animationParam))
			{
				animationParams.Remove(key);
			}
		}


		public bool HasAnimationParam(int key)
		{
			return animationParams.ContainsKey(key);
		}


		public AnimationParam GetAnimationParam(int key, GameObject gameObject)
		{
			if (!IsLoaded())
			{
				return null;
			}

			if (animationParams.ContainsKey(key))
			{
				return animationParams[key];
			}

			return null;
		}


		public bool IsLoaded()
		{
			return animationParams.Count > 0;
		}
	}
}