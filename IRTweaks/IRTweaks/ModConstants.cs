using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using Localize;

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

    public enum DmgModType
    {
        NOT_SET,
        Stability,
        Heat,
        AP,
        Standard
    }

    public class ActiveDmgMod
    {
        public string WeaponGUID;
        public DmgModType Type;
        public float Mult;
        public Text Txt;

        public ActiveDmgMod(string weaponGUID, DmgModType type, float mult, Text txt)
        {
            this.WeaponGUID = weaponGUID;
            this.Type = type;
            this.Mult = mult;
            this.Txt = txt;
        }
    }

    public class ToHitMod
    {
        public string SourceStatName;
        public string TargetStatName;
        public bool Multi;
        public string Type; //options are EVASIVE (mod on existing evasive pips modifier), ABSOLUTE (straight accuracy buff/debuff), DEFENSE (mod on existing ToHitThisActor of target) 
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
