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

        public static bool MendingIsActive
        {
            get
            {
                return ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Mending");
            }
        }

    }
}
