using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using us.frostraptor.modUtils;

namespace IRTweaks.Helper {
    public static class CalledShotHelper {
        public static int GetCalledShotModifier(Pilot pilot) {
            int mod = 0;

            string pilotKey = GetPilotKey(pilot);
            if (!ModState.PilotCalledShotModifiers.ContainsKey(pilotKey)) {
                Mod.Log.Debug($" Calculating calledShotModifier for pilot:{pilotKey}");
                int defaultMod = Mod.Config.Combat.CalledShot.Modifier;
                int tacticsMod = SkillUtils.GetTacticsModifier(pilot);
                int tagsCSMod = SkillUtils.GetTagsModifier(pilot, Mod.Config.Combat.CalledShot.PilotTags);
                int calledShotMod = defaultMod + (-1 * (tacticsMod + tagsCSMod));
                Mod.Log.Debug($" Pilot:{pilotKey} has calledShotMod:{calledShotMod} = defaultMod:{defaultMod} + (-1 * (tacticsMod:{tacticsMod} + tagsCSMod:{tagsCSMod}))");
                ModState.PilotCalledShotModifiers[pilotKey] = calledShotMod;
            } else {
                mod = ModState.PilotCalledShotModifiers[pilotKey];
            }

            return mod;
        }

        public static string GetPilotKey(Pilot pilot) {
            return $"{pilot?.Name}_{pilot?.ParentActor?.DisplayName}_{pilot?.GUID}_{pilot?.ParentActor?.GUID}";
        }
    }
}
