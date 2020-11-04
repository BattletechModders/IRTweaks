# IRTweaks
This is a mod for the [HBS BattleTech](http://battletechgame.com/) game that includes a variety of tweaks, changes, and modifications to the base game. A wide range of effects is covered, and all of the options can be selectively enabled. A short summary of the features is provided below. You can enable or disable each tweak in `mod.json` by setting the appropriate `Fixes` value to true or false.

* **AlternativeMechNamingStyle**: Used by RogueTech to set unique naming styles consistent with the **LowVisiblity** mod. No other systems should use this.
* **BuildingDamageColorChange**: Changes the floating damage on buildings to be dark blue to distinguish this damage type. Thanks to **Gnivler** for this fix!
* **BraceOnMeleeWithJuggernaut**: If a pilot has the Guts 8 ability (typically Juggernaut), braces the unit after a melee or DFA attack. A direct copy of [RealityMachina's Better-Juggernaut](https://github.com/RealityMachina/Better-Juggernaut).
* **BulkPurchasing**: Provides buttons and keyboard shortcuts that allow bulk purchasing and selling of items in the store.
* **CalledShotTweaks**: The modifier for called shots (aka offensive push) is driven by the pilot's tactics skill, ability, and pilot tags. It can also be influenced by gear. Options allow disabling the ability to called shot 
* **CombatLog**: Provides an in-game log that captures text from floaties and preserves them in a readable format.
* **DisableCampaign**: Disables the HBS campaign button, to prevent errors with various mod packs.
* **DisableCombatRestarts**: Mission restarts lead to corruption at the salvage screen in vanilla, and especially in a modded game. This disables the UI selection that allows in-combat saves to be made.
* **DisableCombatSaves**: Combat saves are prone to errors during vanilla gameplay, but especially so during modded gameplay. This disables the UI selection that allows in-combat saves to be made.
* **DisableMPHashCalculation**: Disables a mod-hash calculated on startup that's only used to validate multiplayer games are compatible. Saves 2-3s of load time.
* **ExtendedStats**: Allows pilots to be assigned Statistic values above the normal bounds of 1-10.
* **FlexibleSensorLock**: Using a Sensor Lock action does not count as movement or firing. This allows it to be combined with actions in a unit's activation.
* **MechbayLayoutFix**: Moves a few UI elements in the mechbay to work better in a MechEngineer based mod. Thanks to Tiraxx for the idea!
* **MultiTargetStat**: This allows units to gain the Multi-Target ability from a Statistic, which can be applied via effects.
* **PainTolerance**: Provides Guts-based resistance to injuries. See below for details.
* **PathfinderTeamFix**: Mission Control introduces pathfinding units that have no Team associated with them. This breaks some mods, which this fix remediates.
* **Random Start by Difficulty Menu**: Allows an option in the new-game difficulty menu to be associated with user-created lists of starting mechs.
* **ReduceSaveCompression**: By default the game is setup to use an aggressive compression on save game files. This slows down game load. By setting this to true saves will use more space on disk but load faster. Thanks to **Gnivler** for this fix!
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

This tweak makes two major changes to how Called Shot (i.e. Morale Attack aka Fury aka Precision Strike) works. The first change is that the modifier applied to the shot is no longer a fixed value (`CombatGameConstants:ToHitOffensivePush`), but rather calculated by the following formula:

`CalledShotModifier = BaseModifier + PilotTacticsModifier + PilotTagsModifier + ActorStatModifier`

The *BaseModifier* is set as `Combat.CalledShots.BaseModifier` in mod.json. The *ActorStatModifier* is defined by the `IRTCalledShotMod` statistic, which can be set by an effect on the actor. The *PilotTacticsModifier* and *PilotTagsModifier* are defined below.

#### Pilot Skills Modifier

A pilots skills and abilities determine the *PilotTagsModifier*. Their **Tactics** skill defines the value of the modifier, as shown in the table below. Note that values are negative, as negatives are bonuses.

| Skill            | 1    | 2    | 3    | 4    | 5    | 6    | 7    | 8    | 9    | 10   | 11   | 12   | 13   |
| ---------------- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| Modifier         | +0   | -1   | -1   | -2   | -2   | -3   | -3   | -4   | -4   | -5   | -6   | -7   | -8   |
| +Level 5 Ability | NA   | NA   | NA   | NA   | -3   | -4   | -4   | -5   | -5   | -6   | -7   | -8   | -9   |
| +Level 8 Ability | NA   | NA   | NA   | NA   | NA   | NA   | NA   | -6   | -6   | -7   | -8   | -9   | -10  |

#### Pilot Tags Modifier

Every pilot can tag one or more tags, which indicate special behaviors. Every tag defined in `Combat.CalledShot.PilotTags`(in _mod.json_) applies a specific modifier if the pilot also possesses that tag. The sum of modifiers from all applicable tags becomes the *PilotTagsModifier*.

> Example: Combat.CalledShot.PilotTag is { "pilot_drunk" : 2, "pilot_reckless" : 1, pilot_assassin": -2,  }. If the pilot has both the pilot_drunk and pilot_reckless tags, their PilotTagsModifier would be +3.

#### Unit Modifier

Units that have `IRTCalledShotMod` in their _StatCollection_ will take this modifier as the `CalledShotUnitMod `. The _StatCollection_ assumes an Int32 (integer) value.

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

## Restrict Called Shots to Head

This tweak changes the called shot dialog to disallow selecting the head unless the target unit is shutdown or prone. This only applies to mech targets; vehicle and building targets may be targeted normally.

Units that have `IRTCalledShotAlwaysAllow : true` in their _StatCollection_ can select heads even if the target is shutdown or prone. This would typically come in through equipment and similar effects.

This tweak has no customization.

## Sensor Lock Freedom

This tweak changes the SensorLock behaviors such that using the action doesn't end your turn. This previously occurred because SensorLock actions counted as both movement and firing (in a very weird way). When this tweak is enabled, you can sensor lock at any point during your activation.

This tweak has no customization.

## **SimGameDifficultyLabelsReplacer**

Copy `StreamingAssets\data\descriptions\CareerMode\TooltipScoreTotalMultiplier.json` into `Mods\IRTweaks\StreamingAssets\data\descriptions\CareerMode` to customize the tooltip text that pops up.

## Spawn Protection

This tweak provides significant defensive bonuses to units when they spawn. Units can be marked to receive a specific number of evasive pips and to be marked as braced. This makes for a better play experience on small maps where the player could be pummeled before they could even move. Mechs and Vehicles receive this protection, but Turrets do not. (Turrets cannot move and thus the modifiers never go away if they are applied to Turrets).

This has been extracted from CWolf's amazing [Mission Control](https://github.com/CWolfs/MissionControl) mod - check it out!

### Configuration

This tweak is enabled if `Fixes.SpawnProtection=true` is set to true in _mod.json_. The following configuration options can further customize this tweak's behavior:

* `Combat.SpawnProtection.ApplyGuard`: If true, the protected unit will be Braced and thus gain the Guarded state. Defaults to true.
* `Combat.SpawnProtection.EvasionPips`: The number of evasion pips to add to the target unit. Defaults to 6.
* `Combat.SpawnProtection.ApplyToEnemies`: If true, enemies will be protected when they spawn as well. Defaults to true.
* `Combat.SpawnProtection.ApplyToNeutrals`: If true, neutrals will be protected when they spawn as well. Defaults to true.
* `Combat.SpawnProtection.ApplyToAllies`: If true, allies will be protected when they spawn as well. Defaults to true.
* `Combat.SpawnProtection.ApplyToReinforcements`: If true, enemies that spawn during a mission (i.e. are not present on the first turn) will be protected. Defaults to true.

## Weapon tooltips

This tweak makes minor changes to the tooltips shown when hovering over weapons:

* Overrides damage calculation on weapon display to show full damage potential (shots * projectiles)

This tweak is enabled if `Fixes.WeaponTooltip=true` is set to true in _mod.json_. There are no customizations for this tweak.
