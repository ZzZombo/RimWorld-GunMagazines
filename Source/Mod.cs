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
				var v = val;
				var comp = req.Thing.TryGetComp<CompGunWithMagazines>();
				if((comp?.Enabled ?? false) && comp.CurrentAmmo > 0)
				{
					val = 0;
				}
				// Log.Message($"{comp?.parent}: {comp?.CurrentAmmo} ammo, {v}s -> {val}s CD.");
			}
		}
		public override string ExplanationPart(StatRequest req)
		{
			if(req.HasThing)
			{
				var comp = req.Thing.TryGetComp<CompGunWithMagazines>();
				if((comp?.Enabled ?? false) && comp.CurrentAmmo > 0)
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
			return !(thing.TryGetComp<CompGunWithMagazines>()?.Enabled ?? false) || base.IsDisabledFor(thing);
		}
		public override bool ShouldShowFor(StatRequest req)
		{
			if(req.HasThing)
			{
				var comp = req.Thing.TryGetComp<CompGunWithMagazines>();
				return comp?.Enabled ?? false;
			}
			return base.ShouldShowFor(req);
		}
		public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
		{
			if(req.HasThing)
			{
				var comp = req.Thing.TryGetComp<CompGunWithMagazines>();
				if(comp?.Enabled ?? false)
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
		//private int maxAmmo = 1;
		public int currentAmmo = -1;
		//public float reFireDelay = 0.1f;
		public CompProperties_GunWithMagazines()
		{
			this.compClass = typeof(CompGunWithMagazines);
			Log.Message($"[ZzZomboRW.CompProperties_GunWithMagazines] Initialized:\n" +
				$"\tCurrent ammo: {this.currentAmmo};\n" +
				//$"\tRefire delay: {this.reFireDelay};\n" +
				$"\tEnabled: {this.enabled}.");
		}

	}
	public class CompGunWithMagazines: ThingComp
	{
		public CompProperties_GunWithMagazines Props => (CompProperties_GunWithMagazines)this.props;
		//private StatModifier statMod;
		//private bool setStatMod = false;
		//private StatModifier StatMod
		//{
		//    get
		//    {
		//        if (!this.setStatMod)
		//        {
		//            var request = StatRequest.For(this.parent);
		//            this.statMod = request.StatBases.Find((a) => a.stat == StatDefOf.RangedWeapon_Cooldown);
		//            this.setStatMod = true;
		//        }
		//        return this.statMod;
		//    }
		//}
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
				this.CurrentAmmo = this.MaxAmmo;
			Log.Message($"[ZzZomboRW.CompGunWithMagazines] Initialized for {this.parent}:\n" +
				$"\tCurrent ammo: {this.CurrentAmmo};\n" +
				$"\tMax ammo: {this.MaxAmmo};\n" +
				//$"\tRefire delay: {this.Props.reFireDelay};\n" +
				$"\tEnabled: {this.Props.enabled}.");
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref this.Props.currentAmmo, "currentAmmo", 1, false);
			//Scribe_Values.Look(ref this.Props.reFireDelay, "reFireDelay", 0.1f, false);
			Scribe_Values.Look(ref this.Props.enabled, "enabled", true, false);
		}
	}

	[StaticConstructorOnStartup]
	internal static class HarmonyHelper
	{
		static HarmonyHelper()
		{
			Harmony harmony = new Harmony($"ZzZomboRW.{MOD.NAME}");
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
				if(__result && (comp?.Enabled ?? false))
				{
					__result = comp.CurrentAmmo > 0;
				}
			}
		}
	}

	[HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
	public static class Verb_Shoot_TryCastShotPatch
	{
		private static void Postfix(ref bool __result, Verb_Shoot __instance)
		{
			var comp = __instance.EquipmentSource?.GetComp<CompGunWithMagazines>();
			if(__result && (comp?.Enabled ?? false) && comp.CurrentAmmo > 0)
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
			if((comp?.Enabled ?? false) && comp.CurrentAmmo < 1)
			{
				//Log.Message($"{comp.parent} is reloaded ({comp.parent.GetStatValue(StatDefOf.RangedWeapon_Cooldown)}s).");
				comp.CurrentAmmo = comp.MaxAmmo;
			}
		}
	}
}
