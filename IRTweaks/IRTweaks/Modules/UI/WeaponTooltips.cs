#if NO_CAC
#else

using BattleTech.UI.Tooltips;
using CustAmmoCategories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IRTweaks.Modules.UI
{

    // Update the tooltip to distinguish between different weapon types, and make CAC mannerisms clearer
    [HarmonyPatch(typeof(TooltipPrefab_Weapon), "SetData")]
    static class WeaponTooltips_TooltipPrefab_Weapon_SetData
    {
        static bool Prepare() => Mod.Config.Fixes.WeaponTooltip;

        static void Postfix(TooltipPrefab_Weapon __instance, object data,
            TextMeshProUGUI ___rangeType, TextMeshProUGUI ___damage)
        {
            Mod.Log.Debug?.Write("TP_W:SD entered.");

            ___rangeType.enabled = false;
            Transform rangeLabelT = ___rangeType.gameObject.transform.parent;
            rangeLabelT.gameObject.SetActive(false);

            Transform layoutT = rangeLabelT.gameObject.transform.parent;
            RectTransform rLayoutT = layoutT.gameObject.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rLayoutT);

            WeaponDef weaponDef = (WeaponDef)data;
            if (weaponDef != null)
            {
                Mod.Log.Debug?.Write($"  Updating tooltip for weapon: {weaponDef.Description.UIName}");
                if (CustomAmmoCategories.isRegistredWeapon(weaponDef.Description.Id))
                {
                    // Is a CAC weapon, use HasShells
                    ExtWeaponDef extDef = CustomAmmoCategories.getExtWeaponDef(weaponDef.Description.Id);
                    Mod.Log.Debug?.Write($" Found CAC extension for weapon.");
                    if (extDef == null)
                    {
                        Mod.Log.Warn?.Write($"Failed to load CAC ExtWeaponDef for {weaponDef.Description.Id}, but it was registered!");
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

                    /*
                        PER HARKONNEN HALLOWED BY THINE NAME
                        If ImprovedBallistic = false OR
                           ImprovedBallistic = true AND (HasShells = true or BallisticDamageperPellet = false) - > damage x shots
                        If ImprovedBallistic = true and BallisticDamageperPellet = true and DamageNotDivided = true > damage x projectiles x shots
                        If ImprovedBallistic = true and BallisticDamageperPellet = true and DamageNotDivided = false > (damage/projectiles) x  projectiles x shots
                    */

                    if (extDef.ImprovedBallistic)
                    {
                        // Damage is damage * shotsWhenFired (each shot = volley of projectiles, projectiles are visual only)
                        float totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired;
                        if (weaponDef.ShotsWhenFired != 1)
                        {
                            string localText = $"{weaponDef.Damage} x {weaponDef.ShotsWhenFired} = {totalDamage}";
                            Mod.Log.Debug?.Write($"ImprovedBallistic weapon damage set to: {localText}<page>");
                            ___damage.SetText(localText);
                        }

                        if (extDef.HasShells == TripleBoolean.True || extDef.BallisticDamagePerPallet != TripleBoolean.True)
                        {
                            if (weaponDef.ShotsWhenFired != 1)
                            {
                                // damage x shots = total
                                totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired;
                                string localText = $"{weaponDef.Damage} x {weaponDef.ShotsWhenFired} = {totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + HasShells || !BallisticDamagePerPallet weapon damage set to: {localText}; ShotsWhenFired != 1");
                                ___damage.SetText(localText);
                            }
                            else
                            {
                                totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired;
                                string localText = $"{totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + HasShells || !BallisticDamagePerPallet weapon damage set to: {localText}; ShotsWhenFired == 1");
                                ___damage.SetText(localText);
                            }

                        }

                        if (extDef.BallisticDamagePerPallet == TripleBoolean.True && extDef.DamageNotDivided == TripleBoolean.True)
                        {
                            if (weaponDef.ShotsWhenFired != 1 && weaponDef.ProjectilesPerShot != 1)
                            {
                                totalDamage = weaponDef.Damage * weaponDef.ProjectilesPerShot * weaponDef.ShotsWhenFired;
                                string localText = $"{weaponDef.Damage} x {weaponDef.ProjectilesPerShot} x {weaponDef.ShotsWhenFired} = {totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + BallisticDamagePerPallet + DamageNotDivided weapon damage set to: {localText}; ProjectilesPerShot != 1 AND ShotsWhenFired != 1");
                                ___damage.SetText(localText);
                            }
                            else if (weaponDef.ShotsWhenFired == 1 && weaponDef.ProjectilesPerShot != 1)
                            {
                                totalDamage = weaponDef.Damage * weaponDef.ProjectilesPerShot * weaponDef.ShotsWhenFired;
                                string localText = $"{weaponDef.Damage} x {weaponDef.ProjectilesPerShot}= {totalDamage}";
                                Mod.Log.Debug?.Write(
                                    $"ImprovedBallistic + BallisticDamagePerPallet + DamageNotDivided weapon damage set to: {localText}; ProjectilesPerShot != 1 BUT ShotsWhenFired == 1");
                                ___damage.SetText(localText);
                            }
                            else if (weaponDef.ShotsWhenFired != 1 && weaponDef.ProjectilesPerShot == 1)
                            {
                                totalDamage = weaponDef.Damage * weaponDef.ProjectilesPerShot * weaponDef.ShotsWhenFired;
                                string localText = $"{weaponDef.Damage} x {weaponDef.ShotsWhenFired} = {totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + BallisticDamagePerPallet + DamageNotDivided weapon damage set to: {localText}; ProjectilesPerShot == 1 BUT ShotsWhenFired != 1");
                                ___damage.SetText(localText);
                            }
                            else
                            {
                                totalDamage = weaponDef.Damage * weaponDef.ProjectilesPerShot * weaponDef.ShotsWhenFired;
                                string localText = $"{totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + HasShells || !BallisticDamagePerPallet weapon damage set to: {localText}; ShotsWhenFired AND ProjectilesPerShot == 1");
                                ___damage.SetText(localText);
                            }

                        }

                        if (extDef.BallisticDamagePerPallet == TripleBoolean.True && extDef.DamageNotDivided != TripleBoolean.True)
                        {
                            if (weaponDef.ShotsWhenFired != 1 && weaponDef.ProjectilesPerShot != 1)
                            {
                                float damagePerPellet = weaponDef.Damage / weaponDef.ProjectilesPerShot;
                                totalDamage = damagePerPellet * weaponDef.ShotsWhenFired * weaponDef.ProjectilesPerShot;
                                string localText = $"{damagePerPellet} x {weaponDef.ShotsWhenFired} x {weaponDef.ProjectilesPerShot} = {totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + BallisticDamagePerPallet + !DamageNotDivided weapon damage set to: {localText}; ProjectilesPerShot != 1 AND ShotsWhenFired != 1");
                                ___damage.SetText(localText);
                            }
                            else if (weaponDef.ShotsWhenFired == 1 && weaponDef.ProjectilesPerShot != 1)
                            {
                                float damagePerPellet = weaponDef.Damage / weaponDef.ProjectilesPerShot;
                                totalDamage = damagePerPellet * weaponDef.ShotsWhenFired * weaponDef.ProjectilesPerShot;
                                string localText = $"{damagePerPellet} x {weaponDef.ProjectilesPerShot} = {totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + BallisticDamagePerPallet + !DamageNotDivided weapon damage set to: {localText}; ProjectilesPerShot != 1 BUT ShotsWhenFired == 1");
                                ___damage.SetText(localText);
                            }
                            else if (weaponDef.ShotsWhenFired != 1 && weaponDef.ProjectilesPerShot == 1)
                            {
                                float damagePerPellet = weaponDef.Damage / weaponDef.ProjectilesPerShot;
                                totalDamage = damagePerPellet * weaponDef.ShotsWhenFired * weaponDef.ProjectilesPerShot;
                                string localText = $"{damagePerPellet} x {weaponDef.ShotsWhenFired} = {totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + BallisticDamagePerPallet + !DamageNotDivided weapon damage set to: {localText}; ProjectilesPerShot == 1 BUT ShotsWhenFired != 1");
                                ___damage.SetText(localText);
                            }
                            else
                            {
                                float damagePerPellet = weaponDef.Damage / weaponDef.ProjectilesPerShot;
                                totalDamage = damagePerPellet * weaponDef.ShotsWhenFired * weaponDef.ProjectilesPerShot;
                                string localText = $"{totalDamage}";
                                Mod.Log.Debug?.Write($"ImprovedBallistic + BallisticDamagePerPallet + !DamageNotDivided weapon damage set to: {localText}; ProjectilesPerShot == 1 AND ShotsWhenFired == 1");
                                ___damage.SetText(localText);
                            }

                        }

                    }
                    else
                    {
                        // Vanilla calculations, so ProjectilesPerShot is visual only. See https://github.com/BattletechModders/IRTweaks/issues/8
                        float totalDamage = weaponDef.Damage * weaponDef.ShotsWhenFired;
                        if (weaponDef.ShotsWhenFired != 1)
                        {
                            string localText = $"{weaponDef.Damage} x {weaponDef.ShotsWhenFired} shots = {totalDamage}";
                            Mod.Log.Debug?.Write($"Vanilla + ShotsWhenFire > 1 weapon damage set to: {localText}");
                            ___damage.SetText(localText);
                        }
                        else
                        {
                            string localText = $"{weaponDef.Damage}";
                            Mod.Log.Debug?.Write($"Vanilla + 1 ShotsWhenFired weapon damage set to: {localText}");
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
                        Mod.Log.Debug?.Write($"Vanilla + ShotsWhenFire > 1 weapon damage set to: {localText}");
                        ___damage.SetText(localText);
                    }
                    else
                    {
                        string localText = $"{weaponDef.Damage}";
                        Mod.Log.Debug?.Write($"Vanilla + 1 ShotsWhenFired weapon damage set to: {localText}");
                        ___damage.SetText(localText);
                    }
                }

            }


        }
    }
}

#endif