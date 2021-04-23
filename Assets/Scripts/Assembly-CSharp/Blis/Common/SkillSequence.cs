namespace Blis.Common
{
	public class SkillSequence
	{
		private int currentSequence;


		public SkillSequence(int maxSequence)
		{
			MaxSequence = maxSequence;
		}


		public SkillSequence(SkillSequenceSnapshot snapshot)
		{
			currentSequence = snapshot.currentSequence;
			MaxSequence = snapshot.maxSequence;
		}


		public int MaxSequence { get; }


		public int Current()
		{
			return currentSequence;
		}


		public void Set(int seq)
		{
			currentSequence = seq;
			if (MaxSequence <= currentSequence)
			{
				Reset();
			}
		}


		public void Next()
		{
			currentSequence++;
			if (MaxSequence <= currentSequence)
			{
				Reset();
			}
		}


		public void Reset()
		{
			currentSequence = 0;
		}


		public bool IsPlaying()
		{
			return currentSequence > 0 && currentSequence < MaxSequence;
		}
	}
}