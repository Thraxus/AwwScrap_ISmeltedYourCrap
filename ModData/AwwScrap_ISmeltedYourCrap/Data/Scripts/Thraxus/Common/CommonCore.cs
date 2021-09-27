using AwwScrap_ISmeltedYourCrap.Thraxus.Common.BaseClasses;
using AwwScrap_ISmeltedYourCrap.Thraxus.Common.Enums;
using AwwScrap_ISmeltedYourCrap.Thraxus.Common.Factions.Models;
using AwwScrap_ISmeltedYourCrap.Thraxus.Common.Reporting;
using VRage.Game.Components;

namespace AwwScrap_ISmeltedYourCrap.Thraxus.Common
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, priority: int.MinValue + 1)]
	public class CommonCore : BaseSessionComp
	{
		protected override string CompName { get; } = "CommonCore";
		protected override CompType Type { get; } = CompType.Server;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.NoUpdate;

		protected override void SuperEarlySetup()
		{
			base.SuperEarlySetup();
		}

		protected override void LateSetup()
		{
			base.LateSetup();
			FactionDictionaries.Initialize();
			WriteGeneral($"{CompName} - Game Settings", $"{GameSettings.Report()}");
			WriteGeneral($"{CompName} - Factions", $"{FactionDictionaries.Report()}");
		}
	}
}
