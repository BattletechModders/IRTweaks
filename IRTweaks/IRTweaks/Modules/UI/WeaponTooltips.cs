using BattleTech;
using BattleTech.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IRTweaks.Modules.UI {
    public static class WeaponTooltips {
        public static void TooltipPrefab_Weapon_SetData_Postfix(TooltipPrefab_Weapon __instance, object data, TextMeshProUGUI ___rangeType, TextMeshProUGUI ___damage) {
            Mod.Log.Debug("TP_W:SD entered.");

            ___rangeType.enabled = false;
            Transform rangeLabelT = ___rangeType.gameObject.transform.parent;
            rangeLabelT.gameObject.SetActive(false);

            Transform layoutT = rangeLabelT.gameObject.transform.parent;
            RectTransform rLayoutT = layoutT.gameObject.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rLayoutT);

            Mod.Log.Debug("  Updating weapon text.");
            WeaponDef weaponDef = (WeaponDef)data;
            if (weaponDef != null) {
                float totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired * weaponDef.ProjectilesPerShot;
                if (weaponDef.ShotsWhenFired != 1 && weaponDef.ProjectilesPerShot != 1) {
                    ___damage.SetText($"{weaponDef.Damage.ToString()} x {weaponDef.ShotsWhenFired.ToString()} x {weaponDef.ProjectilesPerShot.ToString()} = {totalDamage}");
                } else if (weaponDef.ShotsWhenFired != 1) {
                    ___damage.SetText($"{weaponDef.Damage.ToString()} x {weaponDef.ShotsWhenFired.ToString()} = {totalDamage}");
                } else if (weaponDef.ProjectilesPerShot != 1 && !Mod.Config.Fixes.OneShotDamagePerPallet) {
                    ___damage.SetText($"{weaponDef.Damage.ToString()} x {weaponDef.ProjectilesPerShot.ToString()} = {totalDamage}");
                } else if (weaponDef.ShotsWhenFired == 1 && weaponDef.ProjectilesPerShot != 1 && Mod.Config.Fixes.OneShotDamagePerPallet) {
                    ___damage.SetText($"{(weaponDef.Damage / weaponDef.ProjectilesPerShot).ToString()} x {weaponDef.ProjectilesPerShot.ToString()} shots = {weaponDef.Damage}");
                }  else {
                    ___damage.SetText($"{weaponDef.Damage.ToString()}");
                }
            }
        }
    }
}
