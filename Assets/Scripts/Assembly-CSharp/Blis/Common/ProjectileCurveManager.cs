using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	
	public class ProjectileCurveManager : MonoBehaviour
	{
		
		
		public static ProjectileCurveManager instance
		{
			get
			{
				if (ProjectileCurveManager._instance == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ProjectileCurveManager"));
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					ProjectileCurveManager._instance = gameObject.GetComponent<ProjectileCurveManager>();
				}
				return ProjectileCurveManager._instance;
			}
		}

		
		public AnimationCurve GetAnimationCurve(int code)
		{
			foreach (ProjectileCurveManager.CodeWithCurve codeWithCurve in this.curveDatas)
			{
				if (codeWithCurve.Code == code)
				{
					return codeWithCurve.Curve;
				}
			}
			return null;
		}

		
		public void InsertNewAnimationCurve(int newCode = 0)
		{
			using (List<ProjectileCurveManager.CodeWithCurve>.Enumerator enumerator = this.curveDatas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Code == newCode)
					{
						this.InsertNewAnimationCurve(++newCode);
						return;
					}
				}
			}
			this.curveDatas.Add(new ProjectileCurveManager.CodeWithCurve(newCode, AnimationCurve.Linear(0f, 0f, 1f, 1f)));
		}

		
		public bool CheckExist(int code)
		{
			using (List<ProjectileCurveManager.CodeWithCurve>.Enumerator enumerator = this.curveDatas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Code == code)
					{
						return true;
					}
				}
			}
			return false;
		}

		
		[SerializeField]
		private List<ProjectileCurveManager.CodeWithCurve> curveDatas = new List<ProjectileCurveManager.CodeWithCurve>();

		
		private static ProjectileCurveManager _instance;

		
		[Serializable]
		private class CodeWithCurve
		{
			
			
			public int Code
			{
				get
				{
					return this.code;
				}
			}

			
			
			public AnimationCurve Curve
			{
				get
				{
					return this.curve;
				}
			}

			
			public CodeWithCurve(int code, AnimationCurve curve)
			{
				this.code = code;
				this.curve = curve;
			}

			
			[SerializeField]
			private int code;

			
			[SerializeField]
			private AnimationCurve curve;
		}
	}
}
