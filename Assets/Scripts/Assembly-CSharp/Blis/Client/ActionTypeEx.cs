using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public static class ActionTypeEx
	{
		public static bool IsCanNotAction(this ActionCategoryType canNotActionCategory, LocalObject interactionTarget)
		{
			if (canNotActionCategory == ActionCategoryType.None)
			{
				return false;
			}

			ObjectType objectType = interactionTarget.ObjectType;
			if (canNotActionCategory.HasFlag(ActionCategoryType.NoCastAction))
			{
				if (objectType == ObjectType.Item)
				{
					return true;
				}

				if (objectType == ObjectType.StaticItemBox)
				{
					return true;
				}

				LocalCharacter localCharacter = interactionTarget as LocalCharacter;
				if (localCharacter != null && !localCharacter.IsAlive)
				{
					return true;
				}
			}

			if (canNotActionCategory.HasFlag(ActionCategoryType.CastAction))
			{
				if (objectType == ObjectType.AirSupplyItemBox)
				{
					return true;
				}

				if (objectType == ObjectType.ResourceItemBox)
				{
					return true;
				}

				if (objectType == ObjectType.Hyperloop)
				{
					return true;
				}

				if (objectType == ObjectType.SecurityConsole)
				{
					return true;
				}
			}

			if (canNotActionCategory.HasFlag(ActionCategoryType.ResurrectAction))
			{
				LocalPlayerCharacter localPlayerCharacter = interactionTarget as LocalPlayerCharacter;
				if (localPlayerCharacter != null && localPlayerCharacter.IsDyingCondition &&
				    MonoBehaviourInstance<ClientService>.inst.MyPlayer.GetHostileType(localPlayerCharacter) ==
				    HostileType.Ally)
				{
					return true;
				}
			}

			return false;
		}
	}
}