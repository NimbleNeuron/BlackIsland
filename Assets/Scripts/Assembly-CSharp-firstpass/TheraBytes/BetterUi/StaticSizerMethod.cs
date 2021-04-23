using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class StaticSizerMethod
	{
		[SerializeField] private string assemblyName = "Assembly-CSharp";


		[SerializeField] private string typeName = default;


		[SerializeField] private string methodName = default;


		public float Invoke(Component caller, Vector2 optimizedResolution, Vector2 actualResolution, float optimizedDpi,
			float actualDpi)
		{
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
				.FirstOrDefault(a => a.GetName().Name == assemblyName);
			if (assembly == null)
			{
				Debug.LogErrorFormat("Static Sizer Method: Assembly with name '{0}' could not be found.", assemblyName);
				return 0f;
			}

			Type type = assembly.GetType(typeName, false);
			if (type == null)
			{
				Debug.LogErrorFormat(
					"Static Sizer Method: Type '{0}' could not be found in assembly '{1}'. Make sure the name contains the full namespace.",
					typeName, assemblyName);
				return 0f;
			}

			MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				Debug.LogErrorFormat(
					"Static Sizer Method: Method '{0}()' could not be found in Type '{0}'. Make sure it is declared public and static.",
					methodName, typeName);
				return 0f;
			}

			float result;
			try
			{
				result = (float) method.Invoke(null, new object[]
				{
					caller,
					optimizedResolution,
					actualResolution,
					optimizedDpi,
					actualDpi
				});
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat(
					"Static Sizer Method: Method '{0}.{1}()' could be found but failed to be invoked (see details below). Make sure it has all parameters and returns a float.",
					methodName, typeName);
				Debug.LogException(exception);
				result = 0f;
			}

			return result;
		}
	}
}