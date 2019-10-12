# IRTweaks
This mod for the [HBS BattleTech](http://battletechgame.com/) game includes miscellaneous tweaks and changes that aren't large enough to justify a separate mod. They cover a wide range of effects, and can be selectively enabled as you like. A summary of current features includes:

* **Nuanced Called Shot Modifier**: The modifier for called shots (aka offensive push) is driven by the pilot's tactics skill, ability, and pilot tags. It can also be influenced by gear.
* **Restrict Called Shots to Head**: You can only select the head on a called shot if the target is shutdown, prone, or you have special equipment.
* **Sensor Lock Freedom**: Using a Sensor Lock action does not count as movement or firing. This allows it to be combined with actions in a unit's activation.
* **SkirmishReset**: This fix is a modder's resource. Skirmish saves the mechDefs that were customized, which can result in an ever-spinny when itemDefs are changed or mods are disabled. When enabled, this fix will always reset the Skirmish lances and mech definitions to the base state by deleting all customizations.

This mod replaces the following mods, which used to be stand-alone:

* *BTRandomStartByDifficultyMenu*
* *CombatSaveDisabler*
* *IRUITweaks*
* *SpawnProtection*

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

## Restrict Called Shots to Head

This tweak changes the called shot dialog to disallow selecting the head unless the target unit is shutdown or prone. This only applies to mech targets; vehicle and building targets may be targeted normally.

Units that have `IRTCalledShotAlwaysAllow : true` in their _StatCollection_ can select heads even if the target is shutdown or prone. This would typically come in through equipment and similar effects.

This tweak has no customization. 

## Sensor Lock Freedom

This tweak changes the SensorLock behaviors such that using the action doesn't end your turn. This previously occurred because SensorLock actions counted as both movement and firing (in a very weird way). When this tweak is enabled, you can sensor lock at any point during your activation.

This tweak has no customization. 