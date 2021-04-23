using System.Linq;
using UnityEngine;

namespace Werewolf.StatusIndicators.Components
{
	public class SplatManager : MonoBehaviour
	{
		public bool HideCursor;


		public LayerMask ProjectorIgnore = 0;


		public LayerMask MouseRaycast = -1;


		
		public SpellIndicator[] SpellIndicators { get; set; }


		
		public StatusIndicator[] StatusIndicators { get; set; }


		
		public RangeIndicator[] RangeIndicators { get; set; }


		
		public SpellIndicator CurrentSpellIndicator { get; private set; }


		
		public StatusIndicator CurrentStatusIndicator { get; private set; }


		
		public RangeIndicator CurrentRangeIndicator { get; private set; }


		private void Update()
		{
			if (HideCursor)
			{
				if (CurrentSpellIndicator != null)
				{
					Cursor.visible = false;
					return;
				}

				Cursor.visible = true;
			}
		}


		private void OnEnable()
		{
			Initialize();
		}


		private void OnValidate() { }


		private void Initialize()
		{
			SpellIndicators = GetComponentsInChildren<SpellIndicator>();
			StatusIndicators = GetComponentsInChildren<StatusIndicator>();
			RangeIndicators = GetComponentsInChildren<RangeIndicator>();
			SpellIndicators.ToList<SpellIndicator>().ForEach(delegate(SpellIndicator x) { InitializeSplat(x); });
			StatusIndicators.ToList<StatusIndicator>().ForEach(delegate(StatusIndicator x) { InitializeSplat(x); });
			RangeIndicators.ToList<RangeIndicator>().ForEach(delegate(RangeIndicator x) { InitializeSplat(x); });
		}


		private void InitializeSplat(Splat splat)
		{
			splat.Manager = this;
			splat.Initialize();
			splat.gameObject.SetActive(false);
			UpdateProjectorIgnoreLayers(splat);
		}


		private void UpdateProjectorIgnoreLayers(Splat splat)
		{
			splat.Projectors.ToList<Projector>().ForEach(delegate(Projector x) { x.ignoreLayers = ProjectorIgnore; });
		}


		public Vector3 GetSpellCursorPosition()
		{
			if (CurrentSpellIndicator != null)
			{
				return CurrentSpellIndicator.transform.position;
			}

			return Get3DMousePosition();
		}


		public void SelectSpellIndicator(string splatName)
		{
			CancelSpellIndicator();
			SpellIndicator spellIndicator = GetSpellIndicator(splatName);
			if (spellIndicator.RangeIndicator != null)
			{
				spellIndicator.RangeIndicator.gameObject.SetActive(true);
				spellIndicator.RangeIndicator.OnShow();
			}

			spellIndicator.gameObject.SetActive(true);
			spellIndicator.OnShow();
			CurrentSpellIndicator = spellIndicator;
		}


		public void SelectStatusIndicator(string splatName)
		{
			CancelStatusIndicator();
			StatusIndicator statusIndicator = GetStatusIndicator(splatName);
			statusIndicator.gameObject.SetActive(true);
			statusIndicator.OnShow();
			CurrentStatusIndicator = statusIndicator;
		}


		public void SelectRangeIndicator(string splatName)
		{
			CancelRangeIndicator();
			RangeIndicator rangeIndicator = GetRangeIndicator(splatName);
			if (CurrentSpellIndicator != null && CurrentSpellIndicator.RangeIndicator == rangeIndicator)
			{
				CancelSpellIndicator();
			}

			rangeIndicator.gameObject.SetActive(true);
			rangeIndicator.OnShow();
			CurrentRangeIndicator = rangeIndicator;
		}


		public SpellIndicator GetSpellIndicator(string splatName)
		{
			return (from x in SpellIndicators
				where x.name == splatName
				select x).FirstOrDefault<SpellIndicator>();
		}


		public StatusIndicator GetStatusIndicator(string splatName)
		{
			return (from x in StatusIndicators
				where x.name == splatName
				select x).FirstOrDefault<StatusIndicator>();
		}


		public RangeIndicator GetRangeIndicator(string splatName)
		{
			return (from x in RangeIndicators
				where x.name == splatName
				select x).FirstOrDefault<RangeIndicator>();
		}


		public void CancelSpellIndicator()
		{
			if (CurrentSpellIndicator != null)
			{
				if (CurrentSpellIndicator.RangeIndicator != null)
				{
					CurrentSpellIndicator.RangeIndicator.gameObject.SetActive(false);
				}

				CurrentSpellIndicator.OnHide();
				CurrentSpellIndicator.gameObject.SetActive(false);
				CurrentSpellIndicator = null;
			}
		}


		public void CancelStatusIndicator()
		{
			if (CurrentStatusIndicator != null)
			{
				CurrentStatusIndicator.OnHide();
				CurrentStatusIndicator.gameObject.SetActive(false);
				CurrentStatusIndicator = null;
			}
		}


		public void CancelRangeIndicator()
		{
			if (CurrentRangeIndicator != null)
			{
				CurrentRangeIndicator.OnHide();
				CurrentRangeIndicator.gameObject.SetActive(false);
				CurrentRangeIndicator = null;
			}
		}


		public Vector3 Get3DMousePosition()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 300f, MouseRaycast))
			{
				return raycastHit.point;
			}

			return Vector3.zero;
		}
	}
}