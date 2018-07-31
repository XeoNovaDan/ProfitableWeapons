using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace ProfitableWeapons
{
    class SpecialThingFilterWorker_NonLootedWeapon : SpecialThingFilterWorker
    {

        public override bool Matches(Thing t)
        {
            if (t.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp)
                return !lootedComp.IsLootedWeapon;
            return false;
        }

    }
}
