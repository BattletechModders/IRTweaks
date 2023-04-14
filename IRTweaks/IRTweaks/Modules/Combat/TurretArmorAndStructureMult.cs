namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(Turret))]
    [HarmonyPatch("ArmorMultiplier", MethodType.Getter)]

    static class Turret_ArmorMultiplier_Patch
    {
        static bool Prepare() => Mod.Config.Combat.TurretArmorAndStructure.ArmorMultiplierTurret > 0f;

        static void Prefix(ref bool __runOriginal, Turret __instance, ref float __result)
        {
            if (!__runOriginal) return;

            __result = Mod.Config.Combat.TurretArmorAndStructure.ArmorMultiplierTurret;
            Mod.Log.Debug?.Write($"Applying ArmorMultiplierTurret {__result} to {__instance.Description.UIName}");
            __runOriginal = false;
        }
    }

    [HarmonyPatch(typeof(Turret))]
    [HarmonyPatch("StructureMultiplier", MethodType.Getter)]

    static class Turret_StructureMultiplier_Patch
    {
        static bool Prepare() => Mod.Config.Combat.TurretArmorAndStructure.StructureMultiplierTurret > 0f;

        static void Prefix(ref bool __runOriginal, Turret __instance, ref float __result)
        {
            if (!__runOriginal) return;

            __result = Mod.Config.Combat.TurretArmorAndStructure.StructureMultiplierTurret;
            Mod.Log.Debug?.Write($"Applying StructureMultiplierTurret {__result} to {__instance.Description.UIName}");
            __runOriginal = false;
        }
    }
}
