using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
	{
		private static T instance;


		private static bool creatingInstance;


		public static T Instance {
			get
			{
				EnsureInstance();
				return instance;
			}
		}


		public static bool HasInstance => instance != null;


		public static T EnsureInstance()
		{
			if (instance == null)
			{
				if (creatingInstance)
				{
					throw new Exception("Instance accessed during creation of instance.");
				}

				creatingInstance = true;
				Type typeFromHandle = typeof(T);
				PropertyInfo property = typeFromHandle.GetProperty("FilePath",
					BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty);
				if (property == null)
				{
					throw new Exception("No static Property 'FilePath' in " + typeFromHandle);
				}

				string text = property.GetValue(null, null) as string;
				if (text == null)
				{
					throw new Exception("static property 'FilePath' is not a string or null in " + typeFromHandle);
				}

				if (!text.Contains("Resources"))
				{
					throw new Exception("static property 'FilePath' must contain a Resources folder.");
				}

				if (text.Contains("Plugins"))
				{
					throw new Exception("static property 'FilePath' must not contain a Plugin folder.");
				}

				if (!text.EndsWith(".asset"))
				{
					text += ".asset";
				}

				Object @object = Resources.Load(Path.GetFileNameWithoutExtension(text.Split(new[]
				{
					"Resources"
				}, StringSplitOptions.None).Last<string>()));
				instance = @object as T;
				if (@object == null)
				{
					instance = CreateInstance<T>();
					Debug.LogErrorFormat(
						"Could not find scriptable object of type '{0}'. Make sure it is instantiated inside Unity before building.",
						typeof(T));
				}

				creatingInstance = false;
			}

			return instance;
		}
	}
}