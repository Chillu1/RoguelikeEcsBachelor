using NUnit.Framework;
using RoguelikeEcs.Core;

namespace RoguelikeEcs.Tests
{
	public class UpgradeStateTests : BaseTest
	{
		[Test]
		public void Enable_UpgradeOn()
		{
			ref var upgrades = ref World.Get<UpgradesComp>();

			Assert.False(upgrades.AlwaysCrit.IsOn());

			upgrades.AlwaysCrit.Enable();

			Assert.True(upgrades.AlwaysCrit.IsOn());
		}

		[Test]
		public void EnableTwice_Buffer_DisableOnce_UpgradeOn()
		{
			ref var upgrades = ref World.Get<UpgradesComp>();

			upgrades.AlwaysCrit.Enable();
			upgrades.AlwaysCrit.Enable();

			Assert.True(upgrades.AlwaysCrit.IsOn());

			upgrades.AlwaysCrit.Disable();

			Assert.True(upgrades.AlwaysCrit.IsOn());
		}

		[Test]
		public void EnableThrice_DisableThrice_UpgradeOff()
		{
			ref var upgrades = ref World.Get<UpgradesComp>();

			upgrades.AlwaysCrit.Enable();
			upgrades.AlwaysCrit.Enable();
			upgrades.AlwaysCrit.Enable();

			Assert.True(upgrades.AlwaysCrit.IsOn());

			upgrades.AlwaysCrit.Disable();
			upgrades.AlwaysCrit.Disable();
			Assert.True(upgrades.AlwaysCrit.IsOn());
			upgrades.AlwaysCrit.Disable();

			Assert.False(upgrades.AlwaysCrit.IsOn());
		}
	}
}