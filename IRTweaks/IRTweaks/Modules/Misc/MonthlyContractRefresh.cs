using System.Collections.Generic;
using System.Linq;

namespace IRTweaks.Modules.Misc
{
    public static class MonthlyContractRefresh
    {
        public static bool IsProcedural(SimGameState sim, Contract contract)
        {
            return string.IsNullOrEmpty(contract.GUID) || sim.contractBits == null || sim.contractBits.All(bit => bit.GUID != contract.GUID);
        }

        public static void OnMonthEnd(SimGameState sim)
        {
            StarSystem system = sim.CurSystem;
            if (system is not { InitialContractsFetched: true })
            {
                return;
            }
            Contract selectedContract = sim.SelectedContract;
            bool selectedContractListed = selectedContract != null && (system.SystemContracts.Contains(selectedContract) || system.SystemBreadcrumbs.Contains(selectedContract));
            int removedContracts = system.SystemContracts.RemoveAll(c => IsProcedural(sim, c));
            int removedBreadcrumbs = system.SystemBreadcrumbs.RemoveAll(c => c != sim.activeBreadcrumb);
            system.CurMaxContracts = system.GetSystemMaxContracts();
            if (selectedContractListed && !system.SystemContracts.Contains(selectedContract) && !system.SystemBreadcrumbs.Contains(selectedContract))
            {
                sim.SetSelectedContract();
            }
            Mod.Log.Info?.Write($"MonthlyContractRefresh: month end in {system.ID}, removed {removedContracts} procedural contracts and {removedBreadcrumbs} procedural travel contracts, kept {system.SystemContracts.Count} contracts and {system.SystemBreadcrumbs.Count} travel contracts, CurMaxContracts {system.CurMaxContracts}");
            Generate(sim);
            SGContractsWidget widget = sim.RoomManager?.CmdCenterRoom?.contractsWidget;
            if (widget != null && widget.Visible)
            {
                widget.ListContracts(sim.GetAllCurrentlySelectableContracts());
            }
        }

        private static void Generate(SimGameState sim)
        {
            sim.GeneratePotentialContracts(false, null);
            if (sim.NearestToTarget != null)
            {
                sim.GeneratePotentialContracts(false, null, sim.NearestToTarget);
            }
            Mod.Log.Info?.Write($"MonthlyContractRefresh: generated {sim.CurSystem.SystemContracts.Count} contracts and {sim.CurSystem.SystemBreadcrumbs.Count} travel contracts for system {sim.CurSystem.ID}");
        }
    }

    public class NonProceduralContractsRestorationState
    {
        private readonly StarSystem system;
        private readonly List<Contract> nonProceduralContracts;

        public NonProceduralContractsRestorationState(StarSystem system, List<Contract> nonProceduralContracts)
        {
            this.system = system;
            this.nonProceduralContracts = nonProceduralContracts;
        }

        public void RestoreNonProceduralContracts()
        {
            system.SystemContracts.AddRange(nonProceduralContracts);
        }
    }

    [HarmonyPatch(typeof(SimGameState), "GeneratePotentialContracts")]
    public static class SimGameState_GeneratePotentialContracts_MonthlyContractRefresh
    {
        private static bool Prepare() => Mod.Config.Fixes.MonthlyContractRefresh;

        private static void Prefix(SimGameState __instance, bool clearExistingContracts, StarSystem systemOverride, ref bool useCoroutine, out NonProceduralContractsRestorationState __state)
        {
            __state = null;
            if (clearExistingContracts || systemOverride != null)
            {
                return;
            }
            useCoroutine = false;
            StarSystem system = __instance.CurSystem;
            List<Contract> nonProceduralContracts = system.SystemContracts.Where(c => !MonthlyContractRefresh.IsProcedural(__instance, c)).ToList();
            if (nonProceduralContracts.Count == 0)
            {
                return;
            }
            system.SystemContracts.RemoveAll(nonProceduralContracts.Contains);
            __state = new NonProceduralContractsRestorationState(system, nonProceduralContracts);
        }

        private static void Finalizer(NonProceduralContractsRestorationState __state)
        {
            __state?.RestoreNonProceduralContracts();
        }
    }

    [HarmonyPatch(typeof(SimGameState), "DeductQuarterlyFunds")]
    public static class SimGameState_DeductQuarterlyFunds_MonthlyContractRefresh
    {
        private static bool Prepare() => Mod.Config.Fixes.MonthlyContractRefresh;

        private static void Postfix(SimGameState __instance)
        {
            MonthlyContractRefresh.OnMonthEnd(__instance);
        }
    }
}
