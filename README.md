# IRTweaks
This is a mod for the [HBS BattleTech](http://battletechgame.com/) game that includes a variety of tweaks, changes, and modifications to the base game. A wide range of effects is covered, and all of the options can be selectively enabled. A short summary of the features is provided below. You can enable or disable each tweak in `mod.json` by setting the appropriate `Fixes` value to true or false.

* **AlternativeMechNamingStyle**: Used by RogueTech to set unique naming styles consistent with the **LowVisiblity** mod. No other systems should use this.
* **BuildingDamageColorChange**: Changes the floating damage on buildings to be dark blue to distinguish this damage type. Thanks to **Gnivler** for this fix!
* **BraceOnMeleeWithJuggernaut**: If a pilot has the Guts 8 ability (typically Juggernaut), braces the unit after a melee or DFA attack. A direct copy of [RealityMachina's Better-Juggernaut](https://github.com/RealityMachina/Better-Juggernaut).
* **BulkPurchasing**: Provides buttons and keyboard shortcuts that allow bulk purchasing and selling of items in the store.
* **BulkScrapping**: Allows players to scrap all chassis of a given weight from the storage screen at once. Hold Alt and click one of the weight filters at the top of the storage screen to scrap everything in that category.
* **CalledShotTweaks**: The modifier for called shots (aka offensive push) is driven by the pilot's tactics skill, ability, and pilot tags. It can also be influenced by gear. Options allow disabling the ability to called shot 
* **CombatLog**: Provides an in-game log that captures text from floaties and preserves them in a readable format.
* **DisableCampaign**: Disables the HBS campaign button, to prevent errors with various mod packs.
* **DisableCombatRestarts**: Mission restarts lead to corruption at the salvage screen in vanilla, and especially in a modded game. This disables the UI selection that allows in-combat saves to be made.
* **DisableCombatSaves**: Combat saves are prone to errors during vanilla gameplay, but especially so during modded gameplay. This disables the UI selection that allows in-combat saves to be made.
* **DisableLowFundsNotification**: Disables the irritating "low funds" notification.
* **DisableMPHashCalculation**: Disables a mod-hash calculated on startup that's only used to validate multiplayer games are compatible. Saves 2-3s of load time.
* **ExtendedStats**: Allows pilots to be assigned Statistic values above the normal bounds of 1-10.
* **FlexibleSensorLock**: Using a Sensor Lock action does not count as movement or firing. This allows it to be combined with actions in a unit's activation.
* **MaxArmorMaxesArmor**: "Max Armor" button in mechbay now sets armor to the maximum possible for the chassis; ignores available tonnage. Holding Control while clicking will use vanilla functionality (does not work well with MechEngineer armors like ferro, etc.)
* **MechbayLayoutFix**: Moves a few UI elements in the mechbay to work better in a MechEngineer based mod. Thanks to Tiraxx for the idea!
* **MechbayAdvancedStripping**: Holding Left or Right Control while clicking the "Strip Equipment" button in the mechbay will strip <i>only</i> weapons and ammo.
* **MultiTargetStat**: This allows units to gain the Multi-Target ability from a Statistic, which can be applied via effects.
* **PainTolerance**: Provides Guts-based resistance to injuries. See below for details.
* **PathfinderTeamFix**: Mission Control introduces pathfinding units that have no Team associated with them. This breaks some mods, which this fix remediates.
* **Random Start by Difficulty Menu**: Allows an option in the new-game difficulty menu to be associated with user-created lists of starting mechs.
* **ReduceSaveCompression**: By default the game is setup to use an aggressive compression on save game files. This slows down game load. By setting this to true saves will use more space on disk but load faster. Thanks to **Gnivler** for this fix!
* **ScaleObjectiveBuildingStructure**: Increases the structure of any building that is the target of an objective. This allows high difficulty attack bases to be more difficult, and high difficulty defend bases to be easier.
* **ShowAllArgoUpgrades**: Shows all available argo upgrades, instead of hiding the ones that require additional unlocks.
* **SimGameDifficultyLabelsReplacer**: Allows customization of the labels on the 'difficulty' bar when you start a new career or campaign game. 
* **SkirmishAlwaysUnlimited**: This allows you to drop from Skirmish even if your lances violate the limits of the currently selected operation type.
* **SkirmishReset**: This fix is a modder's resource. Skirmish saves the mechDefs that were customized, which can result in an ever-spinny when itemDefs are changed or mods are disabled. When enabled, this fix will always reset the Skirmish lances and mech definitions to the base state by deleting all customizations.
* **SkipDeleteSavePopup**: Disables the 'are you sure' prompt when you delete save games.
* **Spawn Protection**: Provides high evasion, braced, and guarded status to units when they spawn. This can prevent first-turn damage during mission start, or to reinforcements that spawn close to the player.
* **Streamlined Main Menu**: This tweaks the layout of the main Argo UI to move the most commonly accessed buttons directly to the sidebar. 
* **Urban Explosions Fix**: This corrects a subtle bug in HBS code that causes exploding buildings to not sequence properly. Unfortunately enabling this fix makes buildings take significantly longer to be destroyed. This will be improved in a future fix.
* **Weapon Tooltips**: Modifies the weapon tooltips to more accurately report damage when a weapon uses extensions provided by [CustomAmmoCategories](https://github.com/CMiSSioN/CustomAmmoCategories).

This mod replaces the following mods, which used to be stand-alone:

* *BTRandomStartByDifficultyMenu*
* *CombatSaveDisabler*
* *IRUITweaks*
* *SpawnProtection*
* *Better-Juggernaut / Mighty-Juggernaut*

This mod requires the latest releases of the following mods:

* [IRBTModUtils](https://github.com/iceraptor/IRBTModUtils/) - a collection of common utilities used in all my mods.
* [Custom Ammo Categories](https://github.com/BattletechModders/CustomBundle/releases) - for enhanced weapon tooltips

## Bulk Purchasing


This tweak makes minor changes to the store UI elements:

* Enables bulk-purchase for items in the mech store
* Allows Shift+Click on the -/+ in the bulk-purchase and bulk-sell screens to increment the count by -/+ 5. You can customize this value by setting `Store.QuantityOnShift` in mod.json.
* Allows Control+Click on the -/+ in the bulk-purchase and bulk-sell screens to increment the count by -/+ 20. You can customize this value by setting `Store.QuantityOnControl` in mod.json.

## Called Shot Tweaks

This tweak makes two major changes to how Called Shot (i.e. Morale Attack aka Fury aka Precision Strike) works. The first change is that the modifier applied to the shot is no longer a fixed value (`CombatGameConstants:ToHitOffensivePush`), but rather calculated by a formula (see below). The second change is that choosing a location can be disabled entirely, or headshots can be disabled. 

### Called Shot Modifier Calculation

`CalledShotModifier = BaseModifier + PilotTacticsModifier + PilotTagsModifier + ActorStatModifier`

The *BaseModifier* is set as `Combat.CalledShots.BaseModifier` in mod.json. The *ActorStatModifier* is defined by the `IRTCalledShotMod` statistic, which can be set by an effect on the actor. The *PilotTacticsModifier* and *PilotTagsModifier* are defined below.

#### Pilot Skills Modifier

A pilots skills and abilities determine the *PilotTagsModifier*. Their **Tactics** skill defines the value of the modifier, as shown in the table below. Note that values are negative, as negatives are bonuses.

| Skill            | 1    | 2    | 3    | 4    | 5    | 6    | 7    | 8    | 9    | 10   | 11   | 12   | 13   |
| ---------------- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| Modifier         | +0   | -1   | -1   | -2   | -2   | -3   | -3   | -4   | -4   | -5   | -6   | -7   | -8   |
| +Level 5 Ability | NA   | NA   | NA   | NA   | -3   | -4   | -4   | -5   | -5   | -6   | -7   | -8   | -9   |
| +Level 8 Ability | NA   | NA   | NA   | NA   | NA   | NA   | NA   | -6   | -6   | -7   | -8   | -9   | -10  |

You can disable the calculation of this modifier by setting `Combat.CalledShot.EnableTacticsModifier=false`.

#### Pilot Tags Modifier

Every pilot can tag one or more tags, which indicate special behaviors. Every tag defined in `Combat.CalledShot.PilotTags`(in _mod.json_) applies a specific modifier if the pilot also possesses that tag. The sum of modifiers from all applicable tags becomes the *PilotTagsModifier*.

> Example: Combat.CalledShot.PilotTag is { "pilot_drunk" : 2, "pilot_reckless" : 1, pilot_assassin": -2,  }. If the pilot has both the pilot_drunk and pilot_reckless tags, their PilotTagsModifier would be +3.

#### Unit Modifier

Units that have `IRTCalledShotMod` in their _StatCollection_ will take this modifier as the `CalledShotUnitMod `. The _StatCollection_ assumes an Int32 (integer) value.

### Called Shot Location

By setting `Combat.CalledShot.DisableAllLocations=true`, you can disable the selection of any location during a called shot. This will apply any relevant penalties and consume morale, but will randomize locations like a normal attack. The popup won't be displayed but the background will light up. 

If you instead set `Combat.CalledShot.DisableHeadshots=true`, the popup will be displayed normally but the head will not be a valid selection. You won't be able to click on it, though it is still displayed in the paper doll as normal.

This restrictions do not apply to any units that are currently prone or shutdown. These units can be targeted with called shot as per vanilla.

Additionally you can allow an actor to bypass this restriction by setting  `IRTCalledShotAlwaysAllow` statistic (Boolean) to true. Units with this set to true can always use the vanilla called shot location selection.

## Flexible Sensor Lock
This tweak allows units to use sensor lock without it consuming their action or movement. This can be limited to units with a specific ability or stat. The ability to restrict this is defined by the `AbilityOpts.FlexibleSensorLockId` value in mod.json, which defaults to `AbilityDefT8A`. The stat is defined in `AbilityOpts.FreeActionStatName`, which defaults `IR_FreeSensorLock`. 

If the `Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe` is true, the ActiveProbe ability that some units possess will also not require an action or movement.

## Pain Tolerance
This tweak makes skilled pilots more resistant to injuries. Any time the pilot would normally be injured by a head hit, torso destruction, ammo explosion, or knockdown, they make a check against their Guts skill rating to determine if they can shrug off the effect. 

This check is also made in the event of a Shutdown, if the optional _ShutdownCausesInjury_ value is set to true. This value can be set in either the `CombatGameConstants.json`, or can be introduced by [MechEngineer](https://github.com/BattletechModders/MechEngineer/blob/master/source/Features/ShutdownInjuryProtection/Patches/MechShutdownSequence_CheckForHeatDamage_Patch.cs). This check is also made if you are using MechEngineer's  [ReceiveHeatDamageInjury](https://github.com/BattletechModders/MechEngineer/blob/master/source/Features/ShutdownInjuryProtection/Patches/Mech_CheckForHeatDamage_Patch.cs) option. 

### Details

Each pilot's resist check is defined by their rating in the Guts skill, as well as any Abilities in that tree that have been taken. The table below defines the guts modifier that will be used as a modifier base. This value is then multiplied by the **ResistPerGuts** configuration value to determine a base check level. Pilot skills of 11-13 are used for elite pilots in the RogueTech mod. Player pilots cannot reach this level.

Assuming a *ResistPerGuts* value of 10, this table defines the resist chance per guts rating:

| Skill                | 1    | 2    | 3    | 4    | 5    | 6    | 7    | 8    | 9    | 10   | 11   | 12   | 13   |
| -------------------- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| Modifier             | 10% | 20% | 30% | 40% | 50% | 60% | 70% | 80% | 90% | 100% | 110% | 120% | 130% |
| with Level 5 Ability | 10% | 20% | 30% | 40% | 60% | 70% | 80% | 90% | 100% | 110% | 120% | 130% | 140% |
| with Level 8 Ability | 10% | 20% | 30% | 40% | 70% | 80% | 90% | 100% | 110% | 120% | 130% | 140% | 150% |

> Example: A pilot has guts 5 and the level 5 ability. This gives them a 60% modifier to ignore damage sources. If the pilot improves their Guts skill to 6, their modifier would increase to 70%.

### Ammo Explosion Injuries

When an ammo box explodes, the ratio of available to total rounds in the exploding ammo box is calculated. For each percentage point, the resist penalty is reduced by `PainTolerance.PenaltyPerAmmoExplosionRatio`. This defaults to 1, so the resist penalty will be reduced by 1% for each 1% of ammo remaining in the box.

### Head Injuries

When a mech's head is hit, each point of damage reduces the resist chance by the **HeadDamageResistPenaltyPerArmorPoint** configuration value. If the attack only inflicts armor damage, the total damage is multiplied by the **HeadHitArmorOnlyResistPenaltyMulti** value. By default this reduces the damage amount by 50%, making it easier for players to shrug off the hit.

> Example: A pilot with guts 7 (but no abilities) gets hit in the head of 3 damage. The damage doesn't penetrate the armor, so it becomes 3 * 0.5 = 1.5, rounded down to 1. The pilot's resist chance is 70% - 5% for the damage, or 65%.
>
> Example 2: A pilot with guts 10 and both abilities gets hit in the head for 18 damage. Some of the damage penetrates structure. The pilot's base resist chance is 120%. The attack reduces that check by 18 * 5% = 90%. The pilot has 30% chance to avoid the injury.

### Knockdown Injuries

When a Mech suffers a knockdown, the resist chance is reduced by 6%. You can customize this value by setting `PainTolerance.KnockdownResistPenalty`.

> Example: A pilot with guts 3 suffers a knockdown. They have a base resist of 30%, -6% for a knockdown, and thus will ignore the injury on 24% or less.

## Obstructed Line-Of-Site Damage Resistance

When LOS to a target is "obstructed" (red-yellow or purple colored targeting line with an "eye" icon), shots hitting certain mech or vehicle armor locations can have their damage reduced.

### Configuration

`DRMechLocs` - List of valid mech `ArmorLocation`s that have can incoming damage reduced.

`DRVehicleLocs` - List of valid `VehicleChassisLocations` that can have incoming damage reduced.

`QuadTags` - List of mechdef tags that denote a unit is a Quad mech. Adds "LeftArm" and "RightArm" to valid ArmorLocations for these units only.

`ObstructionDRByTags` - Dictionary where key = unit mechdef or vehicledef tag, and value = incoming damage multiplier for units with that tag.

> Example:
```
			"ObstructionTweaks": {
				"DRMechLocs": [
					"LeftLeg",
					"RightLeg"
				],
				"DRVehicleLocs": [
					"Front",
					"Rear",
					"Left",
					"Right"
				],
				"QuadTags": [
					"unit_quad"
				],
				"ObstructionDRByTags": {
					"unit_mech": 0.5,
					"unit_vehicle": 0.75,
					"unit_quad": 0.1
				}
			}
```
Using the above settings, a standard mech with tag `unit_mech` would take 50% incoming damage to its left and right legs. A vehicle with tag `unit_vehicle` would take 75% incoming damage to front, rear, left, and right sides, and a Quad mech with tag `unit_quad` would take 10% damage to all 4 legs (left leg, right leg, left "arm", and right "arm"). A unit with multiple matching "tags" in `ObstructionDRByTags` will use the <i>lowest</i> damage multiplier, taking the least damage.

### Overheat Injuries

When a mech takes overheat damage, each point of heat over the overheating limit reduces the resist chance by **OverheatResistPenaltyPerHeatPercentile**. The difference between the mech's maximum heat (at which point is shuts down) and it's overheating limit is taken as a spectrum from 0 - 100. The mech's current heat within this spectrum defines the resistance penalty.

> Example: A pilot with a resistance check of 60% overheats their Mech. The Mech has a maximum heat of 200, and an overheating limit of 120. 200 - 120 = 80 points on the overheating spectrum. If the Mech was currently as 180 heat points, the overheating points would be calculated as 180 -120 = 60 points. The ratio of points to spectrum is then calculated, or 80 / 60 = 0.75 * 100 = 75 points. Assuming PenaltyPerHeatDamageInjuryRatio = 1, then the resist check is reduced by -75%. The pilot cannot resist and thus takes the injury.

### Side Torso Destroyed Injuries

When a side torso is destroyed, a pilot normally takes a point of damage. The resist chance is reduced by `PainTolerance.SideLocationDestroyedResistPenalty`, which defaults to 10%.

## Random Start By Difficulty

This tweak allows you to define difficulty settings that impact the Career starts. This is useful for mods like RogueTech, which customizes your starting lance and faction reputation based upon a difficulty menu selection. The mod looks for two custom *DifficultyConstants*, each with a different behavior.

This tweak is enabled if `Fixes.RandomStartByDifficulty=true` is set to true in _mod.json_. Customizations are only expressed through the difficulty constants described below.

### StartingRandomMechLists

A comma separated list of lancedefs that should be used to randomize the starting mech selections. It's only useful when 'random mechs' are selected as an option. An example value would be:

```json
{
                    "ID": "diff_myDiff",
                    "Name": "UI Label To display",
                    "DifficultyValue": 0,
                    "DifficultyConstants": [
                        {
                            "ConstantType": "CareerMode",
                            "ConstantName": "StartingSystems",
                            "ConstantValue": "UrCruinne"
                        },
                        {
                            "ConstantType": "CareerMode",
                            "ConstantName": "StartingRandomMechLists",
                            "ConstantValue":
                            "itemCollection_Mechs_Starting_0,itemCollection_Mechs_Starting_1,itemCollection_Mechs_Starting_2"
                        }
                    ]
                },
```



### FactionReputation

A comma separated list of factions and reputation bonuses to apply. Each faction is and bonus is joined by a colon, like `Steiner:20`. Should accept both positive and negative modifiers.

Example:

```json
{
                    "ID": "diff_myDiff",
                    "Name": "UI Label To display",
                    "DifficultyValue": 0,
                    "DifficultyConstants": [
                        {
                            "ConstantType": "CareerMode",
                            "ConstantName": "StartingSystems",
                            "ConstantValue": "UrCruinne"
                        },
                        {
                            "ConstantType": "CareerMode",
                            "ConstantName": "FactionReputation",
                            "ConstantValue":
                            "Davion:20,Liao:-20,Kurita:-20,Steiner:10"
                        }
                    ]
                },
```

## ScaleObjectiveBuildingStructure

This tweak modifies the structure of all buildings that are associated with an objective at the start of combat. All buildings are changed regardless of their faction affiliation, so it applies to defend base targets as much as attack base targets.

This tweak can be customized in mod.json through the `Combat.ScaledStructure` settings. `ScaledSettings.DifficultyScaling` is a dictionary keyed by the current contract's **finalDifficulty**. Each value has a `Multi` and `Mod` value. The `Multi` value is a multiplier that multiplies the base structure of the building. The 'Mod' value then modifies this value.

`AdjustedStructure = (BaseStructure x Multi) + Mod`


 `ScaledStructure.DefaultScale` defines the modifications that should be used when no difficulty-based scaling can be found. If the __finalDifficulty_ of a contract cannot be found, but the __finalDifficulty_ is within the minimum and maximum values defined in `DifficultyScaling` the default scale will be used.

## Sensor Lock Freedom

This tweak changes the SensorLock behaviors such that using the action doesn't end your turn. This previously occurred because SensorLock actions counted as both movement and firing (in a very weird way). When this tweak is enabled, you can sensor lock at any point during your activation.

This tweak has no customization.

## **SimGameDifficultyLabelsReplacer**

Copy `StreamingAssets\data\descriptions\CareerMode\TooltipScoreTotalMultiplier.json` into `Mods\IRTweaks\StreamingAssets\data\descriptions\CareerMode` to customize the tooltip text that pops up.

## Spawn Protection

This tweak provides significant defensive bonuses to units when they spawn. Units can be marked to receive a specific number of evasive pips and to be marked as braced. This makes for a better play experience on small maps where the player could be pummeled before they could even move. Mechs and Vehicles receive this protection, but Turrets do not. (Turrets cannot move and thus the modifiers never go away if they are applied to Turrets).

This has been extracted from CWolf's amazing [Mission Control](https://github.com/CWolfs/MissionControl) mod - check it out!

#### Configuration

This tweak is enabled if `Fixes.SpawnProtection=true` is set to true in _mod.json_. The following configuration options can further customize this tweak's behavior:

* `Combat.SpawnProtection.ApplyGuard`: If true, the protected unit will be Braced and thus gain the Guarded state. Defaults to true.
* `Combat.SpawnProtection.EvasionPips`: The number of evasion pips to add to the target unit. Defaults to 6.
* `Combat.SpawnProtection.ApplyToEnemies`: If true, enemies will be protected when they spawn as well. Defaults to true.
* `Combat.SpawnProtection.ApplyToNeutrals`: If true, neutrals will be protected when they spawn as well. Defaults to true.
* `Combat.SpawnProtection.ApplyToAllies`: If true, allies will be protected when they spawn as well. Defaults to true.
* `Combat.SpawnProtection.ApplyToReinforcements`: If true, enemies that spawn during a mission (i.e. are not present on the first turn) will be protected. Defaults to true.

## Turret Armor and Structure Multiplier

Decouples turret armor and structure multipliers from using the `ArmorMultiplierVehicle` and `StructureMultiplierVehicle` in CombatGameConstants. Disabled if the below tweaks are omitted from settings or set to 0, in which case the vehicle values from CombatGameConstants will be used. Otherwise works identically to the multiplier settings in CombatGameConstants.

### Configuration

```
"TurretArmorAndStructure": {
	"StructureMultiplierTurret": 2,
	"ArmorMultiplierTurret": 5
},
```

Above values would multiply turret structure and armor defined in defs by 2 and 5, respectively.

## Weapon Tooltips

This tweak makes minor changes to the tooltips shown when hovering over weapons:

* Overrides damage calculation on weapon display to show full damage potential (shots * projectiles)

This tweak is enabled if `Fixes.WeaponTooltip=true` is set to true in _mod.json_. There are no customizations for this tweak.
