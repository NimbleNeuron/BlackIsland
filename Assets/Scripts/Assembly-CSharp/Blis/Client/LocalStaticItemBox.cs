using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.StaticItemBox)]
	public class LocalStaticItemBox : LocalItemBox
	{
		private PositionTweener openEventTween;
		private Vector3 originPosition;

		private void Awake()
		{
			openEventTween = gameObject.GetComponent<PositionTweener>();
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.StaticItemBox;
		}


		public new void Init(int itemSpawnPointCode)
		{
			base.Init(itemSpawnPointCode);
			originPosition = transform.position;
		}


		public override void OpenBox()
		{
			PlayOpen();
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override void Init(byte[] snapshotData) { }


		private void PlayOpen()
		{
			openEventTween.from = originPosition;
			openEventTween.to = originPosition;
			PositionTweener positionTweener = openEventTween;
			positionTweener.to.x = positionTweener.to.x + 0.2f;
			openEventTween.PlayAnimation();
			string openBoxSoundName = GameDB.effectAndSound.GetOpenBoxSoundName(firstChildBoxName);
			if (!string.IsNullOrEmpty(openBoxSoundName))
			{
				Singleton<SoundControl>.inst.PlayFXSound(openBoxSoundName, "OpenBox", 15, transform.position, false);
			}
		}
	}
}