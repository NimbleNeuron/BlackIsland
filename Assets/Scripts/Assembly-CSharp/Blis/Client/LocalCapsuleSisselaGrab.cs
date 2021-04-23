using Blis.Common.Utils;

namespace Blis.Client
{
	public abstract class LocalCapsuleSisselaGrab : Grab
	{
		private readonly int preStateCasterId = -1;
		private LocalSummonServant _wilson;


		protected LocalSummonServant wilson {
			get
			{
				if (_wilson == null)
				{
					SetWilson();
				}
				else if (preStateCasterId != CasterId)
				{
					SetWilson();
				}

				return _wilson;
			}
		}


		private void SetWilson()
		{
			LocalPlayerCharacter owner =
				MonoBehaviourInstance<ClientService>.inst.World.Find<LocalPlayerCharacter>(CasterId);
			_wilson = LocalSisselaSkill.GetWilson(owner);
		}
	}
}