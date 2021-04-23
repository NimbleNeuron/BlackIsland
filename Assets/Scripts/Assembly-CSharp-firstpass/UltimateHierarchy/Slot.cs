using System;
using UnityEngine;

namespace UltimateHierarchy
{
	[Serializable]
	public class Slot
	{
		[HideInInspector]
		public enum ObjectType
		{
			Gameobject,


			Prefab,


			Script,


			Comp
		}


		public GameObject obj;


		[HideInInspector] public Behaviour script;


		[HideInInspector] public Component comp;


		[HideInInspector] public bool deleteQuestion;


		[HideInInspector] public ObjectType objectType;


		// [HideInInspector]
		public Component[] comps;


		[HideInInspector] public string[] componentNames;


		[HideInInspector] public int type;


		public bool showTransform;


		public bool showVariables;


		public bool isHidden;
	}
}