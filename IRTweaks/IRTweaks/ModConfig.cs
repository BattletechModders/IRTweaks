using BattleTech;
using System.Collections.Generic;
using System.Linq;

namespace IRTweaks
{

    public class AbilityOpts
    {
        public string FlexibleSensorLockId = "AbilityDefT8A";
        public string JuggernautId = "AbilityDefGu8";
        public string MultiTargetId = "AbilityDefG5";

        public BraceEffects BraceEffectConfig = new BraceEffects();
    }

    public class StoreOpts
    {
        public int QuantityOnShift = 5;
        public int QuantityOnControl = 20;
    }

    public class CheatOpts
    {
        public bool CheatDetection = false;
        public bool CheatDetectionNotify = false;
        public string CheatDetectionStat = "CheatFound1";
    }

    public class MiscOpts
    {
        public DifficultyUIScalingOpts DifficultyUIScaling = new DifficultyUIScalingOpts();
        public class DifficultyUIScalingOpts
        {
            public float StartOnlyScalar = 40f;
            public float StartOnlyPositionY = 40f;
            public float RegularPositionY = 40f;
        }

        public MechLabRefitReqOpts MechLabRefitReqs = new MechLabRefitReqOpts();
        public class MechLabRefitReqOpts
        {
            public Dictionary<string, string> MechLabRefitReqs = new Dictionary<string, string>();
        }
    }

    public class CombatOpts
    {
        public int PilotAttributesMax = 13;
        public List<string> DisableCTMaxInjureTags = new List<string>();
        public string TorsoMountStatName = "";
        public CalledShotOpts CalledShot = new CalledShotOpts();
        public class CalledShotOpts
        {

            public bool DisableAllLocations = true;
            public bool DisableHeadshots = true;
            public bool EnableTacticsModifier = true;

            public int BaseModifier = 6;
            public Dictionary<string, int> PilotTags = new Dictionary<string, int>();
        }

        public ToHitStatModOpts ToHitStatMods = new ToHitStatModOpts();

        public class ToHitStatModOpts
        {
            public List<ToHitMod> WeaponToHitMods = new List<ToHitMod>();
            public List<ToHitMod> ActorToHitMods = new List<ToHitMod>();
        }

        public DamageModsBySkillOpts DamageModsBySkill = new DamageModsBySkillOpts();

        public class DamageModsBySkillOpts
        {
            public bool DisplayFloatiesOnTrigger = false;
            public List<StabilityMod> StabilityMods = new List<StabilityMod>();
            public List<HeatMod> HeatMods = new List<HeatMod>();
            public List<APDmgMod> APDmgMods = new List<APDmgMod>();
            public List<StdDmgMod> StdDmgMods = new List<StdDmgMod>();

            //           public List<Tuple<string, float, float>> StabilityMod = new List<Tuple<string, float, float>>();
            //           public List<Tuple<string, float, float>> HeatMod = new List<Tuple<string, float, float>>();
            //           public List<Tuple<string, float, float>> APDmgMod = new List<Tuple<string, float, float>>();
            //           public List<Tuple<string, float, float>> StdDmgMod = new List<Tuple<string, float, float>>();
        }

        public FlexibleSensorLockOptions FlexibleSensorLock = new FlexibleSensorLockOptions();
        public class FlexibleSensorLockOptions
        {
            public bool FreeActionWithAbility = false;
            public bool FreeActionWithStat = false;
            public string FreeActionStatName = "IR_FreeSensorLock";
            public bool AlsoAppliesToActiveProbe = false;
        }

        public ObstructionTweakOpts ObstructionTweaks = new ObstructionTweakOpts();
        public class ObstructionTweakOpts
        {
            public List<ArmorLocation> DRMechLocs = new List<ArmorLocation>();
            public List<VehicleChassisLocations> DRVehicleLocs = new List<VehicleChassisLocations>();
            public List<string> QuadTags = new List<string>();
            public Dictionary<string, float> ObstructionDRByTags = new Dictionary<string, float>(); 

        }

