using System.Linq;
using System.Reflection;

namespace IRTweaks.Helper {

    public static class DiagnosticLogger {
        public static void PatchAllMethods() {
            Mod.Log.Debug?.Write("=== Initializing Diagnostics Logger ====");
            var assembly = Assembly.GetAssembly(typeof(AIUtil));
            var names = (from type in assembly.GetTypes()
                         from method in type.GetMethods(
                           BindingFlags.Public | BindingFlags.NonPublic |
                           BindingFlags.Instance | BindingFlags.Static)
                         select type.FullName + ":" + method.Name).Distinct().ToList();

            foreach (string fqn in names) {
                Mod.Log.Debug?.Write($"Found fqn:{fqn}");
            }

            Mod.Log.Debug?.Write("=== End Diagnostics Logger ====");
        }
    }
}
