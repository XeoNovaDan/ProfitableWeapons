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

        static ModCompatibilityCheck()
        {
            var activeMods = ModsConfig.ActiveModsInLoadOrder.ToList();
            for (int i = 0; i < activeMods.Count; i++)
            {
                var curMod = activeMods[i];
                if (curMod.Name == "Combat Extended")
                    CombatExtended = true;
                else if (curMod.Name == "MendAndRecycle")
                    Mending = true;
                else if (curMod.Name == "Nano Repair Tech")
                    NanoRepairTech = true;
            }
        }

        public static bool CombatExtended;

        public static bool Mending;

        public static bool NanoRepairTech;

    }
}