        public OnWeaponFireSpecialEffectOpts OnWeaponFireOpts = new OnWeaponFireSpecialEffectOpts();
        public class OnWeaponFireSpecialEffectOpts
        {
            public string SelfKnockdownCheckStatName = ""; // stat NAME will be this", should be float, and is base chance of resisting self-knockdown
            public float SelfKnockdownPilotingFactor = 0f;
            public float SelfKnockdownBracedFactor = 0f;
            public float SelfKnockdownTonnageFactor = 0f;
            public float SelfKnockdownTonnageBonusThreshold = 100f; //units less than this use linear formula only
            public float SelfKnockdownTonnageBonusFactor = 1f; //z of ln((tonnage/divisor)*z)
            public string IgnoreSelfKnockdownTag = "";

            public string SelfInstabilityStatName = ""; // stat NAME will be this", should be float, and is base chance of resisting self-knockdown
            public float SelfInstabilityPilotingFactor = 0f;
            public float SelfInstabilityBracedFactor = 0f;
            public float SelfInstabilityTonnageFactor = 0f;
            public float SelfInstabilityTonnageBonusThreshold = 100f; //units less than this use linear formula only
            public float SelfInstabilityTonnageBonusFactor = 1f; //z of ln((tonnage/divisor)*z)
            public string IgnoreSelfInstabilityTag = "";
        }

        public OnWeaponHitSpecialEffectOpts OnWeaponHitOpts = new OnWeaponHitSpecialEffectOpts();
        public class OnWeaponHitSpecialEffectOpts
        {
            public string ForceShutdownOnHitStat = ""; // stat NAME will be this", should be float, and is base chance of resisting self-knockdown
            public float ResistShutdownGutsFactor = 0f;
            public string IgnoreShutdownTag = "";
        }

        public PainToleranceOpts PainTolerance = new PainToleranceOpts();
        public class PainToleranceOpts
        {
            public float ResistPerGuts = 10.0f;

            public float HeadDamageResistPenaltyPerArmorPoint = 5.0f;
            public float HeadHitArmorOnlyResistPenaltyMulti = 0.5f;

            // Reduces resist by this multiplied the capacity ratio of an ammo explosion
            public float AmmoExplosionResistPenaltyPerCapacityPercentile = 1.0f;
            // Reduces resist by this multiplied the capacity ratio of an head damage injury
            public float OverheatResistPenaltyPerHeatPercentile = 1.0f;

            public float KnockdownResistPenalty = 6f;
            public float SideLocationDestroyedResistPenalty = 10f;
            public float CTLocationDestroyedResistPenalty = 10f;

        }

        public ScaledStructureOpts ScaledStructure = new ScaledStructureOpts();
        public class ScaledStructureOpts
        {
            public Dictionary<string, StructureScale> ContractScaling = new Dictionary<string, StructureScale>();
            public Dictionary<int, StructureScale> DifficultyScaling = new Dictionary<int, StructureScale>()
            {
                {  1, new StructureScale() { Mod = 0, Multi = 1f } },
                {  2, new StructureScale() { Mod = 0, Multi = 1.25f } },
                {  3, new StructureScale() { Mod = 0, Multi = 1.5f } },
                {  4, new StructureScale() { Mod = 0, Multi = 2f } },
                {  5, new StructureScale() { Mod = 0, Multi = 2.5f } },
                {  6, new StructureScale() { Mod = 0, Multi = 3f } },
                {  7, new StructureScale() { Mod = 0, Multi = 3.5f } },
                {  8, new StructureScale() { Mod = 0, Multi = 4f } },
                {  9, new StructureScale() { Mod = 0, Multi = 4.5f } },
                { 10, new StructureScale() { Mod = 0, Multi = 5f } }
            };

            public StructureScale DefaultScale = new StructureScale() { Mod = 0, Multi = 1f };
            public StructureScale StoryScale = new StructureScale() { Mod = 0, Multi = 1f };
            public bool UseStoryScale = false;

            public int MinDifficulty = 1;
            public int MaxDifficulty = 10;
        }

        public class StructureScale
        {
            public int Mod = 0;
            public float Multi = 1f;
        }

        public SpawnProtectionOpts SpawnProtection = new SpawnProtectionOpts();
        public class SpawnProtectionOpts
        {
            public bool ApplyGuard = true;

            public int EvasionPips = 6;

            public bool ApplyToEnemies = true;
            public bool ApplyToAllies = true;
            public bool ApplyToNeutrals = true;

            public bool ApplyToReinforcements = false;
        }

        public TurretArmorAndStructureOpts TurretArmorAndStructure = new TurretArmorAndStructureOpts();

        public class TurretArmorAndStructureOpts
        {
            public float StructureMultiplierTurret;
            public float ArmorMultiplierTurret;
        }
    }

