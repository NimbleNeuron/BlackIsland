using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class PickPointThenDirectionIndicator : SpellIndicator
	{
		[SerializeField] private GameObject Head = default;


		[SerializeField] private GameObject Body = default;


		[SerializeField] private RangeIndicator OuterRangeIndicator = default;


		[SerializeField] protected Transform lineAnchor = default;


		private readonly float angularSpeed = 40f;


		private Transform anchor = default;


		private float drawLineRange;


		private float lineRange = -1f;


		private Vector3? lineStartPosition;


		private IndicatorPart partBody;


		private IndicatorPart partHead;


		private float skillRange = -1f;


		private StatePickPointThenDirectionIndicator state;


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
			if (fixedDirection == null)
			{
				if (Manager != null)
				{
					if (lineStartPosition != null)
					{
						Vector3 direction = Manager.transform.forward;
						if (!GameUtil.ApproximatelyToPlane(Manager.GetExternalMousePosition(), lineStartPosition.Value,
							0.01f))
						{
							direction = GameUtil.DirectionOnPlane(lineStartPosition.Value,
								Manager.GetExternalMousePosition());
						}
						else if (!GameUtil.ApproximatelyToPlane(Manager.transform.position, lineStartPosition.Value,
							0.01f))
						{
							direction = GameUtil.DirectionOnPlane(Manager.transform.position, lineStartPosition.Value);
						}
						else
						{
							direction = Manager.transform.forward;
						}

						lineAnchor.rotation = GameUtil.LookRotation(direction);
						lineAnchor.position = lineStartPosition.Value;
					}
					else
					{
						Vector3 forward = Manager.transform.forward;
						lineAnchor.rotation = GameUtil.LookRotation(forward);
						lineAnchor.localPosition = Vector3.zero;
					}

					StatePickPointThenDirectionIndicator statePickPointThenDirectionIndicator = state;
					if (statePickPointThenDirectionIndicator != StatePickPointThenDirectionIndicator.PickBefore)
					{
						if (statePickPointThenDirectionIndicator != StatePickPointThenDirectionIndicator.PickAfter)
						{
							return;
						}

						if (!lineAnchor.gameObject.activeSelf)
						{
							lineAnchor.gameObject.SetActive(true);
						}
					}
					else if (lineAnchor.gameObject.activeSelf)
					{
						lineAnchor.gameObject.SetActive(false);
						lineStartPosition = null;
						return;
					}
				}

				return;
			}

			lineAnchor.rotation = GameUtil.LookRotation(fixedDirection.Value);
			if (lineStartPosition != null)
			{
				lineAnchor.position = lineStartPosition.Value;
				return;
			}

			lineAnchor.localPosition = Vector3.zero;
		}


		public void OnEnable()
		{
			if (fixedDirection != null)
			{
				Vector3 value = fixedDirection.Value;
				lineAnchor.rotation = GameUtil.LookRotation(value);
			}

			lineAnchor.gameObject.SetActive(state == StatePickPointThenDirectionIndicator.PickAfter);
		}


		public void OnDisable()
		{
			state = StatePickPointThenDirectionIndicator.PickBefore;
			lineStartPosition = null;
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
			DrawLine(length == -1f ? skillRange : length);
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


		protected override void UpdateMaxRange(float maxRange)
		{
			if (OuterRangeIndicator != null)
			{
				OuterRangeIndicator.Range = maxRange;
			}
		}


		public void SetLineStartPosition(Vector3 lineStartPosition)
		{
			this.lineStartPosition = lineStartPosition;
			lineAnchor.position = lineStartPosition;
		}


		public void SetIndicatorState(StatePickPointThenDirectionIndicator state)
		{
			this.state = state;
		}

		private void Ref()
		{
			Reference.Use(angularSpeed);
		}
	}
}