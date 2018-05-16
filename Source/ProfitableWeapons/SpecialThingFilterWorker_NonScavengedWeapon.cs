using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace ProfitableWeapons
{
    class SpecialThingFilterWorker_NonScavengedWeapon : SpecialThingFilterWorker
    {

        public override bool Matches(Thing t)
        {
            ThingWithComps weapon = t as ThingWithComps;
            if (weapon != null)
            {
                CompScavengedWeapon comp = weapon.TryGetComp<CompScavengedWeapon>();
                if (comp != null)
                {
                    return !comp.IsScavengedWeapon;
                }
                return false;
            }
            return false;
        }

    }
}
