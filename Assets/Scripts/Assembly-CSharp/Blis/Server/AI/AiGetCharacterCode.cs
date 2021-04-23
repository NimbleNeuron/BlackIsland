using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터 코드를 가져와서 characterCode에 저장한다.")]
	public class AiGetCharacterCode : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			this.characterCode.value = base.agent.CharacterCode;
			base.EndAction(true);
		}

		
		public BBParameter<int> characterCode;
	}
}
