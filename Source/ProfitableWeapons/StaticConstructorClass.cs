using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace ProfitableWeapons
{
    [StaticConstructorOnStartup]
    static class StaticConstructorClass
    {

        static StaticConstructorClass()
        {
            // Dynamically patch all ThingDefs that are weapons
            foreach (ThingDef weaponDef in DefDatabase<ThingDef>.AllDefs.Where(d => d.IsWeapon && !d.HasComp(typeof(CompLootedWeapon))))
            {
                // Set sell price multiplier based on settings
                weaponDef.SetStatBaseValue(StatDefOf.SellPriceFactor, ProfitableWeaponsSettings.nonLootedSellPriceMult);

                // CompLootedWeapon
                if (weaponDef.comps == null)
                    weaponDef.comps = new List<CompProperties>();
                weaponDef.comps.Add(new CompProperties
                {
                    compClass = typeof(CompLootedWeapon)
                });
            }

        }

    }
}
