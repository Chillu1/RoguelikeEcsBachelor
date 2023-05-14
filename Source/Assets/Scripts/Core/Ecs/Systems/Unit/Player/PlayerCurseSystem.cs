using DefaultEcs;
using DefaultEcs.System;
using RoguelikeEcs.Core;
using System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(PlayerCurseComp))]
	public class PlayerCurseSystem : CustomEntitySetSystem
	{
		public PlayerCurseSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			ref var playerCurseComp = ref Player.Get<PlayerCurseComp>();
			var upgrades = World.Get<UpgradesComp>();
			ref var playerMultiplier = ref Player.Get<MultiplierComp>();

			bool isOn = false;
			bool hasDamageBoost = false;

			playerCurseComp.Timer -= state;

			if (playerCurseComp.Timer > 0)
			{
				if (upgrades.HasDeflectionCurse)
					Player.Set<DoDeflectProjectilesComp>();

				if (upgrades.HasDamageBoostCurse)
				{
					playerMultiplier.MultipliersList.Add(new MultiplierData()
					{
						StatType = StatType.Damage, Multiplier = 2
					});
					hasDamageBoost = true;
				}

				isOn = true;
			}

			if (playerCurseComp.Timer <= 0 && isOn == true)
			{
				Player.Disable<DoDeflectProjectilesComp>();

				if (hasDamageBoost)
				{
					playerMultiplier.MultipliersList.Add(new MultiplierData()
					{
						StatType = StatType.Damage, Multiplier = -2
					});
					hasDamageBoost = false;
				}

				isOn = false;
			}
		}
	}
}