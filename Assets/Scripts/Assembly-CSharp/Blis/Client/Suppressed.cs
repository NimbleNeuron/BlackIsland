using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.Suppressed)]
	public class Suppressed : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}