using BattleTech.Rendering;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IRTweaks.Modules.Combat
{

    [HarmonyPatch(typeof(ObstructionGameLogic), "ExplodeBuildingIfNeeded", new Type[] { })]
    static class ObstructionGameLogic_ExplodeBuildingIfNeeded
    {
        [HarmonyPrepare]
        static bool Prepare() => Mod.Config.Fixes.ExplodingBuildingFix;

        [HarmonyPrefix]
        static void Prefix(ref bool __runOriginal, ObstructionGameLogic __instance)
        {
            if (!__runOriginal) return;

            if (__instance.IsBuildingEnabled && __instance.explodeBuildingSettings.explode)
            {
                float num = UnityEngine.Random.Range(25f, 30f);
                Vector3 vector = new Vector3(UnityEngine.Random.Range(0f, 1f), 0f, UnityEngine.Random.Range(0f, 1f));
                FootstepManager.Instance.AddScorch(__instance.transform.position, vector.normalized, new Vector3(num, num, num), true);
                UnityEngine.Object.FindObjectOfType<MapMetaDataExporter>().mapMetaData.PaintTerrainMask(__instance.transform.position, __instance.explodeBuildingSettings.radius, __instance.explodeBuildingSettings.paintTerrainMask, __instance.explodeBuildingSettings.removeTerrainMask);
                if (!__instance.Combat.IsLoadingFromSave)
                {
                    List<ICombatant> list = new List<ICombatant>();
                    var combatants = __instance.Combat.GetAllLivingCombatants();
                    for (int i = 0; i < combatants.Count; i++)
                    {
                        if (Vector3.Distance(__instance.Position, combatants[i].CurrentPosition) <
                            __instance.explodeBuildingSettings.radius)
                        {
                            list.Add(combatants[i]);
                        }
                    }
                    List<Vector3> list2 = new List<Vector3>();
                    list2.Add(__instance.transform.position);
                    TerrainMaskFlags applyDesignMaskOnExplosion = __instance.explodeBuildingSettings.applyDesignMaskImmediately ? __instance.explodeBuildingSettings.paintTerrainMask : TerrainMaskFlags.None;
                    ArtilleryObjectiveSequence sequence = new ArtilleryObjectiveSequence(__instance.Combat, list2, __instance.explodeBuildingSettings.artilleryVFXType, list, (float)__instance.explodeBuildingSettings.damage, __instance.explodeBuildingSettings.heatDamage, __instance.explodeBuildingSettings.stabilityDamage, applyDesignMaskOnExplosion);
                    __instance.Combat.MessageCenter.PublishMessage(new AddSequenceToStackMessage(sequence));
                }
                __instance.StartCoroutine(__instance.SpawnExplodeEffectOnCells(__instance.transform.position, __instance.explodeBuildingSettings.radius, __instance.explodeBuildingSettings.paintTerrainVFXName, __instance.explodeBuildingSettings.paintTerrainMask, __instance.explodeBuildingSettings.removeTerrainMask));
            }

            __runOriginal = false;
        }
    }
}