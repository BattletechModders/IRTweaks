# IRTweaks
This is a mod for the [HBS BattleTech](http://battletechgame.com/) game that includes a variety of tweaks, changes, and modifications to the base game. A wide range of effects is covered, and all of the options can be selectively enabled. A short summary of the features includes:

* **BraceOnMeleeWithJuggernaut**: If a pilot has the Guts 8 ability (typically Juggernaut), braces the unit after a melee or DFA attack. A direct copy of [RealityMachina's Better-Juggernaut](https://github.com/RealityMachina/Better-Juggernaut).
* **Bulk Purchasing**: Provides buttons and keyboard shortcuts that allow bulk purchasing and selling of items in the store.
* **Combat Log**: Provides an in-game log that captures text from floaties and preserves them in a readable format.
* **Campaign Disabler**: Disables the HBS campaign button, to prevent errors with various mod packs.
* **Combat Saves Disabler**: Combat saves are prone to errors during vanilla gameplay, but especially so during modded gameplay. This disables the UI selection that allows in-combat saves to be made.
* **Multiplayer Hash Disabler**: Disables a mod-hash calculated on startup that's only used to validate multiplayer games are compatible. Saves 2-3s of load time.
* **Extended Stats**: Allows pilots to be assigned Statistic values above the normal bounds of 1-10.
* **Flexible Sensor Lock**: Using a Sensor Lock action does not count as movement or firing. This allows it to be combined with actions in a unit's activation.
* **Nuanced Called Shot Modifier**: The modifier for called shots (aka offensive push) is driven by the pilot's tactics skill, ability, and pilot tags. It can also be influenced by gear.
* **Random Start by Difficulty Menu**: Allows an option in the new-game difficulty menu to be associated with user-created lists of starting mechs.
* **Restrict Called Shots to Head**: You can only select the head on a called shot if the target is shutdown, prone, or you have special equipment.
* **ShowAllArgoUpgrades**: Shows all available argo upgrades, instead of hiding the ones that require additional unlocks.
* **SimGameDifficultyLabelsReplacer**: Allows customization of the labels on the 'difficulty' bar when you start a new career or campaign game. 
* **SkirmishReset**: This fix is a modder's resource. Skirmish saves the mechDefs that were customized, which can result in an ever-spinny when itemDefs are changed or mods are disabled. When enabled, this fix will always reset the Skirmish lances and mech definitions to the base state by deleting all customizations.
* **SkipDeleteSavePopup**: Disables the 'are you sure' prompt when you delete save games.
* **Spawn Protection**: Provides high evasion, braced, and guarded status to units when they spawn. This can prevent first-turn damage during mission start, or to reinforcements that spawn close to the player.
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
* Allows shift+click on the -/+ in the bulk-purchase and bulk-sell screens to increment the count by -/+5
* Allows control+click on the -/+ in the bulk-purchase and bulk-sell screens to increment the count by -/+20

This tweak is enabled if `Fixes.BulkPurchasing=true` is set to true in _mod.json_. There are no customizations for this tweak.

## Nuanced Called Shot Modifier

This tweak allows fine-grained tuning of the Called Shot modifiers. The game exposes only a single modifier value (via `CombatGameConstants:ToHitOffensivePush`), which is used in all cases. This tweak allows both the pilot and the actor to have an influence on these outcomes as well. The calculation is:

`CalledShotDefaultMod + CalledShotPilotSkillsMod + CalledShotPilotTagsMod + CalledShotUnitMod = CalledShotModifier`

#### Pilot Skills Modifier

A pilots skills and abilities determine the `CalledShotPilotSkillsMod`. Their **Tactics** skill defines the value of the modifier, as shown in the table below:

A MechWarriors's **Tactics** skill adds a flat modifier to the base initiative defined by the unit tonnage. This value is graduated, as defined in the table below.

| Skill            | 1    | 2    | 3    | 4    | 5    | 6    | 7    | 8    | 9    | 10   | 11   | 12   | 13   |
| ---------------- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| Modifier         | +0   | +1   | +1   | +2   | +2   | +3   | +3   | +4   | +4   | +5   | +6   | +7   | +8   |
| +Level 5 Ability | NA   | NA   | NA   | NA   | +3   | +4   | +4   | +5   | +5   | +6   | +7   | +8   | +9   |
| +Level 8 Ability | NA   | NA   | NA   | NA   | NA   | NA   | NA   | +6   | +6   | +7   | +8   | +9   | +10  |

#### Pilot Tags Modifier

The pilot's tags determine the `CalledShotPilotTagsMod`. Every tag defined in `ToHitCfg.CalledShotPilotTags` (in _mod.json_) applies a specific modifier if the pilot also possesses that tag. The modifiers from all applicable tags are summed into `CalledShotPilotTagsMod`.

> Example: ToHitCfg.CalledShotPilotTags is { "pilot_drunk" : 2, "pilot_reckless" : 1, pilot_assassin": -2,  }. If the pilot has both the pilot_drunk and pilot_reckless and tags, their CalledShotPilotTagsMod would be +3.

#### Unit Modifier

Units that have `IRTCalledShotMod` in their _StatCollection_ will take this modifier as the `CalledShotUnitMod `. The _StatCollection_ assumes an int value.

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
