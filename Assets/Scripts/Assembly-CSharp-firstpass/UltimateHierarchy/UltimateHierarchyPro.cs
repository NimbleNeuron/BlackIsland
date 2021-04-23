using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateHierarchy
{
	[ExecuteInEditMode]
	public class UltimateHierarchyPro : MonoBehaviour
	{
		public static readonly float overlayButtonTransparency = 0.7f;


		public static readonly float sceneViewGizmoSize = 0.7f;


		[HideInInspector] public static List<UltimateHierarchyPro> allInstances = new List<UltimateHierarchyPro>();


		[HideInInspector] public static List<UltimateHierarchyPro> toggles = new List<UltimateHierarchyPro>();


		// [HideInInspector]
		public List<Slot> list = new List<Slot>();


		[HideInInspector] public Slot newSlot = new Slot();


		[HideInInspector] public Color colorGO = new Color(1f, 0.87f, 0.48f);


		[HideInInspector] public Color colorP = new Color(0.44f, 0.72f, 1f);


		[HideInInspector] public Color colorS = new Color(1f, 0.35f, 0.55f);


		[HideInInspector] public Color colorC = new Color(0.26f, 0.85f, 0.36f);


		[HideInInspector] public Texture2D textureIcon;


		[HideInInspector] public Texture2D textureIconHover;


		[HideInInspector] public float GUIwidth;


		[HideInInspector] public bool stylesSet;


		public string toggleName = "";


		public bool showGameHoverGUI = true;


		public bool showGameListGUI = true;


		public bool showSceneWiresGUI = true;


		public bool showSceneListGUI = true;


		[HideInInspector] private bool hiddenStateChanged;


		[HideInInspector] private Texture2D textureC;


		[HideInInspector] private Texture2D textureGO;


		[HideInInspector] private Texture2D textureP;


		[HideInInspector] private Texture2D textureS;


		private void Awake()
		{
			textureInit();
			checkNoName();
		}


		private void Update()
		{
			if (!hiddenStateChanged)
			{
				return;
			}

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].objectType == Slot.ObjectType.Gameobject)
				{
					if (list[i].isHidden)
					{
						list[i].obj.hideFlags = list[i].obj.hideFlags | HideFlags.HideInHierarchy;
					}
					else
					{
						list[i].obj.hideFlags = list[i].obj.hideFlags & ~HideFlags.HideInHierarchy;
					}

					list[i].obj.SetActive(!list[i].obj.activeSelf);
					list[i].obj.SetActive(!list[i].obj.activeSelf);
				}
			}

			hiddenStateChanged = false;
		}


		private void OnEnable()
		{
			checkNoName();
			if (!allInstances.Contains(this))
			{
				allInstances.Add(this);
			}
		}


		private void OnDestroy()
		{
			if (toggles.Contains(this))
			{
				toggles.Remove(this);
			}

			for (int i = 0; i < list.Count; i++)
			{
				setHide(i, false);
			}

			if (allInstances.Contains(this))
			{
				allInstances.Remove(this);
			}
		}


		private void OnDrawGizmos()
		{
			int popularType = getPopularType();
			if (popularType == 1)
			{
				Gizmos.color = colorP;
			}
			else if (popularType == 2)
			{
				Gizmos.color = colorS;
			}
			else if (popularType == 3)
			{
				Gizmos.color = colorC;
			}
			else
			{
				Gizmos.color = colorGO;
			}

			Gizmos.DrawSphere(gameObject.transform.position, sceneViewGizmoSize);
			if (!showSceneWiresGUI)
			{
				return;
			}

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].obj == null)
				{
					list.RemoveAt(i);
				}

				if (list[i].objectType == Slot.ObjectType.Gameobject || list[i].objectType == Slot.ObjectType.Script)
				{
					if (list[i].objectType == Slot.ObjectType.Gameobject)
					{
						if (getActive(i))
						{
							Gizmos.color = colorGO;
						}
						else
						{
							Gizmos.color = (colorGO + Color.black) / 2f;
						}
					}
					else if (list[i].objectType == Slot.ObjectType.Script)
					{
						if (getUpdate(i))
						{
							Gizmos.color = colorS;
						}
						else
						{
							Gizmos.color = (colorS + Color.black) / 2f;
						}
					}
					else if (getUpdate(i))
					{
						Gizmos.color = colorC;
					}
					else
					{
						Gizmos.color = (colorC + Color.black) / 2f;
					}

					Gizmos.DrawLine(transform.position, list[i].obj.transform.position);
					Gizmos.DrawSphere(list[i].obj.transform.position, 0.1f);
				}
			}
		}


		private void textureInit()
		{
			textureGO = new Texture2D(1, 1);
			textureGO.SetPixel(0, 0, colorGO);
			textureGO.Apply();
			textureP = new Texture2D(1, 1);
			textureP.SetPixel(0, 0, colorP);
			textureP.Apply();
			textureS = new Texture2D(1, 1);
			textureS.SetPixel(0, 0, colorS);
			textureS.Apply();
			textureC = new Texture2D(1, 1);
			textureC.SetPixel(0, 0, colorC);
			textureC.Apply();
		}


		public Texture2D getTexture(Slot.ObjectType type)
		{
			switch (type)
			{
				case Slot.ObjectType.Prefab:
					return textureP;
				case Slot.ObjectType.Script:
					return textureS;
				case Slot.ObjectType.Comp:
					return textureC;
				default:
					return textureGO;
			}
		}


		public void updateTexture(Slot.ObjectType type)
		{
			if (textureGO == null || textureP == null || textureS == null || textureC == null)
			{
				textureInit();
			}

			switch (type)
			{
				case Slot.ObjectType.Prefab:
					textureP.SetPixel(0, 0, colorP);
					textureP.Apply();
					return;
				case Slot.ObjectType.Script:
					textureS.SetPixel(0, 0, colorS);
					textureS.Apply();
					return;
				case Slot.ObjectType.Comp:
					textureC.SetPixel(0, 0, colorC);
					textureC.Apply();
					return;
				default:
					textureGO.SetPixel(0, 0, colorGO);
					textureGO.Apply();
					return;
			}
		}


		private void checkNoName()
		{
			if (toggleName == "")
			{
				adjustName();
			}
		}


		private void adjustName()
		{
			toggleName = gameObject.name;
			UltimateHierarchyPro[] components = gameObject.GetComponents<UltimateHierarchyPro>();
			if (components.Length > 1)
			{
				toggleName = string.Concat(toggleName, " (", components.Length, ")");
			}
		}


		public int getPopularType()
		{
			int[] array = new int[4];
			for (int i = 0; i < list.Count; i++)
			{
				array[(int) list[i].objectType]++;
			}

			int result = 0;
			int num = 0;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] > num)
				{
					result = j;
					num = array[j];
				}
			}

			return result;
		}


		public void setStyles()
		{
			if (stylesSet)
			{
				return;
			}

			stylesSet = true;
		}


		public bool getGameHoverGUI()
		{
			return showGameHoverGUI;
		}


		public void setGameHoverGUI(bool b)
		{
			showGameHoverGUI = b;
		}


		public void toggleGameHoverGUI()
		{
			showGameHoverGUI = !showGameHoverGUI;
		}


		public bool getGameListGUI()
		{
			return showGameListGUI;
		}


		public void setGameListGUI(bool b)
		{
			showGameListGUI = b;
		}


		public void toggleGameListGUI()
		{
			showGameListGUI = !showGameListGUI;
		}


		public bool getSceneWiresGUI()
		{
			return showSceneWiresGUI;
		}


		public void setSceneWiresGUI(bool b)
		{
			showSceneWiresGUI = b;
		}


		public void toggleSceneWiresGUI()
		{
			showSceneWiresGUI = !showSceneWiresGUI;
		}


		public bool getSceneListGUI()
		{
			return showSceneListGUI;
		}


		public void setSceneListGUI(bool b)
		{
			showSceneListGUI = b;
		}


		public void toggleSceneListGUI()
		{
			showSceneListGUI = !showSceneListGUI;
		}


		public void addSlot(Slot slot)
		{
			if (!list.Contains(slot))
			{
				list.Add(slot);
			}
		}


		public void addGameObject(GameObject go)
		{
			addSlot(new Slot
			{
				obj = go
			});
		}


		public void removeGameObject(GameObject go)
		{
			if (list == null || list.Count == 0)
			{
				return;
			}

			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].obj == go)
				{
					list.Remove(list[i]);
				}
			}
		}


		public void delete(int id)
		{
			if (id >= 0 && id < list.Count)
			{
				if (list[id].obj != null && list[id].objectType == Slot.ObjectType.Gameobject)
				{
					list[id].obj.hideFlags = list[id].obj.hideFlags & ~HideFlags.HideInHierarchy;
					list[id].obj.SetActive(!list[id].obj.activeSelf);
					list[id].obj.SetActive(!list[id].obj.activeSelf);
				}

				list.Remove(list[id]);
				return;
			}

			Debug.LogError(string.Concat(this, " Slot [", id, "] is out of list index."));
		}


		public bool getSolo(int id)
		{
			if (list[id].objectType == Slot.ObjectType.Comp)
			{
				Debug.LogError(this + " Components can not be enabled / disabled.");
				return false;
			}

			if (list[id].objectType == Slot.ObjectType.Script)
			{
				if (!list[id].script.enabled)
				{
					return false;
				}
			}
			else if (!list[id].obj.activeSelf)
			{
				return false;
			}

			bool result = true;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].objectType == list[id].objectType && i != id)
				{
					if (list[i].objectType == Slot.ObjectType.Script)
					{
						if (list[i].script.enabled)
						{
							result = false;
						}
					}
					else if (list[i].obj != null && list[i].obj.activeSelf)
					{
						result = false;
					}
				}
			}

			return result;
		}


		public void setSolo(int id, bool b)
		{
			if (list[id].objectType == Slot.ObjectType.Comp)
			{
				Debug.LogError(this + " Components can not be enabled / disabled.");
				return;
			}

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].objectType == list[id].objectType)
				{
					if (list[id].objectType == Slot.ObjectType.Script)
					{
						list[i].script.GetComponent<Behaviour>().enabled = !b;
					}
					else
					{
						setActive(i, !b);
					}
				}
			}

			if (list[id].objectType == Slot.ObjectType.Script)
			{
				list[id].script.GetComponent<Behaviour>().enabled = b;
				return;
			}

			setActive(id, b);
		}


		public void toggleSolo(int id)
		{
			if (list[id].objectType == Slot.ObjectType.Comp)
			{
				Debug.LogError(this + " Components can not be enabled / disabled.");
				return;
			}

			setSolo(id, !getSolo(id));
		}


		public bool getActive(int id)
		{
			return list[id].obj != null &&
			       (list[id].objectType == Slot.ObjectType.Gameobject ||
			        list[id].objectType == Slot.ObjectType.Prefab) && list[id].obj.activeSelf;
		}


		public void setActive(int id, bool b)
		{
			if (!(list[id].obj != null))
			{
				Debug.LogError(string.Concat(this, " Slot [", id, "] has no GameObject."));
				return;
			}

			if (list[id].objectType == Slot.ObjectType.Gameobject || list[id].objectType == Slot.ObjectType.Prefab)
			{
				list[id].obj.SetActive(b);
				return;
			}

			Debug.LogError(string.Concat(this, " Slot [", id,
				"] is not a GameObject or Prefab and can not toggle Active."));
		}


		public void toggleActive(int id)
		{
			if (!(list[id].obj != null))
			{
				Debug.LogError(string.Concat(this, " Slot [", id, "] has no GameObject."));
				return;
			}

			if (list[id].objectType == Slot.ObjectType.Gameobject || list[id].objectType == Slot.ObjectType.Prefab)
			{
				setActive(id, !getActive(id));
				return;
			}

			Debug.LogError(string.Concat(this, " Slot [", id,
				"] is not a GameObject or Prefab and can not toggle Active."));
		}


		public bool getUpdate(int id)
		{
			if (list[id].obj != null)
			{
				if (list[id].objectType == Slot.ObjectType.Script)
				{
					Slot slot = list[id];
					Behaviour script = slot.script;
					if (script == null)
					{
						return false;
					}

					return script.enabled;
				}

				Debug.LogError(string.Concat(this, " Slot [", id, "] is not a Script and can not toggle Update."));
			}

			return false;
		}


		public void setUpdate(int id, bool b)
		{
			if (list[id].obj != null)
			{
				if (list[id].objectType == Slot.ObjectType.Script)
				{
					try
					{
						list[id].script.enabled = b;
						return;
					}
					catch (Exception ex)
					{
						Debug.LogWarning(ex);
						return;
					}
				}

				Debug.LogError(string.Concat(this, " Slot [", id, "] is not a Script and can not toggle Update."));
			}
		}


		public void toggleUpdate(int id)
		{
			if (list[id].obj != null)
			{
				if (list[id].objectType == Slot.ObjectType.Script)
				{
					setUpdate(id, !getUpdate(id));
					return;
				}

				Debug.LogError(string.Concat(this, " Slot [", id, "] is not a Script and can not toggle Update."));
			}
		}


		public bool getHide(int id)
		{
			if (list[id].obj != null)
			{
				if (list[id].objectType == Slot.ObjectType.Gameobject)
				{
					if (list[id].obj.hideFlags == HideFlags.HideInHierarchy)
					{
						return true;
					}

					HideFlags hideFlags = list[id].obj.hideFlags;
					return false;
				}

				Debug.LogError(string.Concat(this, " Slot [", id, "] is not a GameObject and can not toggle Hide."));
			}

			return false;
		}


		public void setHide(int id, bool b)
		{
			if (list[id].obj != null && list[id].objectType == Slot.ObjectType.Gameobject)
			{
				list[id].isHidden = b;
			}

			hiddenStateChanged = true;
		}


		public void toggleHide(int id)
		{
			if (list[id].obj != null)
			{
				if (list[id].objectType == Slot.ObjectType.Gameobject)
				{
					setHide(id, !getHide(id));
					return;
				}

				Debug.LogError(string.Concat(this, " Slot [", id, "] is not a GameObject and can not toggle Hide."));
			}
		}


		public void setHideAll(bool b)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].objectType == Slot.ObjectType.Gameobject)
				{
					setHide(i, b);
				}
			}
		}


		public void setActiveUpdateAll(Slot.ObjectType type, bool b)
		{
			if (type == Slot.ObjectType.Comp)
			{
				Debug.LogError(this + " Components can not be enabled / disabled.");
				return;
			}

			for (int i = 0; i < list.Count; i++)
			{
				if (list == null || list[i] == null || list[i].obj == null) continue;
				
				if (list[i].objectType == type)
				{
					if (list[i].objectType == Slot.ObjectType.Script)
					{
						setUpdate(i, b);
					}
					else
					{
						list[i].obj.SetActive(b);
						setActive(i, b);
					}
				}
			}
		}


		public void flipAll(Slot.ObjectType type)
		{
			if (type == Slot.ObjectType.Comp)
			{
				Debug.LogError(this + " Components can not be enabled / disabled.");
				return;
			}

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].objectType == type)
				{
					if (list[i].objectType == Slot.ObjectType.Script)
					{
						toggleUpdate(i);
					}
					else
					{
						toggleActive(i);
					}
				}
			}
		}


		public bool hasObjectsOfType(Slot.ObjectType type)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].objectType == type)
				{
					return true;
				}
			}

			return false;
		}


		public bool getTypeIdStartEnd(Slot.ObjectType type, ref int first, ref int last)
		{
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].objectType == type && !flag)
				{
					flag = true;
					first = i;
				}
				else if (list[i].objectType != type && flag)
				{
					last = i - 1;
				}
			}

			return flag;
		}


		public static string getShortString(string inputString)
		{
			string text = inputString[0].ToString().ToUpper();
			for (int i = 1; i < inputString.Length; i++)
			{
				if (inputString[i].ToString() == inputString[i].ToString().ToUpper())
				{
					text += inputString[i].ToString().ToUpper();
				}
			}

			return text;
		}


		public void setColor(Slot.ObjectType type, Color color)
		{
			switch (type)
			{
				case Slot.ObjectType.Prefab:
					colorP = color;
					return;
				case Slot.ObjectType.Script:
					colorS = color;
					return;
				case Slot.ObjectType.Comp:
					colorC = color;
					return;
				default:
					colorGO = color;
					return;
			}
		}


		public bool getGameObjectId(Slot.ObjectType type, GameObject go, ref int id)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (type == list[i].objectType && list[i].obj == go)
				{
					id = i;
					return true;
				}
			}

			return false;
		}


		public int getListLength()
		{
			return list.Count;
		}


		public List<Slot> getList()
		{
			return list;
		}


		public Slot getSlot(int id)
		{
			return list[id];
		}
	}
}