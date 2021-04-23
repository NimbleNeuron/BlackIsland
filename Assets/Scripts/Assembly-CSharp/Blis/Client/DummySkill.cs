using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.DummySkill)]
	public class DummySkill : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}