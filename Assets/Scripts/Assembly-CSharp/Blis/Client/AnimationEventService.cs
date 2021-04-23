using Blis.Common;

namespace Blis.Client
{
	public class AnimationEventService : SingletonMonoBehaviour<AnimationEventService>
	{
		public AnimationCollection AnimationCollection { get; } = new AnimationCollection();
	}
}