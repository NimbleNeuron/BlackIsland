using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqUseTargetSkill, true)]
	public class ReqUseTargetSkill : ReqPacketForResponse
	{
		[Key(1)] public SkillSlotSet skillSlotSet;


		[Key(2)] public int targetId;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldCharacter worldCharacter = null;
			if (!gameService.World.TryFind<WorldCharacter>(targetId, ref worldCharacter))
			{
				return new ResUseSkill
				{
					errorCode = 8
				};
			}

			if (playerSession.Character.IsRest)
			{
				return new ResUseSkill
				{
					errorCode = 5
				};
			}

			SkillData skillData = playerSession.Character.GetSkillData(skillSlotSet);
			if (worldCharacter as WorldSummonBase != null)
			{
				if (skillData == null)
				{
					return new ResUseSkill
					{
						errorCode = 7
					};
				}

				if (skillData.TargetType != SkillTargetType.NotSpecifiedAndSummonObject)
				{
					return new ResUseSkill
					{
						errorCode = 7
					};
				}
			}

			if (!worldCharacter.IsAlive)
			{
				return new ResUseSkill
				{
					errorCode = 9999
				};
			}

			if (worldCharacter.IsDyingCondition)
			{
				if (GameDB.skill.GetSkillGroupData(skillData.group) == null)
				{
					return new ResUseSkill
					{
						errorCode = 7
					};
				}

				if (GameDB.skill.GetSkillGroupData(skillData.group).impossibleDyingConditionTarget)
				{
					return new ResUseSkill
					{
						errorCode = 9999
					};
				}
			}

			int errorCode = (int) playerSession.Character.UseSkill(skillSlotSet, worldCharacter);
			return new ResUseSkill
			{
				errorCode = errorCode
			};
		}
	}
}