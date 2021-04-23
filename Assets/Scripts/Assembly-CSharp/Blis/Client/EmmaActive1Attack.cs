using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaActive1Attack)]
	public class EmmaActive1Attack : LocalEmmaSkillScript
	{
		public override void Start()
		{
			base.Start();
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
		}


		public override void Finish(bool cancel)
		{
			base.Finish(cancel);
		}
	}
}