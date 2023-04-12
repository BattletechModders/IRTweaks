using IRBTModUtils;
using IRBTModUtils.Extension;
using System.Collections.Generic;
using us.frostraptor.modUtils;

namespace IRTweaks.Helper
{
    public static class ProtectionHelper
    {
        public static void ProtectActor(AbstractActor actor)
        {
            if (actor is Turret turret)
            {
                Mod.Log.Info?.Write($" Spawn protection skipping turret:{CombatantUtils.Label(actor)}");
                return;
            }

            if (!ModState.SpawnProtectedUnits.Contains(actor.GUID))
            {
                Mod.Log.Info?.Write($" Spawn protecting actor: {actor.DistinctId()}");
                if (Mod.Config.Combat.SpawnProtection.ApplyGuard)
                {
                    Mod.Log.Info?.Write($" -- applying Braced state");
                    actor.ApplyBraced();
                }

                if (Mod.Config.Combat.SpawnProtection.EvasionPips > 0)
                {
                    Mod.Log.Info?.Write($" -- setting evasion pips to: {Mod.Config.Combat.SpawnProtection.EvasionPips}");
                    actor.EvasivePipsCurrent = Mod.Config.Combat.SpawnProtection.EvasionPips;
                    //Traverse.Create(actor).Property("EvasivePipsTotal").SetValue(actor.EvasivePipsCurrent);
                    //AccessTools.Property(typeof(AbstractActor), "EvasivePipsTotal").SetValue(actor, actor.EvasivePipsCurrent, null);
                    actor.EvasivePipsTotal = actor.EvasivePipsCurrent;
                    SharedState.Combat.MessageCenter.PublishMessage(new EvasiveChangedMessage(actor.GUID, actor.EvasivePipsCurrent));
                }

                ModState.SpawnProtectedUnits.Add(actor.GUID);
            }
            else
            {
                Mod.Log.Info?.Write($"Actor: {actor.DistinctId()} already protected, skipping.");
            }

        }

        public static void ProtectActors(List<AbstractActor> actorsToProtect)
        {
            Mod.Log.Info?.Write($"Applying spawn protection to {actorsToProtect.Count} units");
            foreach (AbstractActor actor in actorsToProtect)
            {
                if (actor is Turret turret)
                {
                    Mod.Log.Info?.Write($" Spawn protection skipping turret:{CombatantUtils.Label(actor)}");
                    continue;
                }

                if (!ModState.SpawnProtectedUnits.Contains(actor.GUID))
                {
                    Mod.Log.Info?.Write($" Spawn protecting actor: {actor.DistinctId()}");
                    if (Mod.Config.Combat.SpawnProtection.ApplyGuard)
                    {
                        Mod.Log.Info?.Write($" -- applying Braced state");
                        actor.ApplyBraced();
                    }

                    if (Mod.Config.Combat.SpawnProtection.EvasionPips > 0)
                    {
                        Mod.Log.Info?.Write($" -- setting evasion pips to: {Mod.Config.Combat.SpawnProtection.EvasionPips}");
                        actor.EvasivePipsCurrent = Mod.Config.Combat.SpawnProtection.EvasionPips;
                        //AccessTools.Property(typeof(AbstractActor), "EvasivePipsTotal").SetValue(actor, actor.EvasivePipsCurrent, null);
                        actor.EvasivePipsTotal = actor.EvasivePipsCurrent;
                        SharedState.Combat.MessageCenter.PublishMessage(new EvasiveChangedMessage(actor.GUID, actor.EvasivePipsCurrent));
                    }

                    ModState.SpawnProtectedUnits.Add(actor.GUID);
                }
                else
                {
                    Mod.Log.Info?.Write($"Actor: {actor.DistinctId()} already protected, skipping.");
                }
            }
        }
    }
}
