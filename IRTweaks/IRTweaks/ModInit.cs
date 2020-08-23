using Harmony;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
using IRTweaks.Modules.Combat;
using IRTweaks.Modules.Tooltip;
using IRTweaks.Modules.Misc;
using IRBTModUtils.Logging;

namespace IRTweaks {

    public static class Mod {

        public const string HarmonyPackage = "us.frostraptor.IRTweaks";
        public const string LogName = "ir_tweaks";

        public static DeferringLogger Log;
        public static string ModDir;
        public static ModConfig Config;

        public static readonly Random Random = new Random();

        public static void Init(string modDirectory, string settingsJSON) {
            ModDir = modDirectory;

            Exception settingsE = null;
            try {
                Mod.Config = JsonConvert.DeserializeObject<ModConfig>(settingsJSON);
            } catch (Exception e) {
                settingsE = e;
                Mod.Config = new ModConfig();
            }

            Log = new DeferringLogger(modDirectory, LogName, Config.Debug, Config.Trace);

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            Log.Info?.Write($"Assembly version: {fvi.ProductVersion}");

            Log.Debug?.Write($"ModDir is:{modDirectory}");
            Log.Debug?.Write($"mod.json settings are:({settingsJSON})");
            Mod.Config.LogConfig();

            if (settingsE != null) {
                Log.Info?.Write($"ERROR reading settings file! Error was: {settingsE}");
            } else {
                Log.Info?.Write($"INFO: No errors reading settings file.");
            }

            var harmony = HarmonyInstance.Create(HarmonyPackage);

            // Initialize modules
            CombatFixes.InitModule(harmony);
            MiscFixes.InitModule(harmony);
            UIFixes.InitModule(harmony);

            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Setup the diag for me 
            //Helper.DiagnosticLogger.PatchAllMethods();
        }

    }
}
