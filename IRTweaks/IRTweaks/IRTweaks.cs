using Harmony;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;

namespace IRTweaks {

    public static class Mod {

        public const string HarmonyPackage = "us.frostraptor.IRTweaks";
        public const string LogName = "ir_tweaks";

        public static Logger Log;
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

            Log = new Logger(modDirectory, LogName);

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            Log.Info($"Assembly version: {fvi.ProductVersion}");

            Log.Debug($"ModDir is:{modDirectory}");
            Log.Debug($"mod.json settings are:({settingsJSON})");
            Mod.Config.LogConfig();

            if (settingsE != null) {
                Log.Info($"ERROR reading settings file! Error was: {settingsE}");
            } else {
                Log.Info($"INFO: No errors reading settings file.");
            }

            var harmony = HarmonyInstance.Create(HarmonyPackage);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Setup the diag for me 
            //Helper.DiagnosticLogger.PatchAllMethods();
        }

    }
}
