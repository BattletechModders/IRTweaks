using IRBTModUtils.Logging;
using IRTweaks.Modules.Combat;
using IRTweaks.Modules.Misc;
using IRTweaks.Modules.Tooltip;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace IRTweaks
{

    public static class Mod
    {

        public const string HarmonyPackage = "us.frostraptor.IRTweaks";
        public const string LogName = "ir_tweaks";

        public static DeferringLogger Log;
        public static string ModDir;
        public static ModConfig Config;
        public static ModText LocalizedText;

        public static readonly Random Random = new Random();

        public static void Init(string modDirectory, string settingsJSON)
        {
            ModDir = modDirectory;

            Exception settingsE = null;
            try
            {
                Mod.Config = JsonConvert.DeserializeObject<ModConfig>(settingsJSON);
            }
            catch (Exception e)
            {
                settingsE = e;
                Mod.Config = new ModConfig();
            }

            Log = new DeferringLogger(modDirectory, LogName, Config.Debug, Config.Trace);

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            Log.Info?.Write($"Assembly version: {fvi.FileVersion}");

            Log.Debug?.Write($"ModDir is:{modDirectory}");
            Log.Debug?.Write($"mod.json settings are:({settingsJSON})");

            // Initialize the mod settings
            Mod.Config.Init();
            Mod.Config.LogConfig();

            if (settingsE != null)
            {
                Log.Info?.Write($"ERROR reading settings file! Error was: {settingsE}");
            }
            else
            {
                Log.Info?.Write($"INFO: No errors reading settings file.");
            }

            // Read localization
            string localizationPath = Path.Combine(ModDir, "./mod_localized_text.json");
            try
            {
                string jsonS = File.ReadAllText(localizationPath);
                Mod.LocalizedText = JsonConvert.DeserializeObject<ModText>(jsonS);
            }
            catch (Exception e)
            {
                Mod.LocalizedText = new ModText();
                Log.Error?.Write(e, $"Failed to read localizations from: {localizationPath} due to error!");
            }

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), HarmonyPackage);

            // Initialize modules
            CombatFixes.InitModule();
            MiscFixes.InitModule();
            UIFixes.InitModule();

            ModState.InitializeEffects();
        }
    }
}
