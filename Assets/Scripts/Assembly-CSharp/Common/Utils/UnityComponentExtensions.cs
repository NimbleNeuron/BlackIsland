using System;
using System.Reflection;
using UnityEngine;

namespace Common.Utils
{
	
	public static class UnityComponentExtensions
	{
		
		private static Component GetCopyOf(this Component comp, Component other)
		{
			Type type = comp.GetType();
			if (type != other.GetType())
			{
				return null;
			}
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			foreach (FieldInfo fieldInfo in type.GetFields(bindingAttr))
			{
				fieldInfo.SetValue(comp, fieldInfo.GetValue(other));
			}
			return comp;
		}

		
		public static Component AddCopyComponent(this GameObject go, Component toAdd)
		{
			return go.AddComponent(toAdd.GetType()).GetCopyOf(toAdd);
		}

		
		public static T AddCopyComponent<T>(this GameObject go, T toAdd) where T : Component
		{
			return go.AddComponent<T>().GetCopyOf(toAdd) as T;
		}
	}
}
