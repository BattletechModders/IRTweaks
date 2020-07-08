using BattleTech;
using BattleTech.UI.Tooltips;
using CustAmmoCategories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IRTweaks.Modules.UI {

    // Update the tooltip to distinguish between different weapon types, and make CAC mannerisms clearer
    public static class WeaponTooltips {
        public static void TooltipPrefab_Weapon_SetData_Postfix(TooltipPrefab_Weapon __instance, object data, 
            TextMeshProUGUI ___rangeType, TextMeshProUGUI ___damage) {
            Mod.Log.Debug("TP_W:SD entered.");

            ___rangeType.enabled = false;
            Transform rangeLabelT = ___rangeType.gameObject.transform.parent;
            rangeLabelT.gameObject.SetActive(false);

            Transform layoutT = rangeLabelT.gameObject.transform.parent;
            RectTransform rLayoutT = layoutT.gameObject.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rLayoutT);

            WeaponDef weaponDef = (WeaponDef)data;
            if (weaponDef != null)
            {
                Mod.Log.Debug($"  Updating tooltip for weapon: {weaponDef.Description.UIName}");
                if (CustomAmmoCategories.isRegistredWeapon(weaponDef.Description.Id))
                {
                    // Is a CAC weapon, use HasShells
                    ExtWeaponDef extDef = CustomAmmoCategories.getExtWeaponDef(weaponDef.Description.Id);
                    Mod.Log.Debug($" Found CAC extension for weapon.");
                    if (extDef == null)
                    {
                        Mod.Log.Warn($"Failed to load CAC ExtWeaponDef for {weaponDef.Description.Id}, but it was registered!");
                        return;
                    }

                    /*
                     "ImprovedBallistic": true, - whether use or not own ballistic weapon effect engine. 
                        Difference between "improved" and vanilla engine:
                        1. Improved mode uses ShotsWhenFire properly (vanilla had not used them at all)                        
                        3. Improved mode fire ShotsWhenFire volleys with ProjectilesPerShot bullets in each. 
                            Bullets in one volley fired simultaneously instead of one by one (as in WeaponRealizer)
                            But damage still dealt once per volley, not per bullet, to keep compatibility with vanilla.
                        NOTE! If ImprovedBallistic is set DisableClustering is forced to true and "wr-clustered_shots" tag removed from definition. 

                        "BallisticDamagePerPallet": true - if true damage inflicted per pallet instead of per shot. 
                        Only working with ImprovedBallistic true, ballistic weapon effect and HasShels false
                                    Damage will be divided by ProjectilesPerShot value, heat damage and stable damage too. 
                    */
                    if (extDef.ImprovedBallistic)
                    {
                        // Damage is damage * shotsWhenFired (each shot = volley of projectiles, projectiles are visual only)
                        float totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired;
                        if (weaponDef.ShotsWhenFired != 1)
                        {
                            string localText = $"{weaponDef.Damage} x {weaponDef.ShotsWhenFired} = {totalDamage}";
                            Mod.Log.Debug($"ImprovedBallistic weapon damage set to: {localText}<page>");
                            ___damage.SetText(localText);
                        }

                        /*
                            "BallisticDamagePerPallet": true - if true damage inflicted per pallet instead of per shot. 
                            Only working with ImprovedBallistic true, ballistic weapon effect and HasShels false
                                        Damage will be divided by ProjectilesPerShot value, heat damage and stable damage too. 
                        */
                        if (extDef.HasShells != TripleBoolean.True && extDef.BallisticDamagePerPallet == TripleBoolean.True)
                        {
                            // Damage is per pellet, instead of per shot. Heat, stability, damage all divided by projectiles per shot
                            //   so: (damage / projectilesPerShot) x (shotsWhenFired) x (projectilesPerShot) = {totalDamage}
                            float damagePerPellet = weaponDef.Damage / weaponDef.ProjectilesPerShot;
                            totalDamage = damagePerPellet * weaponDef.ShotsWhenFired * weaponDef.ProjectilesPerShot;
                            string localText = $"{damagePerPellet} x {weaponDef.ShotsWhenFired} x {weaponDef.ProjectilesPerShot} = {totalDamage}";
                            Mod.Log.Debug($"ImprovedBallistic + BallisticDamagePerPallet weapon damage set to: {localText}");
                            ___damage.SetText(localText);
                        }

                        /*
                            "HasShells": true/false, if defined determinate has shots shrapnel effect or not. If defined can't be overriden by ammo or mode. 
                            Shells count is effective ProjectilesPerShot for this weapon/ammo/mode.
                            Damage per shell - full damage per projectile / ProjectilesPerShot
                            Only for missiles, ballistic and gauss effects. Should not be used with AoE.
                            NOTE! If ImprovedBallistic is false HasShells considered as false too no matter real value. 
                        */
                        if (extDef.HasShells == TripleBoolean.True)
                        {
                            // Per Harkonnen, sensible thing to do is just damage x shots
                            totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired;
                            string localText = $"{weaponDef.Damage} x {weaponDef.ShotsWhenFired} = {totalDamage}";
                            Mod.Log.Debug($"ImprovedBallistic + HasShells weapon damage set to: {localText}");
                            ___damage.SetText(localText);
                        }

                    }
                    else
                    {
                        // Vanilla calculations, so ProjectilesPerShot is visual only. See https://github.com/BattletechModders/IRTweaks/issues/8
                        float totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired;
                        if (weaponDef.ShotsWhenFired != 1)
                        {
                            string localText = $"{weaponDef.Damage} x {weaponDef.ShotsWhenFired} shots = {totalDamage}";
                            Mod.Log.Debug($"Vanilla + ShotsWhenFire > 1 weapon damage set to: {localText}");
                            ___damage.SetText(localText);
                        }
                        else
                        {
                            string localText = $"{weaponDef.Damage}";
                            Mod.Log.Debug($"Vanilla + 1 ShotsWhenFired weapon damage set to: {localText}");
                            ___damage.SetText(localText);
                        }
                    }

                    /*
                     * "AOEDamage": 0 - if > 0 alternative AoE damage algorithm will be used. Main projectile will not always miss. 
                     *  Instead it will inflict damage twice  one for main target - direct hit (this damage can be have variance) 
                     *  and second for all targets in AoE range including main. 
                     *  
                     *  "AOEHeatDamage": 0 - if > 0 alternative AoE damage algorithm will be used. Main projectile will not always miss. 
                     *  Instead it will inflict damage twice one for main target - direct hit (this damage can be have variance) 
                     *  and second for all targets in AoE range including main. 
                     *  
                     *  "AOEInstability": 0 - instability AoE damage 
                     */

                }
                else
                {
                    // Not a CAC weapon, vanilla, so ProjectilesPerShot is visual only. See https://github.com/BattletechModders/IRTweaks/issues/8
                    float totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired;
                    if (weaponDef.ShotsWhenFired != 1)
                    {
                        string localText = $"{weaponDef.Damage} x {weaponDef.ShotsWhenFired} shots = {totalDamage}";
                        Mod.Log.Debug($"Vanilla + ShotsWhenFire > 1 weapon damage set to: {localText}");
                        ___damage.SetText(localText);
                    }
                    else
                    {
                        string localText = $"{weaponDef.Damage}";
                        Mod.Log.Debug($"Vanilla + 1 ShotsWhenFired weapon damage set to: {localText}");
                        ___damage.SetText(localText);
                    }
                }

            }


        }
    }
}
