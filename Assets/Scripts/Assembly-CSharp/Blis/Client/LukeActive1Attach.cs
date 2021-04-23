using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive1Attach)]
	public class LukeActive1Attach : LocalSkillScript
	{
		private RangeIndicator rangeIndicator;

		public override void Start()
		{
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				if (rangeIndicator == null)
				{
					InitActive1_2Indicator();
				}

				SetActive1_2Indicator(true);
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				SetActive1_2Indicator(false);
			}
		}


		private void InitActive1_2Indicator()
		{
			rangeIndicator = Object
				.Instantiate<GameObject>(
					SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorPrefab("RangeIndicator"))
				.GetComponent<RangeIndicator>();
			rangeIndicator.transform.SetParent(Self.transform);
			rangeIndicator.transform.localPosition = Vector3.zero;
			SkillData skillData =
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.GetSkillData(SkillSlotSet.Active1_2);
			if (skillData != null)
			{
				rangeIndicator.Range = skillData.range;
			}
		}


		private void SetActive1_2Indicator(bool active)
		{
			rangeIndicator.gameObject.SetActive(active);
		}
	}
}