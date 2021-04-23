using System.Collections.Generic;
using Blis.Common;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("Range 범위 안에 유저 or 봇이 있으면 공격한다.")]
	public class AiScanTarget : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			float num = (float)this.range;
			if (this.useAutoRange)
			{
				num = base.agent.Stat.SightRange;
			}
			this.characterList.Clear();
			if (this.scanUser && this.scanBot)
			{
				using (List<WorldPlayerCharacter>.Enumerator enumerator = base.agent.FindEnemyPlayersForAttack(base.agent.GetPosition(), num).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WorldPlayerCharacter item = enumerator.Current;
						this.characterList.Add(item);
					}
					goto IL_137;
				}
			}
			if (this.scanUser)
			{
				using (List<WorldPlayerCharacter>.Enumerator enumerator = base.agent.FindEnemyPlayersForAttack(base.agent.GetPosition(), num, PlayerType.UserPlayer).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WorldPlayerCharacter item2 = enumerator.Current;
						this.characterList.Add(item2);
					}
					goto IL_137;
				}
			}
			if (this.scanBot)
			{
				foreach (WorldPlayerCharacter item3 in base.agent.FindEnemyPlayersForAttack(base.agent.GetPosition(), num, PlayerType.BotPlayer))
				{
					this.characterList.Add(item3);
				}
			}
			IL_137:
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.characterList)
			{
				if (OperationTools.Compare(base.agent.Status.Level, worldPlayerCharacter.Status.Level, this.levelCheckType))
				{
					base.agent.Controller.TargetOn(worldPlayerCharacter);
					base.EndAction(true);
					return;
				}
			}
			base.EndAction(false);
		}

		
		public bool scanUser;

		
		public bool scanBot;

		
		public bool useAutoRange;

		
		public int range;

		
		public CompareMethod levelCheckType;

		
		private List<WorldPlayerCharacter> characterList = new List<WorldPlayerCharacter>();
	}
}
