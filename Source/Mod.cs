using RimWorld;
using Verse;

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
				if(comp?.Enabled is true && comp.CurrentAmmo > 0)
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
}
