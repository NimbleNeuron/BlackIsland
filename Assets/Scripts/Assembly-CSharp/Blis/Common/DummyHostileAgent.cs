namespace Blis.Common
{
	public class DummyHostileAgent : HostileAgent
	{
		public DummyHostileAgent(ObjectBase obj) : base(obj) { }


		public override HostileType GetHostileType(HostileAgent target)
		{
			return HostileType.Enemy;
		}
	}
}