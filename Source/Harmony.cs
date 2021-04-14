using HarmonyLib;
using Verse;

namespace ZzZomboRW
{
	[StaticConstructorOnStartup]
	internal static class HarmonyHelper
	{
		static HarmonyHelper()
		{
			var harmony = new Harmony($"ZzZomboRW.{MOD.NAME}");
			harmony.PatchAll();
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
			private static void Postfix(bool __result, Verb_Shoot __instance)
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
}
