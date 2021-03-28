using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRTweaks
{
    public static class ModStats
    {
        public const string CalledShot_AlwaysAllow = "IRTCalledShotAlwaysAllow";
        public const string CalledShot_AttackMod = "IRTCalledShotMod";

        public const string EnableMultiTarget = "IRAllowMultiTarget";

        public const string IgnoreHeadInjuries = "IRIgnoreHeadInjuries";

        public const string HBS_AmmoBox_CurrentAmmo = "CurrentAmmo";
        public const string HBS_FactionRep = "FactionReputation";
        public const string HBS_MaxTargets = "MaxTargets";
        public const string HBS_RandomMechs = "StartingRandomMechLists";
        public const string HBS_StrayShotValidTargets = "StrayShotValidTargets";
        public const string HBS_StrayShotEnabler = "StrayShotsEnabled";
        public const string HBS_StrayShotHitsUnits = "StrayShotsHitUnits";

        public const string HBS_Building_Structure = "Structure";

    }
    public class StabilityMod
    {
        public string StatName;
        public float Probability;
        public float Multiplier;
    }
    public class HeatMod
    {
        public string StatName;
        public float Probability;
        public float Multiplier;
    }
    public class APDmgMod
    {
        public string StatName;
        public float Probability;
        public float Multiplier;
    }
    public class StdDmgMod
    {
        public string StatName;
        public float Probability;
        public float Multiplier;
    }

}
