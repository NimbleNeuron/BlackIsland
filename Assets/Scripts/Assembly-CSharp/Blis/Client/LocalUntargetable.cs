using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.Untargetable)]
	public class LocalUntargetable : LocalSkillScript
	{
		public override void Start()
		{
			LocalCharacter.Pickable.ForcePickableDisable(true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			LocalCharacter.Pickable.ForcePickableDisable(false);
		}
	}
}