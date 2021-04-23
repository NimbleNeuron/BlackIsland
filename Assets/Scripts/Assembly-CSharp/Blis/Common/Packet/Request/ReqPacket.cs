using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[Union(0, typeof(Ready))]
	[Union(1, typeof(ReqForceStartGame))]
	[Union(2, typeof(ReqMoveTo))]
	[Union(3, typeof(ReqWarpTo))]
	[Union(4, typeof(ReqTarget))]
	[Union(5, typeof(ReqPlayingSkillOnSelect))]
	[Union(7, typeof(ReqHold))]
	[Union(8, typeof(ReqCheat))]
	[Union(9, typeof(ReqEmotionCharacterVoice))]
	[Union(11, typeof(ReqUnequipItem))]
	[Union(12, typeof(ReqDropItemFromInventory))]
	[Union(13, typeof(ReqDropItemPieceFromInventory))]
	[Union(14, typeof(ReqDropItemFromEquipment))]
	[Union(15, typeof(ReqTakeItem))]
	[Union(16, typeof(ReqInsertItem))]
	[Union(17, typeof(ReqSwapInvenItemSlot))]
	[Union(18, typeof(ReqInstallSummon))]
	[Union(19, typeof(ReqAdminCreateItem))]
	[Union(20, typeof(ReqRest))]
	[Union(21, typeof(ReqOpenItemBox))]
	[Union(22, typeof(ReqMakeItem))]
	[Union(23, typeof(ReqUpdateStrategySheet))]
	[Union(24, typeof(ReqUpgradeSkill))]
	[Union(25, typeof(ReqEvolutionSkill))]
	[Union(26, typeof(ReqCreateDummy))]
	[Union(27, typeof(ReqGunReload))]
	[Union(28, typeof(ReqHyperloop))]
	[Union(29, typeof(ReqSecurityConsoleAction))]
	[Union(30, typeof(ReqNextTutorialSequence))]
	[Union(31, typeof(ReqChat))]
	[Union(32, typeof(ReqPingTarget))]
	[Union(33, typeof(ReqMark))]
	[Union(34, typeof(ReqRemoveMark))]
	[Union(35, typeof(ReqItemPing))]
	[Union(36, typeof(ReqReqObserving))]
	[Union(37, typeof(ReqSurrenderGame))]
	[Union(38, typeof(ReqExitTeamGame))]
	[Union(39, typeof(ReqEmotionIcon))]
	[Union(40, typeof(ReqChangeToObserver))]
	[MessagePackObject]
	public abstract class ReqPacket : IPacket
	{
		[IgnoreMember] protected const int LAST_KEY_IDX = -1;


		public abstract void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession);


		public virtual void Action(GameServer gameServer, GameService gameService, ObserverSession observerSession) { }
	}
}