using RimWorld;
using UnityEngine;
using Verse;

namespace ZzZomboRW
{
	public class CompProperties_GunWithMagazines: CompProperties
	{
		public bool enabled = true;
		public int currentAmmo = -1;
		public CompProperties_GunWithMagazines()
		{
			this.compClass = typeof(CompGunWithMagazines);
			Log.Message($"[{this.GetType().FullName}] Initialized:\n" +
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
			Log.Message($"[{this.GetType().FullName}] Initialized for {this.parent}:\n" +
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
}
