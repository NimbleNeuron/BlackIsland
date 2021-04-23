using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpgradeSkill, false)]
	public class CmdUpgradeSkill : LocalPlayerCharacterCommandPacket
	{
		[Key(2)] public int skillPoint;


		[Key(1)] public SkillSlotIndex skillSlotIndex;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				MasteryType equipWeaponMasteryType = self.GetEquipWeaponMasteryType();
				self.UpdateSkillPoint(equipWeaponMasteryType, skillPoint);
				CharacterVoiceType charVoiceType =
					CharacterVoiceUtil.MasteryTypeConvertToCharacterVoiceType(equipWeaponMasteryType);
				self.CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15, self.GetPosition());
				if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
				{
					MonoBehaviourInstance<TutorialController>.inst.SuccessLearnWeaponSkill();
				}
			}
			else
			{
				self.UpdateSkillPoint(skillPoint);
			}

			self.UpgradeSkill(skillSlotIndex);
		}
	}
}