    public class FixesFlags
    {

        // Combat
        public bool AbilityResourceFix = true;
        public bool OnWeaponFireFix = true;
        public bool AlternateMechNamingStyle = true;
        public bool BuildingDamageColorChange = true;
        public bool BreachingShotIgnoresAllDR = false;
        public bool BraceOnMeleeWithJuggernaut = true;
        public bool CalledShotTweaks = true;
        public bool CTDestructInjuryFix = true;
        public bool ExplodingBuildingFix = true;
        public bool ExtendedStats = true;
        public bool FlexibleSensorLock = true;
        public bool PainTolerance = true;
        public bool PathfinderTeamFix = true;
        public bool ScaleObjectiveBuildingStructure = true;
        public bool SpawnProtection = true;
        public bool TgtComputerTonnageDisplay = true;
        public bool TurnDirectorStartFirstRoundFix = true;
        public bool UrbanExplosionsFix = true;
        public bool WeakAgainstMeleeFix = true;

        // Misc

        public bool DifficultyModsFromStats = true;
        public bool DisableCampaign = true;
        public bool DisableDebug = true;
        public bool DisableLowFundsNotification = true;
        public bool DisableMPHashCalculation = true;
        public bool EventRequirementsScopeFix = true;
        public bool FactionValueFix = true;
        public bool MultiTargetStat = true;
        public bool RandomStartByDifficulty = true;
        public bool ReduceSaveCompression = true;
        public bool RestoreMechTagsOnReady = true;
        public bool ShowAllArgoUpgrades = true;
        public bool SkipDeleteSavePopup = true;
        public bool SkirmishReset = false;
        public bool DeathChanceStat = true;

        // UI
        public bool BulkPurchasing = true;
        public bool BulkScrapping = true;
        public bool CombatLog = true;
        public bool CombatLogNameModifiers = true;
        public bool DisableCombatRestarts = true;
        public bool DisableCombatSaves = true;
        public bool MaxArmorMaxesArmor = false;
        public bool MechbayLayout = true;
        public bool MechbayLayoutDisableStore = true;
        public bool MechbayAdvancedStripping = true;
        public bool SkirmishAlwaysUnlimited = true;
        public bool SimGameDifficultyLabelsReplacer = true;
        public bool StreamlinedMainMenu = true;
        public bool WeaponTooltip = true;
        public bool DamageReductionInCombatHud = false;

    }

    public class ModConfig
    {

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;

        public FixesFlags Fixes = new FixesFlags();

        public AbilityOpts Abilities = new AbilityOpts();
        public CheatOpts CheatDetection = new CheatOpts();
        public MiscOpts Misc = new MiscOpts();
        public CombatOpts Combat = new CombatOpts();
        public StoreOpts Store = new StoreOpts();

