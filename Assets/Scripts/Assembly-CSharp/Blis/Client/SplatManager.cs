using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class SplatManager : MonoBehaviour
	{
		public bool HideCursor;


		private readonly Dictionary<string, Splat> indicators = new Dictionary<string, Splat>();


		private Splat currentIndicator;


		protected Vector3 externalMousePosition;


		private Plane plane = new Plane(Vector3.up, Vector3.zero);


		public Splat CurrentIndicator => currentIndicator;


		private void Update()
		{
			if (HideCursor)
			{
				if (currentIndicator != null)
				{
					Cursor.visible = false;
					return;
				}

				Cursor.visible = true;
			}
		}


		public void SetIndicator(Splat indicator)
		{
			if (indicator == null)
			{
				return;
			}

			if (currentIndicator != indicator)
			{
				GameUI inst = MonoBehaviourInstance<GameUI>.inst;
				if (inst == null)
				{
					return;
				}

				MinimapUI minimap = inst.Minimap;
				if (minimap == null)
				{
					return;
				}

				if (minimap.UIMap == null)
				{
					return;
				}

				MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.HideIndicator();
				CancelIndicator();
				currentIndicator = indicator;
				currentIndicator.Manager = this;
			}

			if (currentIndicator == null)
			{
				return;
			}

			currentIndicator.Show();
		}


		public void CancelIndicator()
		{
			if (currentIndicator != null)
			{
				currentIndicator.Progress = 0f;
				currentIndicator.Hide();
				currentIndicator = null;
			}
		}


		public void SetExternalMousePosition(Vector3 mousePosition)
		{
			externalMousePosition = mousePosition;
		}


		public Vector3 GetExternalMousePosition()
		{
			return externalMousePosition;
		}


		public Vector3 Get3DMousePositionForLine(float maxRange)
		{
			if (maxRange <= 0.5f)
			{
				maxRange = 0.5f;
			}

			Vector3 vector = transform.position;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			plane.distance = -Vector3.Dot(plane.normal, transform.position);
			float distance;
			if (plane.Raycast(ray, out distance))
			{
				vector = ray.GetPoint(distance);
			}

			Vector3 origin = transform.position + GameUtil.DirectionOnPlane(transform.position, vector) * maxRange;
			origin.y = 100f;
			RaycastHit raycastHit;
			if (!Physics.Raycast(new Ray(origin, Vector3.down), out raycastHit, 1000f,
				GameConstants.LayerMask.GROUND_LAYER, QueryTriggerInteraction.Collide))
			{
				return vector;
			}

			Vector3 result;
			if (!MoveAgent.CanStandToPosition(raycastHit.point, 2147483640, out result))
			{
				return vector;
			}

			return result;
		}


		public Vector3 Get3DMousePosition()
		{
			plane.distance = -Vector3.Dot(plane.normal, transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float distance;
			if (plane.Raycast(ray, out distance))
			{
				return ray.GetPoint(distance);
			}

			return Vector3.zero;
		}


		public void InitIndicators(int characterCode)
		{
			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.CreateIndicators();
			foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
			{
				if (skillSlotSet != SkillSlotSet.None)
				{
					CreateCharacterSkillIndicators(characterCode, skillSlotSet);
				}
			}
		}


		private void CreateCharacterSkillIndicators(int characterCode, SkillSlotSet skillSlotSet)
		{
			foreach (SkillData skillData in GameDB.skill.GetCharacterSkills(characterCode, ObjectType.PlayerCharacter,
				skillSlotSet, 1))
			{
				CreateIndicator(skillData);
			}
		}


		public void CreateWeaponSkillIndicators(MasteryType masteryType)
		{
			foreach (SkillData skillData in GameDB.skill.GetWeaponSkills(masteryType, 1))
			{
				CreateIndicator(skillData);
			}
		}


		private void CreateIndicator(SkillData skillData)
		{
			if (!string.IsNullOrEmpty(skillData.Guideline))
			{
				Splat splat = FindIndicator(skillData.Guideline);
				if (splat != null)
				{
					return;
				}

				splat = Instantiate<GameObject>(
						SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorPrefab(skillData.Guideline),
						transform)
					.GetComponent<Splat>();
				splat.Progress = 0f;
				splat.Hide();
				indicators.Add(skillData.Guideline, splat);
			}

			if (!string.IsNullOrEmpty(skillData.SubGuideline))
			{
				Splat splat2 = FindIndicator(skillData.SubGuideline);
				if (splat2 != null)
				{
					return;
				}

				splat2 = Instantiate<GameObject>(
						SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorPrefab(skillData.SubGuideline),
						transform)
					.GetComponent<Splat>();
				splat2.Progress = 0f;
				splat2.Hide();
				indicators.Add(skillData.SubGuideline, splat2);
			}
		}


		private Splat FindIndicator(string indicatorName)
		{
			if (!indicators.ContainsKey(indicatorName))
			{
				return null;
			}

			return indicators[indicatorName];
		}


		public Splat GetIndicator(string indicatorName)
		{
			return indicators[indicatorName];
		}


		public bool IsCurrentIndicatorCheck(string indicatorName)
		{
			return indicators.ContainsKey(indicatorName) && !(currentIndicator == null) &&
			       indicators[indicatorName].Equals(currentIndicator);
		}


		public void CreateIndicator(string indicatorName)
		{
			if (!indicators.ContainsKey(indicatorName))
			{
				Splat component =
					Instantiate<GameObject>(
							SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorPrefab(indicatorName), transform)
						.GetComponent<Splat>();
				component.Progress = 0f;
				component.Hide();
				indicators.Add(indicatorName, component);
			}
		}
	}
}