using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using HBS.Collections;
using IRTweaks.Helper;

namespace IRTweaks.Modules.Misc
{
    [HarmonyPatch(typeof(MechDef), MethodType.Constructor,
        new Type[] {typeof(ChassisDef), typeof(string), typeof(MechDef) })]
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