        public void LogConfig()
        {
            Mod.Log.Info?.Write("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info?.Write($"  DEBUG: {this.Debug} Trace: {this.Trace}");

            Mod.Log.Info?.Write("  -- Fixes --");
            Mod.Log.Info?.Write($"  AbilityResourceFix:                 {this.Fixes.AbilityResourceFix}");
            Mod.Log.Info?.Write($"  AlternateMechNamingStyle:           {this.Fixes.AlternateMechNamingStyle}");
            Mod.Log.Info?.Write($"  BuildingDamageColorChange:          {this.Fixes.BuildingDamageColorChange}");
            Mod.Log.Info?.Write($"  BraceOnMeleeWithJuggernaut:         {this.Fixes.BraceOnMeleeWithJuggernaut}");
            Mod.Log.Info?.Write($"  BreachingShotIgnoresAllDR:          {this.Fixes.BreachingShotIgnoresAllDR}");
            Mod.Log.Info?.Write($"  BulkPurchasing:                     {this.Fixes.BulkPurchasing}");
            Mod.Log.Info?.Write($"  BulkScrapping:                      {this.Fixes.BulkScrapping}");
            Mod.Log.Info?.Write($"  CalledShotTweaks:                   {this.Fixes.CalledShotTweaks}");
            Mod.Log.Info?.Write($"  CombatLog:                          {this.Fixes.CombatLog}");
            Mod.Log.Info?.Write($"  DifficultyModsFromStats:            {this.Fixes.DifficultyModsFromStats}");
            Mod.Log.Info?.Write($"  DisableCampaign:                    {this.Fixes.DisableCampaign}");
            Mod.Log.Info?.Write($"  DisableDebug:                       {this.Fixes.DisableDebug}");
            Mod.Log.Info?.Write($"  DisableCombatRestarts:              {this.Fixes.DisableCombatRestarts}");
            Mod.Log.Info?.Write($"  DisableCombatSaves:                 {this.Fixes.DisableCombatSaves}");
            Mod.Log.Info?.Write($"  DisableMPHashCalculation:           {this.Fixes.DisableMPHashCalculation}");
            Mod.Log.Info?.Write($"  DisableLowFundsNotification:        {this.Fixes.DisableLowFundsNotification}");
            Mod.Log.Info?.Write($"  ExplodingBuildingFix:               {this.Fixes.ExplodingBuildingFix}");
            Mod.Log.Info?.Write($"  EventRequirementsScopeFix:          {this.Fixes.EventRequirementsScopeFix}");
            Mod.Log.Info?.Write($"  ExtendedStats:                      {this.Fixes.ExtendedStats}");
            Mod.Log.Info?.Write($"  FlexibleSensorLock:                 {this.Fixes.FlexibleSensorLock}");
            Mod.Log.Info?.Write($"  MaxArmorMaxesArmor:                 {this.Fixes.MaxArmorMaxesArmor}");
            Mod.Log.Info?.Write($"  MechbayLayout:                      {this.Fixes.MechbayLayout}");
            Mod.Log.Info?.Write($"  MechbayLayoutDisableStore:          {this.Fixes.MechbayLayoutDisableStore}");
            Mod.Log.Info?.Write($"  OnWeaponFireFix:                    {this.Fixes.OnWeaponFireFix}");
            Mod.Log.Info?.Write($"  PainTolerance:                      {this.Fixes.PainTolerance}");
            Mod.Log.Info?.Write($"  PathfinderTeamFix:                  {this.Fixes.PathfinderTeamFix}");
            Mod.Log.Info?.Write($"  RandomStartByDifficulty:            {this.Fixes.RandomStartByDifficulty}");
            Mod.Log.Info?.Write($"  ReduceSaveCompression:              {this.Fixes.ReduceSaveCompression}");
            Mod.Log.Info?.Write($"  ScaleObjectiveBuildingStructure:    {this.Fixes.ScaleObjectiveBuildingStructure}");
            Mod.Log.Info?.Write($"  ShowAllArgoUpgrades:                {this.Fixes.ShowAllArgoUpgrades}");
            Mod.Log.Info?.Write($"  SkipDeleteSavePopup:                {this.Fixes.SkipDeleteSavePopup}");
            Mod.Log.Info?.Write($"  SkirmishAlwaysUnlimited:            {this.Fixes.SkirmishAlwaysUnlimited}");
            Mod.Log.Info?.Write($"  SkirmishReset:                      {this.Fixes.SkirmishReset}");
            Mod.Log.Info?.Write($"  SimGameDifficultyLabelsReplacer:    {this.Fixes.SimGameDifficultyLabelsReplacer}");
            Mod.Log.Info?.Write($"  SpawnProtection:                    {this.Fixes.SpawnProtection}");
            Mod.Log.Info?.Write($"  StreamlinedMainMenu:                {this.Fixes.StreamlinedMainMenu}");
            Mod.Log.Info?.Write($"  TurnDirectorStartFirstRoundFix:     {this.Fixes.TurnDirectorStartFirstRoundFix}");
            Mod.Log.Info?.Write($"  TgtComputerTonnageDisplay:          {this.Fixes.TgtComputerTonnageDisplay}");
            Mod.Log.Info?.Write($"  UrbanExplosionsFix:                 {this.Fixes.UrbanExplosionsFix}");
            Mod.Log.Info?.Write($"  WeaponTooltips:                     {this.Fixes.WeaponTooltip}");
            Mod.Log.Info?.Write($"  DamageReductionInCombatHud:         {this.Fixes.DamageReductionInCombatHud}");
            Mod.Log.Info?.Write($"  DeathChanceStat:                    {this.Fixes.DeathChanceStat}");

            Mod.Log.Info?.Write($"  DisableCTMaxInjureTags:                     {this.Combat.DisableCTMaxInjureTags}");
            Mod.Log.Info?.Write($"  TorsoMountStatName:                     {this.Combat.TorsoMountStatName}");
            Mod.Log.Info?.Write("  -- Called Shot --");
            Mod.Log.Info?.Write($"   Disable => AllLocations: {Combat.CalledShot.DisableAllLocations}  Headshots: {Combat.CalledShot.DisableHeadshots}");
            Mod.Log.Info?.Write($"   Enable => ComplexTacticsModifier: {Combat.CalledShot.EnableTacticsModifier}");
            Mod.Log.Info?.Write($"   BaseModifier:{Combat.CalledShot.BaseModifier}");
            foreach (KeyValuePair<string, int> kvp in Combat.CalledShot.PilotTags)
            {
                Mod.Log.Info?.Write($"   CalledShotPilotModifier - tag:{kvp.Key} modifier:{kvp.Value}");
            }
            Mod.Log.Info?.Write($"   CalledShotDefaultMod:{Combat.CalledShot.BaseModifier}");

            Mod.Log.Info?.Write("  -- Spawn Protection --");
            Mod.Log.Info?.Write($"   ApplyGuard:{Combat.SpawnProtection.ApplyGuard}  EvasionPips:{Combat.SpawnProtection.EvasionPips}");
            Mod.Log.Info?.Write($"   ApplyToEnemies:{Combat.SpawnProtection.ApplyToEnemies}  ApplyToAllies:{Combat.SpawnProtection.ApplyToAllies}  ApplyToNeutrals:{Combat.SpawnProtection.ApplyToNeutrals}  ");
            Mod.Log.Info?.Write($"   ApplyToReinforcements:{Combat.SpawnProtection.ApplyToReinforcements}");

            Mod.Log.Info?.Write("  -- Cheat Detection --");
            Mod.Log.Info?.Write($"   CheatDetection:{CheatDetection.CheatDetection}");
            Mod.Log.Info?.Write($"   CheatDetectionNotify:{CheatDetection.CheatDetectionNotify}");
            Mod.Log.Info?.Write($"   CheatDetectionStat:{CheatDetection.CheatDetectionStat}");

            Mod.Log.Info?.Write("  -- ObstructionTweaks --");
            Mod.Log.Info?.Write($"   DRMechLocs:{Combat.ObstructionTweaks.DRMechLocs}");
            Mod.Log.Info?.Write($"   DRVehicleLocs:{Combat.ObstructionTweaks.DRVehicleLocs}");
            Mod.Log.Info?.Write($"   QuadTags:{Combat.ObstructionTweaks.QuadTags}");
            Mod.Log.Info?.Write($"   ObstructionDRByTags:{Combat.ObstructionTweaks.ObstructionDRByTags}");

            Mod.Log.Info?.Write("  -- Store --");
            Mod.Log.Info?.Write($"   QuantityOnShift:{Store.QuantityOnShift}  QuantityOnControl:{Store.QuantityOnControl}");

            Mod.Log.Info?.Write("  -- Flexible Sensor Lock Options --");
            Mod.Log.Info?.Write($"   FreeActionWithAbility:{this.Combat.FlexibleSensorLock.FreeActionWithAbility}  AbilityId:{this.Abilities.FlexibleSensorLockId}");

            Mod.Log.Info?.Write("=== MOD CONFIG END ===");
        }

        public void Init()
        {

            Mod.Log.Info?.Write("== Mod Config Initialization Started == ");

            // Iterate scaled structure, setting max and min
            if (Mod.Config.Fixes.ScaleObjectiveBuildingStructure)
            {
                if (Combat.ScaledStructure.DifficultyScaling.Keys.Count <= 0)
                    Mod.Log.Warn?.Write($"ScaleObjectiveBuildingStructure enabled but configuration is empty!");

                if (Combat.ScaledStructure.DifficultyScaling.Keys.Count > 0)
                {
                    List<int> keys = Combat.ScaledStructure.DifficultyScaling.Keys.ToList<int>();
                    keys.Sort();
                    Combat.ScaledStructure.MinDifficulty = keys[0];
                    Combat.ScaledStructure.MaxDifficulty = keys[keys.Count - 1];
                    Mod.Log.Info?.Write($"ScaleObjectiveBuildingStructure has difficulties between {Combat.ScaledStructure.MinDifficulty} and {Combat.ScaledStructure.MaxDifficulty}");
                }

            }

            Mod.Log.Info?.Write("== Mod Config Initialization Complete == ");
        }
    }
}
