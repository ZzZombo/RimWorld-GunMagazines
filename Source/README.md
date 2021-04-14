# Description
Adds a simple gun magazine system that doesn't require any real resource to use, in line with other things in vanila RimWorld. In combat NPCs fire as long as there is still ammo in the chamber, reload, rinse and repeat. Weapons use ammo per projectile released, and burst fire weapons still retain this capability unchanged, well, as long as their volley doesn't use more ammo then is available in the magazine, then it will be cut short after the last bullet is used up.

# Features
* Simplistic ammo system that doesn't require any real ammo gathering. NPCs magically pull bullets out their pockets before loading them into their guns.
* NPCs have minimal cooldown between volleys as long as there is still ammo loaded.
* NPCs take aim between each volley anew, but this time is less than it would be in the same situation w/o the mod, as the cooldown period is skipped while the gun still has some ammo left, so only the warm-up period is entered there.
* NPCs enter the regular cooldown period on running out of ammo to reload.
* Ammo capacity and remaining ammo can be currently seen by inspecting the weapon stats.
* Compatible with most of modded weapons as long as they use the vanilla shooting verb; no modded weapons are supported explicitly as of yet though, but others may easily add patches to them in order to make them run on this system.
* Most of the vanilla weapons are made to use this system out of the box. I didn't however carefully tweak each of them, so I'd like to gather feedback on what needs to be changed for the greater good.

# Known issues
* There are reported cases where weapons start with 1 ammo loaded instead of the maximum. I'm still investigating.
* Reportedly the chain shotgun can shoot more times than it has ammo loaded. Couldn't replicate, if you manage to please contact me!

# Planned
* Extended compatibility with "RunAndGun" and "Dual Wield".
* Controls to reload a given gun on demand.
* Ammo indicator.
* Patches to include certain modded weapons.
* User settings to tweak weapons magazine capacity.
* More options like ammo used per shot.
* Items like attacheable extended magazines and so on.

# For modders
In order to add support for this mod to your weapons you need to:
* Define this mod as a dependency in your mod's metadata.
* Add the component to your weapons: `ZzZomboRW.CompProperties_GunWithMagazines`.
* Define a stat for ammo capacity of the gun: `<ZzZomboRW_GunWithMagazines_MaxAmmo>10</ZzZomboRW_GunWithMagazines_MaxAmmo>`, replace the number with the maximum amount of ammo in the magazine. Goes into the `statBases` tag.
* ???
* PROFIT!
