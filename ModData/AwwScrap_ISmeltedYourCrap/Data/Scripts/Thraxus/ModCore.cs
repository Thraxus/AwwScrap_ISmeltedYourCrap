using AwwScrap_ISmeltedYourCrap.Thraxus.Common.BaseClasses;
using AwwScrap_ISmeltedYourCrap.Thraxus.Common.Enums;
using VRage.Game.Components;

namespace AwwScrap_ISmeltedYourCrap.Thraxus
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, priority: int.MinValue + 100)]
	public class ModCore : BaseSessionComp
	{
		protected override string CompName { get; } = nameof(ModCore);
		protected override CompType Type { get; } = CompType.Both;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.NoUpdate;
	}
}
