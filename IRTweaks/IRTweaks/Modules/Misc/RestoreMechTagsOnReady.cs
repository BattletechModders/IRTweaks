using System;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(MechDef), MethodType.Constructor,
        new Type[] { typeof(ChassisDef), typeof(string), typeof(MechDef) })]
    public static class MechDef_constructor
    {
        static bool Prepare() => Mod.Config.Fixes.RestoreMechTagsOnReady;

        public static void Postfix(MechDef __instance, ChassisDef chassis, string newGUID, MechDef stockMech)
        {
            __instance.MechTags = stockMech.MechTags;
            //Traverse.Create(__instance).Property("MechTags").SetValue(stockMech.MechTags);
            Mod.Log.Trace?.Write($"Patched MechDef cctor to restore tags: {__instance.MechTags.ToJSON()}.");
        }
    }
}