using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using Harmony;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(Turret))]
    [HarmonyPatch("ArmorMultiplier", MethodType.Getter)]
    
    public static class Turret_ArmorMultiplier_Patch
    {
        static bool Prepare() => Mod.Config.Combat.TurretArmorAndStructure.ArmorMultiplierTurret > 0f;

        public static bool Prefix(Turret __instance, ref float __result)
        {
            __result = Mod.Config.Combat.TurretArmorAndStructure.ArmorMultiplierTurret;
            Mod.Log.Debug?.Write($"Applying ArmorMultiplierTurret {__result} to {__instance.Description.UIName}");
            return false;
        }
    }

    [HarmonyPatch(typeof(Turret))]
    [HarmonyPatch("StructureMultiplier", MethodType.Getter)]
    
    public static class Turret_StructureMultiplier_Patch
    {
        static bool Prepare() => Mod.Config.Combat.TurretArmorAndStructure.StructureMultiplierTurret > 0f;

        public static bool Prefix(Turret __instance, ref float __result)
        {
            __result = Mod.Config.Combat.TurretArmorAndStructure.StructureMultiplierTurret;
            Mod.Log.Debug?.Write($"Applying StructureMultiplierTurret {__result} to {__instance.Description.UIName}");
            return false;
        }
    }
}
