using BattleTech;
using System.Collections.Generic;
using System.Linq;

namespace IRTweaks {

    class PilotHelper {

        public static string GetPilotKey(Pilot pilot) {
            return $"{pilot?.Name}_{pilot?.ParentActor?.DisplayName}_{pilot?.GUID}_{pilot?.ParentActor?.GUID}";
        }

        public static int GetCalledShotModifier(Pilot pilot) {
            int mod = 0;

            string pilotKey = GetPilotKey(pilot);
            if (!ModState.PilotCalledShotModifiers.ContainsKey(pilotKey)) {
                Mod.Log.Debug($" Calculating calledShotModifier for pilot:{pilotKey}");
                int defaultMod = Mod.Config.Combat.CalledShot.Modifier;
                int tacticsMod = GetTacticsModifier(pilot);
                int tagsCSMod = GetTagsModifier(pilot, Mod.Config.Combat.CalledShot.PilotTags);
                int calledShotMod = defaultMod + (-1 * (tacticsMod + tagsCSMod));
                Mod.Log.Debug($" Pilot:{pilotKey} has calledShotMod:{calledShotMod} = defaultMod:{defaultMod} + (-1 * (tacticsMod:{tacticsMod} + tagsCSMod:{tagsCSMod}))");
                ModState.PilotCalledShotModifiers[pilotKey] = calledShotMod;
            } else {
                mod = ModState.PilotCalledShotModifiers[pilotKey];
            }
            
            return mod;
        }

        public static int GetGunneryModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Gunnery, "AbilityDefG5", "AbilityDefG8");
        }

        public static int GetGutsModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Guts, "AbilityDefGu5", "AbilityDefGu8");
        }

        public static int GetPilotingModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Piloting, "AbilityDefP5", "AbilityDefP8");
        }

        public static int GetTacticsModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Tactics, "AbilityDefT5A", "AbilityDefT8A");
        }

        public static void LogPilotStats(Pilot pilot) {
            if (Mod.Config.Debug) {
                int normedGuts = NormalizeSkill(pilot.Guts);
                int gutsMod = GetGutsModifier(pilot);
                int normdPilot = NormalizeSkill(pilot.Piloting);
                int pilotingMod = GetPilotingModifier(pilot);
                int normedTactics = NormalizeSkill(pilot.Tactics);
                int tacticsMod = GetTacticsModifier(pilot);

                Mod.Log.Debug($"{pilot.Name} skill profile is " +
                    $"g:{pilot.Guts}->{normedGuts}={gutsMod}" +
                    $"p:{pilot.Piloting}->{normdPilot}={pilotingMod} " +
                    $"t:{pilot.Tactics}->{normedTactics}={tacticsMod} "
                    );
            }
        }

        private static readonly Dictionary<int, int> ModifierBySkill = new Dictionary<int, int> {
            { 1, 0 },
            { 2, 1 },
            { 3, 1 },
            { 4, 2 },
            { 5, 2 },
            { 6, 3 },
            { 7, 3 },
            { 8, 4 },
            { 9, 4 },
            { 10, 5 },
            { 11, 6 },
            { 12, 7 },
            { 13, 8 }
        };

        // Process any tags that provide flat bonuses
        private static int GetTagsModifier(Pilot pilot, Dictionary<string, int> tagsDict) {
            int mod = 0;

            foreach (string tag in pilot.pilotDef.PilotTags.Distinct()) {
                if (tagsDict.ContainsKey(tag)) {
                    int tagMod = tagsDict[tag];
                    Mod.Log.Debug($"Pilot {pilot.Name} has tag:{tag}, applying modifier:{tagMod}");
                    mod += tagMod;
                }
            }

            return mod;
        }

        private static int NormalizeSkill(int rawValue) {
            int normalizedVal = rawValue;
            if (rawValue >= 11 && rawValue <= 14) {
                // 11, 12, 13, 14 normalizes to 11
                normalizedVal = 11;
            } else if (rawValue >= 15 && rawValue <= 18) {
                // 15, 16, 17, 18 normalizes to 14
                normalizedVal = 12;
            } else if (rawValue == 19 || rawValue == 20) {
                // 19, 20 normalizes to 13
                normalizedVal = 13;
            } else if (rawValue <= 0) {
                normalizedVal = 1;
            } else if (rawValue > 20) {
                normalizedVal = 13;
            }
            return normalizedVal;
        }

        private static int GetModifier(Pilot pilot, int skillValue, string abilityDefIdL5, string abilityDefIdL8) {
            int normalizedVal = NormalizeSkill(skillValue);
            int mod = ModifierBySkill[normalizedVal];
            foreach (Ability ability in pilot.Abilities.Distinct()) {
                Mod.Log.Trace($"Pilot {pilot.Name} has ability:{ability.Def.Id}.");
                if (ability.Def.Id.ToLower().Equals(abilityDefIdL5.ToLower()) || ability.Def.Id.ToLower().Equals(abilityDefIdL8.ToLower())) {
                    Mod.Log.Debug($"Pilot {pilot.Name} has targeted ability:{ability.Def.Id}, boosting their modifier.");
                    mod += 1;
                }

            }
            return mod;
        }

    }
}
