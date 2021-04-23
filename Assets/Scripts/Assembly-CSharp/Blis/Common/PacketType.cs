namespace Blis.Common
{
	public enum PacketType
	{
		None,

		Handshake,

		TesterHandshake,

		ResHandshake,

		CommandList,

		CmdWarpTo,

		CmdStopMove,

		CmdRest,

		CmdStopAndInteract,

		CmdSwitchSkillSet,

		CmdStartSkill,

		CmdStartPassiveSkill,

		CmdStartStateSkill,

		CmdFinishSkill,

		CmdFinishPassiveSkill,

		CmdFinishStateSkill,

		CmdResetSkillCooldown,

		CmdStartSkillCooldown,

		CmdModifySkillCooldown,

		CmdHoldSkillCooldown,

		CmdStartConcentration,

		CmdEndConcentration,

		CmdCrowdControl,

		CmdAirborne,

		CmdLookAt,

		CmdLockRotation,

		CmdResetDestination,

		CmdAddState,

		CmdHeal,

		CmdHealEffectAndStateCode,

		CmdHealEffectCode,

		CmdHealStateCode,

		CmdHealHp,

		CmdHealSp,

		CmdHealHpStateCode,

		CmdHealSpStateCode,

		CmdHealHpEffectCode,

		CmdHealSpEffectCode,

		CmdHealHpEffectAndStateCode,

		CmdHealSpEffectAndStateCode,

		CmdSetExtraPoint,

		CmdDamage,

		CmdSpDamage,

		CmdSkillDamage,

		CmdBlock,

		CmdEvasion,

		CmdStartActionCasting,

		CmdCancelActionCasting,

		CmdActiveTrap,

		CmdBurstTrap,

		CmdUpgradeSkill,

		CmdEvolutionSkill,

		CmdUpdateSurvivableTime,

		CmdStartGunReload,

		CmdUpdateStat,

		CmdBroadcastUpdateStat,

		CmdKill,

		CmdDead,

		CmdRanking,

		CmdTeamRanking,

		CmdSurrenderGame,

		CmdUpdateState,

		CmdResetCreateTimeState,

		CmdRemoveState,

		CmdPauseState,

		CmdUpdateExp,

		CmdPlayPassiveSkill,

		CmdPlaySkillAction,

		CmdUpdateSkillPoint,

		CmdUpdateSkillEvolutionPoint,

		CmdResourceBoxChildReady,

		CmdResourceBoxChildActive,

		CmdUpdateResourceBoxCooldown,

		CmdUpdateCharacterInvisible,

		CmdUpdateCharacterMemorizer,

		CmdUpdateInCombat,

		CmdUpdateMastery,

		CmdMasteryLevelUp,

		CmdProjectileExplosion,

		CmdProjectileCollision,

		CmdProjectileCollisionWall,

		CmdAddSight,

		CmdRemoveSight,

		CmdAddItemCooldown,

		CmdRemoveItemCooldown,

		CmdFinishGunReload,

		CmdMoveStraight,

		CmdMoveToDestination,

		CmdMoveByDirection,

		CmdDyingCondition,

		CmdTeamRevival,

		CmdDestroy,

		CmdUpdateMoveSpeed,

		CmdUpdateMoveSpeedWhenMoving,

		CmdConsumeSkillCost,

		CmdUpdateLevel,

		CmdUpdateBullet,

		CmdUpdateShield,

		CmdMoveInCurve,

		CmdMoveStraightWithoutNav,

		CmdMoveToTargetWithoutNav,

		CmdConsoleCheckOut,

		CmdActiveConsoleSafeArea,

		CmdResetSkillSequence,

		CmdSetSkillSequence,

		CmdResetSummonDuration,

		CmdInstallRopeTrap,

		CmdHyperLoop,

		CmdConsoleAction,

		CmdCancelConsoleAction,

		CmdHyperLoopAction,

		CmdCancelHyperLoopAction,

		CmdActiveHyperLoopExit,

		CmdCancelHyperLoopExit,

		CmdEmotionCharacterVoice,

		CmdSpawn,

		CmdSpawns,

		CmdSystemChat,

		CmdUpdateMark,

		CmdPing,

		CmdUserDisconnected,

		CmdEmotionIcon,

		CmdFinishGameTeamAlive,

		CmdFinishGame,

		CmdChangeToObserver,

		CmdUserConnected,

		CmdInSight,

		CmdOutSight,

		CmdOnVisible,

		CmdOnInvisible,

		CmdInBush,

		CmdOutBush,

		RpcJoinUser,

		RpcSetupGame,

		RpcStartGame,

		RpcUpdateRestrictedArea,

		RpcStopRestrictedArea,

		RpcUpdateStrategy,

		RpcUpdateInventory,

		RpcUpdateEquipment,

		RpcCompleteMakeItem,

		RpcNoise,

		RpcOpenItemBox,

		RpcGameAnnounce,

		RpcNoticeWicklineSpawnStart,

		RpcNoticeWicklineMoveArea,

		RpcNoticeWicklineKilled,

		RpcFinishTutorial,

		RpcBattleResultKey,

		RpcExitGame,

		RpcItemBoxAdd,

		RpcItemBoxRemove,

		RpcBroadCastingUpdateBeforeStart,

		RpcUpdateBeforeStart,

		RpcError,

		RpcNoticeAirSupply,

		RpcSpawnAirSupply,

		RpcChat,

		RpcObserving,

		RpcToastMessage,

		RpcSkillSlotLock,

		RpcSkillReserveCancel,

		RpcVisitedNewArea,

		RpcFinishGame,

		ResError,

		ResSuccess,

		ReqReady,

		ReqJoin,

		ResJoin,

		ReqMoveTo,

		ReqWarpTo,

		ReqTarget,

		ReqStop,

		ResStop,

		ReqHold,

		ReqUseTargetSkill,

		ReqUsePointSkill,

		ReqPlayingSkillOnSelect,

		ResUseSkill,

		ResPickUpItem,

		ReqRest,

		ReqCheat,

		ReqCrowdControlCheat,

		ReqMonsterSpawnCheat,

		ReqEquipItem,

		ResEquipItem,

		ReqDropItemFromEquipment,

		ReqDropItemFromInventory,

		ReqDropItemPieceFromInventory,

		ReqTakeItem,

		ReqOpenCorpse,

		ResOpenCorpse,

		ReqPickUpItem,

		ReqOpenItemBox,

		ReqInsertItem,

		ReqSwapInvenItemSlot,

		ReqUseItem,

		ResUseItem,

		ReqMakeItem,

		ReqAdminCreateItem,

		ReqUpdateStrategySheet,

		ReqUpgradeSkill,

		ReqEvolutionSkill,

		ReqForceStartGame,

		ReqGunReload,

		ReqCreateDummy,

		ReqInstallSummon,

		ReqUnequipItem,

		ReqMissingCommand,

		ResMissingCommand,

		ReqCloseBox,

		ReqHyperloop,

		ReqSecurityConsoleAction,

		ReqEmotionCharacaterVoice,

		ReqWalkableArea,

		ResWalkableArea,

		ReqNextTutorialSequence,

		ReqAskGameSetup,

		ResAskGameSetup,

		ReqAskGameStart,

		ResAskGameStart,

		ResWorldSnapshot,

		ReqChat,

		ReqPing,

		ReqMark,

		ReqRemoveMark,

		ReqObserving,

		ReqItemPing,

		ReqSurrenderGame,

		ReqExitTeamGame,

		ResExitTeamGame,

		ReqEmotionIcon,

		ReqChangeToObserver,

		ReqGameSnapshot,

		ResGameSnapshot,

		ReqNicknamePair,

		ResNicknamePair,

		ReqIgnorePlayer,

		ResIgnorePlayer,

		ReqExitGame
	}
}