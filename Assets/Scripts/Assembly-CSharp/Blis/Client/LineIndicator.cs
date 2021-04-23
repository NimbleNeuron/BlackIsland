using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LineIndicator : SpellIndicator
	{
		[SerializeField] private GameObject Head = default;


		[SerializeField] private GameObject Body = default;


		[SerializeField] private RangeIndicator OuterRangeIndicator = default;


		[SerializeField] protected Transform lineAnchor = default;


		private readonly float angularSpeed = 40f;


		private Transform anchor = default;


		private float drawLineRange;


		private float lineRange = -1f;


		private bool lineRangeOnMouse = default;


		private IndicatorPart partBody;


		private IndicatorPart partHead;


		private float skillRange = -1f;


		protected GameObject GetHead => Head;


		protected GameObject GetBody => Body;


		public override void Awake()
		{
			base.Awake();
			partHead = Head.GetComponent<IndicatorPart>();
			partBody = Body.GetComponent<IndicatorPart>();
		}


		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (fixedDirection != null)
			{
				Vector3 value = fixedDirection.Value;
				lineAnchor.rotation = GameUtil.LookRotation(value);
				return;
			}

			if (Manager != null)
			{
				Vector3 mousePosition = GetMousePosition();
				DrawLineRangeOnMouse(mousePosition);
				Vector3 vector = mousePosition - lineAnchor.position;
				if (vector != Vector3.zero)
				{
					Quaternion quaternion = GameUtil.LookRotation(vector);
					Vector3 eulerAngles = Quaternion.Lerp(lineAnchor.rotation, GameUtil.LookRotation(vector),
						Time.deltaTime * angularSpeed).eulerAngles;
					eulerAngles.y = quaternion.eulerAngles.y;
					lineAnchor.eulerAngles = eulerAngles;
				}
			}
		}


		public void OnEnable()
		{
			if (fixedDirection != null)
			{
				Vector3 value = fixedDirection.Value;
				lineAnchor.rotation = GameUtil.LookRotation(value);
				return;
			}

			if (Manager != null)
			{
				Vector3 vector = !lineRangeOnMouse
					? Manager.Get3DMousePositionForLine(MaxRange)
					: Manager.Get3DMousePosition();
				DrawLineRangeOnMouse(vector);
				Vector3 vector2 = vector - lineAnchor.position;
				if (vector2 != Vector3.zero)
				{
					lineAnchor.rotation = GameUtil.LookRotation(vector2);
				}
			}
		}


		protected virtual Vector3 GetMousePosition()
		{
			return Manager.Get3DMousePositionForLine(Mathf.Max(maxRange, range));
		}


		protected override void UpdateProgress(float progress)
		{
			if (Range <= 0f)
			{
				return;
			}

			if (!Application.isPlaying)
			{
				return;
			}

			if (progress <= 0f)
			{
				partHead.SetShaderFloat("_Fill", 0f);
				partBody.SetShaderFloat("_Fill", 0f);
				return;
			}

			float num = Head.transform.localScale.y / Range;
			float num2 = 1f - num;
			partHead.SetShaderFloat("_Fill", progress < num2 ? 0f : (progress - num2) / num);
			partBody.SetShaderFloat("_Fill", progress < num2 ? progress / num2 * 0.5f + 0.5f : 1f);
		}


		protected override void UpdateWidth(float width)
		{
			if (anchor == null)
			{
				anchor = transform.Find("Anchor");
			}

			anchor.localScale = new Vector3(width, 1f, 1f);
		}


		protected override void UpdateRange(float skillRange)
		{
			base.UpdateRange(skillRange);
			this.skillRange = skillRange;
		}


		protected override void UpdateLength(float length)
		{
			base.UpdateLength(length);
			lineRange = length;
			if (lineRangeOnMouse && Manager != null)
			{
				if (!DrawLineRangeOnMouse(Manager.Get3DMousePosition()))
				{
					DrawLine(length == -1f ? skillRange : length);
				}
			}
			else
			{
				DrawLine(length == -1f ? skillRange : length);
			}
		}


		protected void DrawLine(float lineRange)
		{
			if (lineRange < 1f)
			{
				lineRange = 1f;
			}

			if (drawLineRange == lineRange)
			{
				return;
			}

			drawLineRange = lineRange;
			Head.transform.localPosition = new Vector3(0f, lineRange - 1f + 0.5f, 0f);
			Body.transform.localScale = new Vector3(1f, (lineRange - 1f) * 2f, 1f);
		}


		private bool DrawLineRangeOnMouse(Vector3 vec3MousePos)
		{
			if (!lineRangeOnMouse)
			{
				return false;
			}

			Vector2 b = new Vector2(vec3MousePos.x, vec3MousePos.z);
			Vector3 position = lineAnchor.position;
			float num = Vector2.Distance(new Vector2(position.x, position.z), b);
			float num2;
			if (lineRange == -1f)
			{
				num2 = num < skillRange ? num : skillRange;
			}
			else
			{
				num2 = num < lineRange ? num : lineRange;
			}

			DrawLine(num2);
			return true;
		}


		protected override void UpdateMaxRange(float maxRange)
		{
			if (OuterRangeIndicator != null)
			{
				OuterRangeIndicator.Range = maxRange;
			}
		}


		public void SetLineRangeOnMouse(bool lineRangeOnMouse)
		{
			if (lineRangeOnMouse == this.lineRangeOnMouse)
			{
				return;
			}

			this.lineRangeOnMouse = lineRangeOnMouse;
			if (lineRangeOnMouse && Manager != null)
			{
				DrawLineRangeOnMouse(Manager.Get3DMousePosition());
				return;
			}

			DrawLine(lineRange == -1f ? skillRange : lineRange);
		}
	}
}