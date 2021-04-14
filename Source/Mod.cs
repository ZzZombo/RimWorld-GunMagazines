using UnityEngine;
using HarmonyLib;
using Verse;
using RimWorld;

internal static class MOD
{
	public const string NAME = "Gun Magazines";
}

namespace ZzZomboRW
{
	public class StatPart_RangedWeapon_Cooldown: StatPart
	{
		public override void TransformValue(StatRequest req, ref float val)
		{
			if(req.HasThing)
			{
				var comp = req.Thing.TryGetComp<CompGunWithMagazines>();
				if((comp?.Enabled ?? false) && comp.CurrentAmmo > 0)
				{
					val = 0;
				}
			}
		}
		public override string ExplanationPart(StatRequest req)
		{
			if(req.HasThing)
			{
				var comp = req.Thing.TryGetComp<CompGunWithMagazines>();
				if(comp?.Enabled is true && comp.CurrentAmmo > 0)
				{
					return "ZzZomboRW_StatPart_RangedWeapon_Cooldown".Translate();
				}
			}
			return null;
		}
	}
	public class StatWorker_CurrentAmmo: StatWorker
	{
		public override bool IsDisabledFor(Thing thing)
		{
			return !(thing.TryGetComp<CompGunWithMagazines>()?.Enabled is true) || base.IsDisabledFor(thing);
		}
		public override bool ShouldShowFor(StatRequest req)
		{
			if(req.HasThing)
			{
				var comp = req.Thing.TryGetComp<CompGunWithMagazines>();
				return comp?.Enabled is true;
			}
			return base.ShouldShowFor(req);
		}
		public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
		{
			if(req.HasThing)
			{
				var comp = req.Thing.TryGetComp<CompGunWithMagazines>();
				if(comp?.Enabled is true)
				{
					return comp.CurrentAmmo;
				}
			}
			return 0;
		}
	}

	public class CompProperties_GunWithMagazines: CompProperties
	{
		public bool enabled = true;
		public int currentAmmo = -1;
		public CompProperties_GunWithMagazines()
		{
			this.compClass = typeof(CompGunWithMagazines);
			Log.Message($"[ZzZomboRW.CompProperties_GunWithMagazines] Initialized:\n" +
				$"\tCurrent ammo: {this.currentAmmo};\n" +
				$"\tEnabled: {this.enabled}.");
		}

	}
	public class CompGunWithMagazines: ThingComp
	{
		public CompProperties_GunWithMagazines Props => (CompProperties_GunWithMagazines)this.props;
		public bool Enabled => this.Props.enabled && this.MaxAmmo > 1;
		public int MaxAmmo
		{
			get => (int)this.parent.GetStatValue(DefDatabase<StatDef>.GetNamed("ZzZomboRW_GunWithMagazines_MaxAmmo"), true);
		}
		public int CurrentAmmo
		{
			get => this.Props.currentAmmo;
			set => this.Props.currentAmmo = Mathf.Clamp(value, 0, this.MaxAmmo);
		}
		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			if(this.CurrentAmmo < 0)
			{
				this.CurrentAmmo = this.MaxAmmo;
			}
			Log.Message($"[ZzZomboRW.CompGunWithMagazines] Initialized for {this.parent}:\n" +
				$"\tCurrent ammo: {this.CurrentAmmo};\n" +
				$"\tMax ammo: {this.MaxAmmo};\n" +
				$"\tEnabled: {this.Props.enabled}.");
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref this.Props.currentAmmo, "currentAmmo", 1, false);
			Scribe_Values.Look(ref this.Props.enabled, "enabled", true, false);
		}
	}

	[StaticConstructorOnStartup]
	internal static class HarmonyHelper
	{
		static HarmonyHelper()
		{
			var harmony = new Harmony($"ZzZomboRW.{MOD.NAME}");
			harmony.PatchAll();
		}
	}

	[HarmonyPatch(typeof(Verb_LaunchProjectile), nameof(Verb_LaunchProjectile.Available), Priority.Last)]
	public static class Verb_LaunchProjectile_AvailablePatch
	{
		private static void Postfix(ref bool __result, Verb_LaunchProjectile __instance)
		{
			if(__instance is Verb_Shoot verb)
			{
				var comp = verb.EquipmentSource?.GetComp<CompGunWithMagazines>();
				if(__result && comp?.Enabled is true)
				{
					__result = comp.CurrentAmmo > 0;
				}
			}
		}
	}

	[HarmonyPatch(typeof(Verb_Shoot), nameof(Verb_Shoot.TryCastShot))]
	public static class Verb_Shoot_TryCastShotPatch
	{
		private static void Postfix(ref bool __result, Verb_Shoot __instance)
		{
			var comp = __instance.EquipmentSource?.GetComp<CompGunWithMagazines>();
			if(__result && comp?.Enabled is true && comp.CurrentAmmo > 0)
			{
				--comp.CurrentAmmo;
			}
		}
	}

	[HarmonyPatch(typeof(Verb_Shoot), nameof(Verb_Shoot.WarmupComplete))]
	public static class Verb_Shoot_WarmupCompletePatch
	{
		private static void Postfix(Verb_Shoot __instance)
		{
			var comp = __instance.EquipmentSource?.GetComp<CompGunWithMagazines>();
			if(comp?.Enabled is true && comp.CurrentAmmo < 1)
			{
				comp.CurrentAmmo = comp.MaxAmmo;
			}
		}
	}
}
