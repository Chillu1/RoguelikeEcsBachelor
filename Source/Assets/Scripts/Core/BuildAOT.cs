using System;
using System.Linq;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	internal static class BuildAOT
	{
		private static void AOTCodeGeneration()
		{
			// All components registered for AOT compilation
			// This is required for iOS and WebGL builds

			//TODO Wouldn't it be easier to get all the types that are used as attributes? Probably won't work because AoT won't run the code
			AoTHelper.RegisterComponent<VelocityComp>();
			AoTHelper.RegisterComponent<PositionComp>();
			AoTHelper.RegisterComponent<IsPlayerComp>();
			AoTHelper.RegisterComponent<DestroyComp>();
			AoTHelper.RegisterComponent<HealthRegenComp>();
			AoTHelper.RegisterComponent<HealthComp>();
			AoTHelper.RegisterComponent<IsCameraTargetComp>();
			AoTHelper.RegisterComponent<PlayerMouseClickComp>();
			AoTHelper.RegisterComponent<PiercingComp>();
			AoTHelper.RegisterComponent<IsProjectileComp>();
			AoTHelper.RegisterComponent<IsHomingProjectile>();
			AoTHelper.RegisterComponent<TimeOutComp>();
			AoTHelper.RegisterComponent<AttackComp>();
			AoTHelper.RegisterComponent<IsEnemy>();
			AoTHelper.RegisterComponent<HasCorpseExplosion>();
			AoTHelper.RegisterComponent<DamageEventsComp>();
			AoTHelper.RegisterComponent<BurningComp>();
			AoTHelper.RegisterComponent<ColdComp>();
			AoTHelper.RegisterComponent<DeathMarkComp>();
			AoTHelper.RegisterComponent<PoisonedComp>();
			AoTHelper.RegisterComponent<BleedingComp>();
			AoTHelper.RegisterComponent<IsRupturedComp>();
			AoTHelper.RegisterComponent<IsChargeEnemy>();
			AoTHelper.RegisterComponent<MultiplierComp>();
			AoTHelper.RegisterComponent<CircleEffectComp>();
			AoTHelper.RegisterComponent<CircleEffectDataComp>();
			AoTHelper.RegisterComponent<IsGhostEnemy>();
			AoTHelper.RegisterComponent<IsMeleeEnemy>();
			AoTHelper.RegisterComponent<IsAnimationComp>();
			AoTHelper.RegisterComponent<DestinationPositionComp>();
			AoTHelper.RegisterComponent<SplitComp>();
			AoTHelper.RegisterComponent<PlayerCurseComp>();
			AoTHelper.RegisterComponent<FlashEffectComp>();
			AoTHelper.RegisterComponent<DoExplodeOnDeathComp>();
			AoTHelper.RegisterComponent<IsSleeping>();
			AoTHelper.RegisterComponent<PhaseComp>();
			AoTHelper.RegisterComponent<InvulnerableComp>();
			AoTHelper.RegisterComponent<UnHittableComp>();
			AoTHelper.RegisterComponent<IsMapEdgeEnemy>();
			AoTHelper.RegisterComponent<IsRangedEnemy>();
			AoTHelper.RegisterComponent<RendererComp>();
			AoTHelper.RegisterComponent<IsRangedEnemyAttack>();
			AoTHelper.RegisterComponent<SpawnComp>();
			AoTHelper.RegisterComponent<IsDistanceFollowEnemy>();
			AoTHelper.RegisterComponent<IsPinballEnemy>();
			AoTHelper.RegisterComponent<GrowComp>();
			AoTHelper.RegisterComponent<IsBufferEnemy>();
			AoTHelper.RegisterComponent<EnemyBuffComp>();
			AoTHelper.RegisterComponent<ShadowRendererComp>();
			AoTHelper.RegisterComponent<HealthBarComp>();

			throw new InvalidOperationException("This method should never be called");
		}
	}
}