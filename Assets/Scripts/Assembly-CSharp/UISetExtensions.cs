using System;
using System.Reflection;
using UnityEngine.UI;


public static class UISetExtensions
{
	
	private static readonly MethodInfo toggleSetMethod = FindSetMethod(typeof(Toggle));

	
	private static readonly MethodInfo sliderSetMethod = FindSetMethod(typeof(Slider));

	
	private static readonly MethodInfo scrollbarSetMethod = FindSetMethod(typeof(Scrollbar));

	
	private static readonly FieldInfo dropdownValueField =
		typeof(Dropdown).GetField("m_Value", BindingFlags.Instance | BindingFlags.NonPublic);

	
	public static void Set(this Toggle instance, bool value, bool sendCallback = false)
	{
		toggleSetMethod.Invoke(instance, new object[]
		{
			value,
			sendCallback
		});
	}

	
	public static void Set(this Slider instance, float value, bool sendCallback = false)
	{
		sliderSetMethod.Invoke(instance, new object[]
		{
			value,
			sendCallback
		});
	}

	
	public static void Set(this Scrollbar instance, float value, bool sendCallback = false)
	{
		scrollbarSetMethod.Invoke(instance, new object[]
		{
			value,
			sendCallback
		});
	}

	
	public static void Set(this Dropdown instance, int value)
	{
		dropdownValueField.SetValue(instance, value);
		instance.RefreshShownValue();
	}

	
	private static MethodInfo FindSetMethod(Type objectType)
	{
		MethodInfo[] methods = objectType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
		for (int i = 0; i < methods.Length; i++)
		{
			if (methods[i].Name == "Set" && methods[i].GetParameters().Length == 2)
			{
				return methods[i];
			}
		}

		return null;
	}
}