# IRTweaks
This is a mod for the [HBS BattleTech](http://battletechgame.com/) game that includes a variety of tweaks, changes, and modifications to the base game. A wide range of effects is covered, and all of the options can be selectively enabled. A short summary of the features is provided below. You can enable or disable each tweak in `mod.json` by setting the appropriate `Fixes` value to true or false.

* **AbilityResourceFix**: If an Ability has `"Resource" : "ConsumesActivation",` the actors activation will be consumed upon use. In addition, the ability will be <i>unable</i> to be used if the actor has done any other actions (shooting, moving, etc).
* **AlternativeMechNamingStyle**: Used by RogueTech to set unique naming styles consistent with the **LowVisiblity** mod. No other systems should use this.
* **BuildingDamageColorChange**: Changes the floating damage on buildings to be dark blue to distinguish this damage type. Thanks to **Gnivler** for this fix!
* **BraceOnMeleeWithJuggernaut**: If a pilot has the Guts 8 ability (typically Juggernaut), braces the unit after a melee or DFA attack. A direct copy of [RealityMachina's Better-Juggernaut](https://github.com/RealityMachina/Better-Juggernaut).
* **BreachingShotIgnoresAllDR**: Makes breaching shot and rear-arc shots ignore damage resistance from `DamageReductionMultiplierAll` as well
* **BulkPurchasing**: Provides buttons and keyboard shortcuts that allow bulk purchasing and selling of items in the store.
* **BulkScrapping**: Allows players to scrap all chassis of a given weight from the storage screen at once. Hold Alt and click one of the weight filters at the top of the storage screen to scrap everything in that category.
* **CalledShotTweaks**: The modifier for called shots (aka offensive push) is driven by the pilot's tactics skill, ability, and pilot tags. It can also be influenced by gear. Options allow disabling the ability to called shot 
* **CombatLog**: Provides an in-game log that captures text from floaties and preserves them in a readable format.
* **CTDestructInjuryFix**: Re-enables CT destruction causing Maximum pilot injuries, forcing them to win a roll against `SimGameConstants.Pilot.IncapacitatedDeathChance` to survive at end of contract.
* ~~**CustomShopsRepHandling**: Disables internal handling of shop button when using rearranged menu (so CustomShops can handle reputation-based enable/disable). false by default.~~ deprecated
* **DifficultyModsFromStats**: Enables company stat "IRTweaks_DiffMod" to be added or subtracted (if negative) to career difficulty score modifier. Used in events or any Results block.
* **DisableCampaign**: Disables the HBS campaign button, to prevent errors with various mod packs.
* **DisableDebug**: Disables the "Debug career" buttons even when test tools enabled.
* **DisableCombatRestarts**: Mission restarts lead to corruption at the salvage screen in vanilla, and especially in a modded game. This disables the UI selection that allows in-combat saves to be made.
* **DisableCombatSaves**: Combat saves are prone to errors during vanilla gameplay, but especially so during modded gameplay. This disables the UI selection that allows in-combat saves to be made.
* **DisableLowFundsNotification**: Disables the irritating "low funds" notification.
* **DisableMPHashCalculation**: Disables a mod-hash calculated on startup that's only used to validate multiplayer games are compatible. Saves 2-3s of load time.
* **ExtendedStats**: Allows pilots to be assigned Statistic values above the normal bounds of 1-10.
* **FactionValueFix**: Fixes career FactionValues in saves when FactionValues are changed mid-career. Previously, if a FactionValue was set to "IsCareerIgnoredContractTarget": false,, that false value would persist in saves even when the actual FactionValue in Faction.json was changed, preventing that faction from appearing as a procedural contract target for that career.
* **FlexibleSensorLock**: Using a Sensor Lock action does not count as movement or firing. This allows it to be combined with actions in a unit's activation.
* **MaxArmorMaxesArmor**: "Max Armor" button in mechbay now sets armor to the maximum possible for the chassis; ignores available tonnage. Holding Control while clicking will use vanilla functionality (does not work well with MechEngineer armors like ferro, etc.)
* **MechbayLayoutFix**: Moves a few UI elements in the mechbay to work better in a MechEngineer based mod. Thanks to Tiraxx for the idea!
* **MechbayAdvancedStripping**: Holding Left or Right Control while clicking the "Strip Equipment" button in the mechbay will strip <i>only</i> weapons and ammo.
* **MultiTargetStat**: This allows units to gain the Multi-Target ability from a Statistic, which can be applied via effects.
* **OnWeaponFireFix**: Fixes OnWeaponFire effects so that extendDurationOnTrigger and triggerLimit work properly for StatisticEffects. Also necessary for OnWeaponHit effects and self-knockdown check
* **PainTolerance**: Provides Guts-based resistance to injuries. See below for details.
* **PathfinderTeamFix**: Mission Control introduces pathfinding units that have no Team associated with them. This breaks some mods, which this fix remediates.
* **Random Start by Difficulty Menu**: Allows an option in the new-game difficulty menu to be associated with user-created lists of starting mechs.
* **ReduceSaveCompression**: By default the game is setup to use an aggressive compression on save game files. This slows down game load. By setting this to true saves will use more space on disk but load faster. Thanks to **Gnivler** for this fix!
* **RestoreMechTagsOnReady**: Restores MechTags when Mechs are readied from storage (necessary for knockdown on fire ignore tags, shutdown on-hit ignore tags, and anything else that checks player mech tags to do magic.
* **ScaleObjectiveBuildingStructure**: Increases the structure of any building that is the target of an objective. This allows high difficulty attack bases to be more difficult, and high difficulty defend bases to be easier.
* **ShowAllArgoUpgrades**: Shows all available argo upgrades, instead of hiding the ones that require additional unlocks.
* **SimGameDifficultyLabelsReplacer**: Allows customization of the labels on the 'difficulty' bar when you start a new career or campaign game. 
* **SkirmishAlwaysUnlimited**: This allows you to drop from Skirmish even if your lances violate the limits of the currently selected operation type.
* **SkirmishReset**: This fix is a modder's resource. Skirmish saves the mechDefs that were customized, which can result in an ever-spinny when itemDefs are changed or mods are disabled. When enabled, this fix will always reset the Skirmish lances and mech definitions to the base state by deleting all customizations.
* **SkipDeleteSavePopup**: Disables the 'are you sure' prompt when you delete save games.
* **Spawn Protection**: Provides high evasion, braced, and guarded status to units when they spawn. This can prevent first-turn damage during mission start, or to reinforcements that spawn close to the player.
* **Streamlined Main Menu**: This tweaks the layout of the main Argo UI to move the most commonly accessed buttons directly to the sidebar.
* **Targeting Computer Tonnage**: This displays the tonnage of your target in the targeting computer display (next to where it displays weight class in vanilla). 
* **Urban Explosions Fix**: This corrects a subtle bug in HBS code that causes exploding buildings to not sequence properly. Unfortunately enabling this fix makes buildings take significantly longer to be destroyed. This will be improved in a future fix.
* **Weak Against Melee Fix**: Fixes the "Weak Against Melee" debuff for Turrets/Vehicles to accurately report melee damage modifiers from CombatGameConstants
* **Weapon Tooltips**: Modifies the weapon tooltips to more accurately report damage when a weapon uses extensions provided by [CustomAmmoCategories](https://github.com/CMiSSioN/CustomAmmoCategories).
* **DamageReductionInCombatHud**: If compiled with CAC support (and CustomAmmoCategories is active), displays each unit's damage reduction alongside its evasion pips, removing the default >>>> evasion display. While using this, strongly consider `"EvasiveNumberWidth": 90` in CustomAmmoCategories's settings - the default value of 25 will *not* look good.

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

## DifficultyModsFromStats

Enables Company stat "IRTweaks_DiffMod" to be added or subtracted (if negative) to career difficulty score modifier. Used in events or any Results block. E.g. an event with the following block would increase the Career Difficulty score by 0.1

```
"Stats": [
	{
		"typeString": "System.Single",
		"name": "IRTweaks_DiffMod",
		"value": "0.1",
		"set": false,
		"valueConstant": null
	}
],
```

IRTweaks is also now packaged with a SimGameStatDesc to prevent errors from displaying when setting the above company stat in events, as follows. Any field with "result" can also be set to `""` to hide the stat change from player view in the event screen.
```
{
	"Description": {
		"Id": "SimGameStatDesc_IRTweaks_DiffMod",
		"Name": "IRTweaks_DiffMod",
		"Details": "Career Difficulty Modified",
		"Icon": null
	},
	"setResult": "Career Difficulty set to {RES_VALUE.ToString}",
	"positiveResult": "Career Difficulty Modifier increased by {RES_VALUE.ToString}",
	"negativeResult": "Career Difficulty Modifier decreased by {RES_VALUE.ToString}",
	"temporalSetResult": "Career Difficulty set to {RES_VALUE.ToString} for {RES_DURATION.ToString} days",
	"temporalPositiveResult": "Career Difficulty Modifier increased by {RES_VALUE.ToString} for {RES_DURATION.ToString} days",
	"temporalNegativeResult": "Career Difficulty Modifier decreased by {RES_VALUE.ToString} for {RES_DURATION.ToString} days",
	"infinitiveSetResult": "Career Difficulty set to {RES_VALUE.ToString}",
	"infinitivePositiveResult": "Career Difficulty Modifier increased by {RES_VALUE.ToString}",
	"infinitiveNegativeResult": "Career Difficulty Modifier decreased by {RES_VALUE.ToString}"
}
```

## Disable CT Destruction Max Injuries By Tag

This setting, "DisableCTMaxInjureTags" under the "Combat" section of the mod.json settings, gives a list of MechDef tags which, if present, will prevent Max Injury from occurring when CT is destroyed. Intended for use with Battle Armor.

## TorsoMountStatName

e.g. `"TorsoMountStatName": "isTorsoMount",`
This setting under the "Combat" section of mod.json settings is simply the name of a boolean stat that torso-mounted cockpits set to true. If present on a mech, will prevent that mech from being destroyed when the head is destroyed. Temporary fix until MechEngineer implements it properly.

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

## Cheat Detection
This module allows the use of cheats such as CheatEngine or other memory address editors to be detected.

`CheatDetection` - bool, if true CheatDetection is enabled.

`CheatDetectionNotify` - bool, if true a popup is displayed ingame when a cheat was detected.

`CheatDetectionStat` - string, statname of bool statistic written to CompanyStats when a cheat has been detected.

## To Hit Mods
This tweaks allows you to define modifiers on hitchance based on matching statistic values between attacker and target. These modifiers may be absolute (accuracy is always affected) or they may be relative, i.e a multiplier of target Defense and/or Evasion. Further, these matching statistic values can be applied to specific weapons, or to all weapons on the unit. All references to "ToHit" use the "lower values = better hit %" convention; a negative modifier improves chances to hit. EVASIVE and DEFENSE values refered to are the raw effect on ToHit; offsetting 1 EVASIVE is the same as giving -1 ToHit (or +1 Accuracy from the players point of view). These examples are all aimed toward _improving_ ToHit, but developers can certainly assign positive values to the statistics to creat accuracy penalties if desired. You can make AC2's offset some evasion on LAMs and make it functionally impossible to hit a Kirov with a Long Tom, etc.

The following is an example settings block in mod.json for this tweak:

```
"ToHitStatMods": {
				"WeaponToHitMods": [
					{
						"SourceStatName": "WeaponIgnoreEVASIVE_AeroUnit",
						"TargetStatName": "AeroUnit",
						"Multi": false,
						"Type": "EVASIVE"
					}
				],
				"ActorToHitMods": [
					{
						"SourceStatName": "ActorIgnoreDEFENSE_AeroUnit",
						"TargetStatName": "AeroUnit",
						"Multi": true,
						"Type": "DEFENSE"
					},
					{
						"SourceStatName": "ActorABSOLUTE_AeroUnit",
						"TargetStatName": "AeroUnit",
						"Multi": false,
						"Type": "ABSOLUTE"
					}
				]
			},
```
The only fundamental difference is that WeaponToHitMods will only apply to the weapon that carries the statistic, while ActorToHitMods apply to all weapons on the unit. For both WeaponToHitMods and ActorToHitMods the subfields function identically:

`SourceStatName` - name of float statistic (name is arbitrary) on either a weapondef (for WeaponToHitMods) or otherwise given to a unit that defines the ToHit modifier. **the actual value of the statistic should be set by equipment, ability, etc**

`TargetStatName` - name of bool statistic (name is arbitrary) on the intended target units. For example, a target has `AeroUnit`, and the attacker has `ActorIgnoreDEFENSE_AeroUnit` set to a value other than 0, then the ToHit modifier value of `ActorIgnoreDEFENSE_AeroUnit` will apply.

`Multi`: bool. If true, the value of SourceStatName statistic is taken to be a multiplier of either EVASION or DEFENSE. If false, the value of SourceStatName is added directly to ToHit.

`Type`: string, valid values are "ABSOLUTE", "DEFENSE", and "EVASION". DEFENSE and EVASION make the effect of the modifier dependent on the target defense or evasion. 

For example, if a units value for SourceStatName is set to -0.5, `Multi: true`, and `Type: "EVASION"`, the target would have half of their evasion "offset" by the modifier. If it were set to -1, <i>all</i> of their evasion would be offset by the modifier. As another example, if a units value for SourceStatName was -4, `Multi: false`, and `Type: "DEFENSE"`, and the target unit only had a defense modifier of 2, then _only_ those 2 would be offset by the modifier. Similar to vanilla functionality that ignores evasion pips, Type settings of "DEFENSE" and "EVASION" cannot improve hit chance past simply offsetting target evasion or defense. `Type: "ABSOLUTE"` _always_ applies, similar to a normal weapon accuracy buff. Finally **Multi: true cannot be used with "ABSOLUTE"** and will result in an error in logs and no effect in-game.

As further examples, this would be a statusEffects block in a component for "target" units using the above setting block:
```
"statusEffects": [
		{
			"durationData": {
				"duration": -1,
				"stackLimit": 1
			},
			"targetingData": {
				"effectTriggerType": "Passive",
				"effectTargetType": "Creator",
				"showInTargetPreview": false,
				"showInStatusPanel": false
			},
			"effectType": "StatisticEffect",
			"Description": {
				"Id": "AEROUNIT",
				"Name": "aero unit",
				"Details": "aero unit thing",
				"Icon": "uixSvgIcon_equipment_Cockpit"
			},
			"nature": "Buff",
			"statisticData": {
				"statName": "AeroUnit",
				"operation": "Set",
				"modValue": "true",
				"modType": "System.Boolean"
			}
		}
	],
```

This would be a statusEffects block on a component for the attacking unit using the above settings block. The statistic sets -1, and the corresponding mod.json setting has `"Multi": true`, so all of the target Defense would be offset.

```
"statusEffects": [
		{
			"durationData": {
				"duration": -1,
				"stackLimit": 1
			},
			"targetingData": {
				"effectTriggerType": "Passive",
				"effectTargetType": "Creator",
				"showInTargetPreview": true,
				"showInStatusPanel": true
			},
			"effectType": "StatisticEffect",
			"Description": {
				"Id": "ActorIgnoreDEFENSE_AeroUnitStat",
				"Name": "Ignore Aero Defense",
				"Details": "Increased Sight/Sensors more better thing",
				"Icon": "uixSvgIcon_equipment_Cockpit"
			},
			"nature": "Buff",
			"statisticData": {
				"statName": "ActorIgnoreDEFENSE_AeroUnit",
				"operation": "Set",
				"modValue": "-1",
				"modType": "System.Single"
			}
		}
	],
```

This would be a statusEffects block on a WeaponDef for the attacking unit using the above settings block. The statistic sets -2, and the corresponding mod.json setting has `"Multi": false` so the ToHit chance would be improved by at most 2 (or 1 evasion pip).

**Note that any statusEffect defined in a component propagates to the whole unit, so if your intent is to have the ToHit modifier only affect this weaspon, the `statName` MUST be defined in WeaponToHitMods in the mod.json.

```
"statusEffects": [
		{
			"durationData": {
				"duration": -1,
				"stackLimit": 1
			},
			"targetingData": {
				"effectTriggerType": "Passive",
				"effectTargetType": "Creator",
				"showInTargetPreview": true,
				"showInStatusPanel": true
			},
			"effectType": "StatisticEffect",
			"Description": {
				"Id": "WeaponIgnoreEVASIVE_AeroUnitStat",
				"Name": "Ignore Aero Evasive",
				"Details": "Increased Sight/Sensors more better thing",
				"Icon": "uixSvgIcon_equipment_Cockpit"
			},
			"nature": "Buff",
			"statisticData": {
				"statName": "WeaponIgnoreEVASIVE_AeroUnit",
				"operation": "Set",
				"modValue": "-2",
				"modType": "System.Single"
			}
		}
	],
```

## Damage Mods
This tweak allows you to define multipliers to damage of various types: normal, heat, stability, and AP damage (from CAC). Units' access to the modifiers is first controlled via a stat check; the unit must have boolean true value for a statistic named the same as StatName in the following settings. This statistic can be added via equipment, pilot abilities, etc. Probability corresponds to the probability that the modifier will be applied; set to 1 to always apply the modifier. Multiplier represents the actual multiplier to be applied.

if `DisplayFloatiesOnTrigger` = true, a floatie will be generated over the unit if/when the bonus damage is activated.

These values are defined using the following settings:
```json
"DamageModsBySkill": {
		"DisplayFloatiesOnTrigger": true,
		"StabilityMods": [
			{
				"StatName": "stabMod1",
				"Probability": 0.9,
				"Multiplier": 25.0
			},
			{
				"StatName": "stabMod2",
				"Probability": 0.2,
				"Multiplier": 0.1
			}
		],
		"HeatMods": [
			{
				"StatName": "heatMod1",
				"Probability": 0.9,
				"Multiplier": 5.0
			},
			{
				"StatName": "heatMod2",
				"Probability": 0.2,
				"Multiplier": 0.1
			}
		],
		"APDmgMods": [],
		"StdDmgMods": []
	},
```
Using the above example, a unit with a stat effect `stabMod1 = true` would inflict 25 times as much stability damage 90% of the time, while a unit with `stabMod2 = true` would inflict 10% of normal stability damage 20% of the time.

## Effects on Brace `BraceEffectConfig`

This option defines a block of stat effects to be applied when a unit issues a "Brace" command. Intended to be used for an overhaul of the Cover/Braced/Bulwark damage resistance system, in conjunction with the `BreachingShotIgnoresAllDR` tweak (since `DamageReductionMultiplierAll` effects would replace the standard Cover/Braced/Bulwark DR set in combatgameconstants.

Example config block:
```"BraceEffectConfig": {
	"ID": "EffectsOnBrace",
	"Name": "EffectsOnBracing",
	"Description": "Effects happen on brace",
	"effectDataJO": [
		{
			`"durationData": {
			"duration": 1,
			"stackLimit": 1
			},
			"targetingData": {
				"effectTriggerType": "Passive",
				"effectTargetType": "Creator",
				"showInStatusPanel": true
			},
			"effectType": "StatisticEffect",
			"Description": {
				"Id": "BraceAOE_DR",
				"Name": "AOE DR",
				"Details": "Braced Units take 20% less AOE damage.",
				"Icon": "allied-star"
			},
			"nature": "Buff",
			"statisticData": {
				"statName": "CACAoEDamageMult",
				"operation": "Float_Add",
				"modValue": "-0.2",
				"modType": "System.Single"
			}
		}
```

## Flexible Sensor Lock
This tweak allows units to use sensor lock without it consuming their action or movement. This can be limited to units with a specific ability or stat. The ability to restrict this is defined by the `AbilityOpts.FlexibleSensorLockId` value in mod.json, which defaults to `AbilityDefT8A`. The stat is defined in `AbilityOpts.FreeActionStatName`, which defaults `IR_FreeSensorLock`. If the `Combat.FlexibleSensorLock.AlsoAppliesToActiveProbe` is true, the ActiveProbe ability that some units possess will also not require an action or movement.

## OnWeaponFire special effects - `OnWeaponFireOpts` REQUIRES CAC and OnWeaponFireFix enabled

This "tweak" is currently only configured for a single functionality, but can be extended if requested. Current configuration allows for forcing a save against self-knockdown when weapons with the appropriate OnWeaponFire effects block are fired. The actual roll for knockdown takes place after the attacksequence has completed, so knockdown chance from multiple weapons can stack and become progressively more difficult or impossible to beat.

Example mod.json settings:
```
"OnWeaponFireOpts": {
	"SelfKnockdownCheckStatName": "SelfknockdownCheck_OnFire",
	"IgnoreSelfKnockdownTag": "big_chungus",
	"SelfKnockdownTonnageFactor": 0.01,
	"SelfKnockdownPilotingFactor": 0.01,
	"SelfKnockdownTonnageBonusThreshold": 100,
	"SelfKnockdownTonnageBonusFactor" = 0.05
	"SelfKnockdownBracedFactor": 100.0,
},
```
`SelfKnockdownCheckStatName` - name of statistic (_statistic_ *not* effect!) set in weapon that represents the "base chance" of suffering a knockdown due to firing that weapon.
`IgnoreSelfKnockdownTag` - units with this mechdef (or vehicledef) tag will ignore self-knockdown rolls (basically turns off this whole functionality for that unit)
`SelfKnockdownPilotingFactor` - this value X Piloting skill offset chance to self-knockdown
`SelfKnockdownBracedFactor` - if unit braced the previous round *and has not moved this round* this value will be added to offset chance of self-knockdown. Unit *can* still rotate in place.
`SelfKnockdownTonnageFactor` - unit tonnage x this value is added to knockdown resistance
`SelfKnockdownTonnageBonusThreshold` and `SelfKnockdownTonnageBonusFactor` - if the unit tonnage exceeds SelfKnockdownTonnageBonusThreshold, the tonnage up to the threshold gives resistance per SelfKnockdownTonnageFactor, while the remaining tons will gain per-ton resistance per SelfKnockdownTonnageBonusThreshold

So final formula for % to self-knockdown is SelfKnockdownCheckStatName - (SelfKnockdownPilotingFactor + SelfKnockdownBracedFactor)

Example stat block on weapon; if two of these weapons would fire, the "base chance" of self-knockdown would be 2.0
```
"statusEffects": [
		{
			"durationData": {
				"duration": 1,
				"stackLimit": 1
			},
			"targetingData": {
				"effectTriggerType": "OnWeaponFire",
				"effectTargetType": "Creator",
				"showInStatusPanel": false
			},
			"effectType": "StatisticEffect",
			"Description": {
				"Id": "WeaponEffect-SelfknockdownCheck_OnFire",
				"Name": "self knockdown check",
				"Details": "self knockdown check.",
				"Icon": "uixSvgIcon_run_n_gun"
			},
			"statisticData": {
				"statName": "SelfknockdownCheck_OnFire",
				"operation": "Float_Add",
				"modValue": "1.0",
				"modType": "System.Single"
			},
			"nature": "Buff"
		}
	],
```

## OnWeaponHit special effects - `OnWeaponHitOpts` REQUIRES CAC and OnWeaponFireFix enabled

This "tweak" is currently only configured for a single functionality, but can be extended if requested. Current configuration allows for forcing a save against shutdown when hit with a weapon using the appropriate OnFire effects. The actual roll for shutdown takes place after the attacksequence has completed, so shutdown chance from multiple weapons can stack and become progressively more difficult or impossible to beat.

Example mod.json settings:

```
"OnWeaponHitOpts": {
	"ForceShutdownOnHitStat": "OnHitShutdownChance",
	"IgnoreShutdownTag": "DontTazeMeBro",
	"ResistShutdownGutsFactor": 0.01
}
```

`ForceShutdownOnHitStat` - name of statistic (_statistic_ *not* effect!) set in weapon that represents the "base chance" of suffering a shutdown due to being hit by that weapon.
`IgnoreShutdownTag` - units with this mechdef (or vehicledef) tag will ignore shutdown rolls (basically turns off this whole functionality for that unit)
`ResistShutdownGutsFactor` - this value X Guts skill offset chance to shutdown

So final formula for % to self-knockdown is ForceShutdownOnHitStat - ResistShutdownGutsFactor

Example stat block on weapon; if two of these weapons would fire, the "base chance" of shutdown would be 2.0
```
"statusEffects": [
		{
			"durationData": {
				"duration": 1,
				"stackLimit": 1
			},
			"targetingData": {
				"effectTriggerType": "OnHit",
				"showInStatusPanel": false
			},
			"effectType": "StatisticEffect",
			"Description": {
				"Id": "WeaponEffect-OnHitShutdownChance",
				"Name": "on hit shutdown check",
				"Details": "on hit shutdown check.",
				"Icon": "uixSvgIcon_run_n_gun"
			},
			"statisticData": {
				"statName": "OnHitShutdownChance",
				"operation": "Float_Add",
				"modValue": "1.0",
				"modType": "System.Single"
			},
			"nature": "Buff"
		}
	],
```

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

### Overheat Injuries

When a mech takes overheat damage, each point of heat over the overheating limit reduces the resist chance by **OverheatResistPenaltyPerHeatPercentile**. The difference between the mech's maximum heat (at which point is shuts down) and it's overheating limit is taken as a spectrum from 0 - 100. The mech's current heat within this spectrum defines the resistance penalty.

> Example: A pilot with a resistance check of 60% overheats their Mech. The Mech has a maximum heat of 200, and an overheating limit of 120. 200 - 120 = 80 points on the overheating spectrum. If the Mech was currently as 180 heat points, the overheating points would be calculated as 180 -120 = 60 points. The ratio of points to spectrum is then calculated, or 80 / 60 = 0.75 * 100 = 75 points. Assuming PenaltyPerHeatDamageInjuryRatio = 1, then the resist check is reduced by -75%. The pilot cannot resist and thus takes the injury.

### Side Torso Destroyed Injuries

When a side torso is destroyed, a pilot normally takes a point of damage. The resist chance is reduced by `PainTolerance.SideLocationDestroyedResistPenalty`, which defaults to 10%.

### Center Torso Destroyed Injuries

**Requires `CTDestructInjuryFix` set to true!** When the center torso is destroyed, a pilot normally damage equal to their remaining health, incapacitating them. The resist chance is reduced by `PainTolerance.CTLocationDestroyedResistPenalty`, which defaults to 10%.

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

## Mechbay Refit Restrictions

This tweak allows you to restrict refitting certain components (using Component Tags) if the player does not have a matching Argo Upgrade. For example, if you set the component tags for `emod_engineslots_xl_center` to contain the following:
```
"ComponentTags" : {
	"items" : [
	"component_type_stock",
	"requires_refitHarness"
		],
	"tagSetSourceFile" : ""
	}
```
and in IRTweaks mod.json settings you have the following (under the "Misc" section):
```
"MechLabRefitReqs": {
	"MechLabRefitReqs":{
		"requires_refitHarness": "argoUpgrade_mechBay_refitHarness"
		}
	}
```

Then the player will be unable to fit an XL engine if they have not obtained the Argo upgrade with ID `argoUpgrade_mechBay_refitHarness`

## Random Start By Difficulty

This tweak allows you to define difficulty settings that impact the Career starts. This is useful for mods like RogueTech, which customizes your starting lance and faction reputation based upon a difficulty menu selection. The mod looks for two custom *DifficultyConstants*, each with a different behavior.

This tweak is enabled if `Fixes.RandomStartByDifficulty=true` is set to true in _mod.json_. Customizations are only expressed through the difficulty constants described below.

### Restrict More Settings to "Start Only"

This tweak allows the "Career start only" difficulty settings field to be expanded to hold >6 settings, and uses the following settings:
```
	"Misc": {
			"DifficultyUIScaling": {
				"StartOnlyScalar": 40,
				"StartOnlyPositionY": 40,
				"RegularPositionY": 40
			},
```
StartOnlyScalar - float, for each pair of additional "start only" settings, the start only field will be embiggened by this amount, and the "regular" settings field will be shrunk by this amount.

StartOnlyPositionY - float, adjusts vertical position of the "start only" settings field.

RegularPositionY - float, adjusts vertical position of the "regular" settings field.

### Expanded Stray Shot Control

This tweak allows you to further control raycasted Stray Shot behavior: supported options are Disabled, Buildings Only, Enemies Only, Enemies and Neutral, and All (Friendly Fire).

```json
{
	"ID": "diff_FriendlyFire",
	"Name": "Friendly Fire",
	"TelemetryEventName": "",
	"UIOrder": 0,
	"Tooltip": "Select what Stray Shot behavior you want - No Stray Shots, Buildings Only, Enemies Only, Enemies/Neutral, Friendly Fire",
	"Enabled": true,
	"Visible": true,
	"Toggle": false,
	"StartOnly": false,
	"DefaultIndex": 2,
	"Options": [
						{
			"ID": "diff_FF_Disabled",
			"Name": "Disabled",
			"TelemetryEventDesc": "",
			"DifficultyValue": 0,
			"CareerScoreModifier": -1,
			"DifficultyConstants": [
				{
					"ConstantType": "string",
					"ConstantName": "StrayShotsEnabled",
					"ConstantValue": "False"
				}
			]
		},
		{
			"ID": "diff_FF_Buildings",
			"Name": "Buildings Only",
			"TelemetryEventDesc": "",
			"DifficultyValue": 0,
			"CareerScoreModifier": -1,
			"DifficultyConstants": [
				{
					"ConstantType": "string",
					"ConstantName": "StrayShotsHitUnits",
					"ConstantValue": "False"
				}
			]
		},
		{
			"ID": "diff_FF_Off",
			"Name": "Enemies Only",
			"TelemetryEventDesc": "",
			"DifficultyValue": 0,
			"CareerScoreModifier": -1,
			"DifficultyConstants": [
				{
					"ConstantType": "string",
					"ConstantName": "StrayShotValidTargets",
					"ConstantValue": "0"
				}
			]
		},
		{
			"ID": "diff_FF_Neutral",
			"Name": "Enemies/Neutral",
			"TelemetryEventDesc": "",
			"DifficultyValue": 0,
			"CareerScoreModifier": 0,
			"DifficultyConstants": [
				{
					"ConstantType": "string",
					"ConstantName": "StrayShotValidTargets",
					"ConstantValue": "1"
				}
			]
		},
		{
			"ID": "diff_FF_On",
			"Name": "All",
			"TelemetryEventDesc": "",
			"DifficultyValue": 0,
			"CareerScoreModifier": 0.5,
			"DifficultyConstants": [
				{
					"ConstantType": "string",
					"ConstantName": "StrayShotValidTargets",
					"ConstantValue": "2"
				}
			]
		}
	]
}
```

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

## Damage Reduction In Combat Hud

This tweak replaces CAC's evasion number in the UI with a combined damage reduction and evasion pips display. Eg, instead of

```
3 >>>
```

the UI will now display something like

```
24% DR
3 pips >>>
```

(but more nicely alligned).

If you enable this, you will need to set `"EvasiveNumberWidth": 90` in your CustomAmmoCategoriesSettings.json. Failure to do this will result in weird UI interactions (ugly vertical text).
