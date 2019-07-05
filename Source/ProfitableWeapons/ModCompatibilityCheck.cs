using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ProfitableWeapons
{
    [StaticConstructorOnStartup]
    public class ModCompatibilityCheck
    {

        public static bool CombatExtended => ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Combat Extended");

        public static bool Mending => ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "MendAndRecycle");

        public static bool NanoRepairTech => ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Nano Repair Tech");


    }
}
