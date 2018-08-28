using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace ProfitableWeapons
{
    public class SpecialThingFilterWorker_LootedWeapon : SpecialThingFilterWorker
    {

        public override bool Matches(Thing t)
        {
            return (t.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp) ? lootedComp.IsLootedWeapon : false;
        }

    }
}
